using Advent;
using Advent2020.Year2021;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day16 : DayLineLoaderBasic
    {
        class Room
        {
            public string Name { get; private set; }
            public int FlowRate { get; private set; }

            public List<string> Neighbors { get; private set; }

            public Room(string name, int rate)
            {
                Name = name;
                FlowRate = rate;
                Neighbors = new List<string>();
            }
        }

        Dictionary<string, Room> m_rooms = new();

        public override void ParseInputLine(string line)
        {
            Regex reg = new Regex(@"Valve (\w+) has flow rate=(\d+); tunnels? leads? to valves? (.*)");

            Match m = reg.Match(line);
            if (m.Success)
            {
                string name = m.Groups[1].Value;
                int rate = int.Parse(m.Groups[2].Value);

                var dests = m.Groups[3].Value.Split(", ");

                Room r = new Room(name, rate);
                r.Neighbors.AddRange(dests);
                m_rooms[name] = r;
            }
            else
                throw new InvalidOperationException();
        }

        class State : IComparable<State>
        {
            public string CurrRoom { get; set; } = "AA";
            public int ReleasePerTick { get; set; } = 0;
            public int TotalReleased { get; set; } = 0;
            public int Minutes { get; set; } = 1;
            public List<string> OpenedValves { get; private set; } = new();
            public int Potential { get; set; } = 0;
            public float ReleasedPerMinute => (float)TotalReleased / Minutes;

            public int Projected => TotalReleased + (ReleasePerTick * (30 - Minutes));
            public int CompareTo(State other)
            {
                if (other.Projected != this.Projected)
                    return other.Projected - this.Projected;
                if(other.TotalReleased != this.TotalReleased)
                    return other.TotalReleased - this.TotalReleased;
                if(other.Potential != this.Potential)
                    return other.Potential - this.Potential;
                return this.Minutes - other.Minutes;
            }

            public State()
            {
            }

            public State(State other)
            {
                CurrRoom = other.CurrRoom;
                ReleasePerTick = other.ReleasePerTick;
                TotalReleased = other.TotalReleased;
                Minutes = other.Minutes;
                OpenedValves = new List<string>(other.OpenedValves);
                Potential = other.Potential;
            }

            public bool ValveIsOpen()
            {
                return OpenedValves.Contains(CurrRoom);
            }

            public override string ToString()
            {
                return $"Min {Minutes} : {OpenedValves.Aggregate("", (s,x) => s += " " + x)} -> {CurrRoom} : Total {TotalReleased} : PerTick {ReleasePerTick} : Projected {Projected}";
            }
        }

        State Tick(State curr)
        {
            State newState = new State(curr);
            newState.Minutes++;
            newState.TotalReleased += newState.ReleasePerTick;
            return newState;
        }

        State MoveTo(State curr, string room)
        {
            State newState = new State(curr);
            newState.CurrRoom = room;
            newState.Potential = newState.ValveIsOpen() ? 0 : m_rooms[room].FlowRate;
            return newState;
        }

        State OpenValve(State curr)
        {
            if (curr.OpenedValves.Contains(curr.CurrRoom))
                throw new InvalidOperationException();

            Room r = m_rooms[curr.CurrRoom];
            State newState = new State(curr);
            newState.ReleasePerTick += r.FlowRate;
            newState.OpenedValves.Add(curr.CurrRoom);
            newState.Potential = 0;
            return newState;
        }

        int AvailableFlow(State curr)
        {
            int flow = 0;
            foreach(var room in m_rooms.Values)
            {
                if (!curr.OpenedValves.Contains(room.Name))
                    flow += room.FlowRate;
            }

            return flow;
        }

        bool CanBeatMax(State curr, int allValvesOpen, int max)
        {
            return (curr.TotalReleased + (allValvesOpen * (30 - curr.Minutes)) > max);
        }

        class DistState : IComparable<DistState>
        {
            public int Distance { get; set; } = 0;
            public string Curr { get; set; } = "";

            public HashSet<string> Visited { get; set; } = new();

            public DistState(string curr)
            {
                Curr = curr;
            }

            public int CompareTo(DistState other)
            {
                return Distance - other.Distance;
            }
        }

        public int Distance(string room1Name, string room2Name)
        {
            Room room1 = m_rooms[room1Name];
            Room room2 = m_rooms[room2Name];

            MinHeap<DistState> heap = new(100000);
            DistState start = new(room1Name);
            start.Visited.Add(room1Name);
            heap.Add(start);

            while(heap.Count() > 0)
            {
                var curr = heap.Pop();
                var room = m_rooms[curr.Curr];

                if (curr.Curr == room2Name)
                    return curr.Distance;
                else
                {
                    foreach(var neighbor in room.Neighbors)
                    {
                        if(!curr.Visited.Contains(neighbor))
                        {
                            var newState = new DistState(neighbor);
                            newState.Visited = new HashSet<string>(curr.Visited);
                            newState.Visited.Add(neighbor);
                            newState.Distance = curr.Distance + 1;
                            heap.Add(newState);
                        }
                    }
                }
            }

            throw new InvalidOperationException();
        }

        State RunToEnd(State curr)
        {
            State newState = new State(curr);
            for (int i = curr.Minutes; i < 30; i++)
            {
                newState = Tick(newState);
            }
            return newState;
        }

        public override string Part1()
        {
            Dictionary<(string, string), int> distances = new();

            foreach(var start in m_rooms.Keys)
            {
                foreach(var end in m_rooms.Keys)
                {
                    int distance = Distance(start, end);
                    distances[(start, end)] = distance;
                }
            }


            MinHeap<State> heap = new(1000);

            State first = new State();
            heap.Add(first);

            int max = 0;

            while(heap.Count() > 0)
            {
                var curr = heap.Pop();
                //Console.WriteLine($"Checking state {curr}...");

                if (curr.Minutes >= 30)
                {
                    if (curr.TotalReleased > max)
                    {
                        max = curr.TotalReleased;
                        Console.WriteLine($"New max of {max} for order {curr.OpenedValves.Aggregate("", (s, x) => s += " " + x)}");
                    }
                    continue;
                }

                foreach(var room in m_rooms.Values)
                {
                    var newState = new State(curr);
                    // Don't move to ourself - instead just sim out staying here the whole time
                    if (room.Name == curr.CurrRoom)
                    {
                        newState = RunToEnd(curr);
                        heap.Add(newState);
                        continue;
                    }
                    // Don't move to a useless valve
                    if (room.FlowRate == 0)
                        continue;
                    // Don't move to a valve that's already open
                    if (curr.OpenedValves.Contains(room.Name))
                        continue;
                    int elapsed = distances[(curr.CurrRoom, room.Name)];
                    newState = MoveTo(newState, room.Name);

                    // If we can't get there, simulate finishing the 30 minutes
                    if (curr.Minutes + elapsed >= 30)
                    {
                        newState = RunToEnd(newState);
                        heap.Add(newState);
                        continue;
                    }

                    // Tick the state for the time to get there
                    for (int i=0; i < elapsed; i++)
                    {
                        newState = Tick(newState);
                    }
                    // Tick the state for a minute to open the valve
                    newState = OpenValve(newState);
                    newState = Tick(newState);

                    //Console.WriteLine($"  Adding transition to {room.Name} taking {elapsed} minutes...");
                    heap.Add(newState);
                }
            }

            return max.ToString();
        }

        public string Part1TooSlow()
        { 
            MinHeap<State> heap = new(100000);

            State start = new State();
            foreach (var neighbor in m_rooms["AA"].Neighbors)
            {
                var moveState = MoveTo(start, neighbor);
                moveState = Tick(moveState);
                if (!moveState.ValveIsOpen() && m_rooms[moveState.CurrRoom].FlowRate > 0)
                {
                    moveState = OpenValve(moveState);
                    moveState = Tick(moveState);
                }
                heap.Add(moveState);
            }

            int max = 0;

            int allValvesOpen = m_rooms.Values.Sum(x => x.FlowRate);

            int count = 0;
            while(heap.Count() > 0)
            {
                count++;
                if(count % 1000000 == 0)
                {
                    Console.WriteLine($"Heap has {heap.Count()} states, best so far is {max}");

                    //foreach (var state in heap)
                    //{
                    //    Console.WriteLine($"  {state}");
                    //}
                }

                var curr = heap.Pop();
                var room = m_rooms[curr.CurrRoom];

                if(curr.Minutes >= 30)
                {
                    if (curr.TotalReleased > max)
                    {
                        max = curr.TotalReleased;
                        Console.WriteLine($"New max of {max} for order {curr.OpenedValves.Aggregate("", (s,x) => s += " " + x)}");
                    }
                    continue;
                }

                if(!CanBeatMax(curr, allValvesOpen, max))
                    continue;

                if(!curr.ValveIsOpen() && m_rooms[curr.CurrRoom].FlowRate > 0)
                {
                    var valveState = OpenValve(curr);
                    valveState = Tick(valveState);
                    if(CanBeatMax(valveState, allValvesOpen, max))
                        heap.Add(valveState);
                }

                foreach(var neighbor in room.Neighbors)
                {
                    var moveState = MoveTo(curr, neighbor);
                    moveState = Tick(moveState);
                    if (CanBeatMax(moveState, allValvesOpen, max))
                        heap.Add(moveState);
                }
            }

            return max.ToString();
        }

        public override string Part2()
        {
            return "";
        }
    }
}
