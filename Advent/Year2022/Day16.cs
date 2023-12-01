using Advent;
using Advent.Year2021;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Year2022
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

        State RunToEnd(State curr, int endMin = 30)
        {
            State newState = new State(curr);
            for (int i = curr.Minutes; i < endMin; i++)
            {
                newState = Tick(newState);
            }
            return newState;
        }

        Dictionary<(string, string), int> m_distances = new();

        public override string Part1()
        {

            foreach(var start in m_rooms.Keys)
            {
                foreach(var end in m_rooms.Keys)
                {
                    int distance = Distance(start, end);
                    m_distances[(start, end)] = distance;
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
                    int elapsed = m_distances[(curr.CurrRoom, room.Name)];
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


        class Part2State : IComparable<Part2State>
        {
            public string[] Rooms { get; set; } = new string[2] { "AA", "AA" };
            public int[] Arrivals { get; set; } = new int[2] { 0, 0 };
            public int Minutes { get; set; } = 0;
            public int FlowRate { get; set; } = 0;
            public int Total { get; set; } = 0;

            public List<string> OpenedValves { get; set; } = new();
            public List<string> Log { get; set; } = new();

            public Part2State()
            {
            }

            public Part2State(Part2State other)
            {
                Minutes = other.Minutes;
                Rooms[0] = other.Rooms[0];
                Rooms[1] = other.Rooms[1];
                Arrivals[0] = other.Arrivals[0];
                Arrivals[1] = other.Arrivals[1];
                FlowRate = other.FlowRate;
                Total = other.Total;
                OpenedValves = new List<string>(other.OpenedValves);
                Log = new List<string>(other.Log);
            }


            public int Projected => Total+ (FlowRate * (26 - Minutes));
            public int CompareTo(Part2State other)
            {
                if (other.Projected != this.Projected)
                    return other.Projected - this.Projected;
                if (other.Total!= this.Total)
                    return other.Total- this.Total;
                return this.Minutes - other.Minutes;
            }

            public override string ToString()
            {
                return $"Min {Minutes} : {OpenedValves.Aggregate("", (s, x) => s += " " + x)} -> Me({Rooms[0]}, arr {Arrivals[0]}) Elephant({Rooms[1]}, arr {Arrivals[1]}) :  : Total {Total} : FlowRate {FlowRate} : Projected {Projected}";
            }

        }

        Part2State RunToEnd(Part2State curr, int endMin = 26)
        {
            Part2State newState = new(curr);
            newState.Log.Add($"{newState.Minutes}: Running to end!");
            for (int i = curr.Minutes; i < endMin; i++)
            {
                newState = Tick(newState);
            }
            return newState;
        }

        Part2State Tick(Part2State curr)
        {
            Part2State newState = new (curr);
            newState.Minutes++;
            newState.Total += newState.FlowRate;
            newState.Log.Add($"{curr.Minutes}: Ticked to {newState.Minutes} and released {newState.FlowRate} for total {newState.Total}");
            return newState;
        }

        Part2State SetDestination(Part2State curr, string room, int index)
        {
            Part2State newState = new(curr);
            newState.Rooms[index] = room;
            var dist = m_distances[(curr.Rooms[index], room)];
            newState.Arrivals[index] = newState.Minutes + dist + 1;
            newState.Log.Add($"{curr.Minutes}: Picked next room {room} (from {curr.Rooms[index]} that's a distance of {dist}) with arrival {newState.Arrivals[index]} for actor {index}");
            return newState;
        }

        Part2State OpenValve(Part2State curr, string room)
        {
            if (curr.OpenedValves.Contains(room))
                throw new InvalidOperationException();

            Room r = m_rooms[room];
            Part2State newState = new (curr);
            newState.FlowRate += r.FlowRate;
            newState.OpenedValves.Add(room);
            newState.Log.Add($"{newState.Minutes}: Opened valve in room {room}, flow rate now {newState.FlowRate}");

            return newState;
        }

        void PrintState(Part2State state)
        {
            Console.WriteLine($"== {state}");
            foreach(var line in state.Log)
            {
                Console.WriteLine($"  {line}");
            }
        }

        public override string Part2()
        {
            MinHeap<Part2State> heap = new(1000);

            Part2State first = new();
            heap.Add(first);

            int max = 0;
            Part2State maxState = null;

            while (heap.Count() > 0)
            {
                var curr = heap.Pop();
                //Console.WriteLine($"Checking state {curr}...");

                // If this state has exceeded 26 minutes, record and abandon it
                if (curr.Minutes >= 26)
                {
                    if (curr.Total > max)
                    {
                        max = curr.Total;
                        maxState = curr;
                        Console.WriteLine($"New max of {max} for order {curr.OpenedValves.Aggregate("", (s, x) => s += " " + x)}");
                    }
                    continue;
                }

                //if (curr.OpenedValves.Count >= 2 &&
                //    curr.OpenedValves[0] == "DD" &&
                //    curr.OpenedValves[1] == "JJ" &&
                //    curr.Rooms[0] == "BB" &&
                //    curr.Rooms[1] == "HH")
                //{
                //    PrintState(curr);
                //    Debugger.Break();
                //}


                var newState = new Part2State(curr);
                List<int> movers = new();
                bool valveOpened = false;
                // Some character(s) has arrived at their location!
                // Turn on their valve(s)!
                for (int i = 0; i < 2; i++)
                {
                    if (newState.Arrivals[i] <= newState.Minutes)
                    {
                        //newState.Log.Add($"{newState.Minutes}: Actor {i} arrived at room {newState.Rooms[i]}");
                        var room = newState.Rooms[i];
                        // If we're not on a useless valve
                        if (m_rooms[room].FlowRate > 0 && !newState.OpenedValves.Contains(room))
                        {
                            newState = OpenValve(newState, newState.Rooms[i]);
                            valveOpened = true;
                        }
                        movers.Add(i);
                    }
                }

                //// Tick once for the valve-turning, but not in room AA at time 0
                //if(valveOpened)
                //    newState = Tick(newState);

                if(newState.OpenedValves.Count >= m_rooms.Values.Where(x => x.FlowRate > 0).Count())
                {
                    newState = RunToEnd(newState);
                    heap.Add(newState);
                }

                List<Part2State> newStates = new();

                foreach (int index in movers)
                {
                    bool atLeastOneMove = false;
                    var currRoom = newState.Rooms[index];

                    // Now pick all the possible next valve locations they could move
                    foreach (var room in m_rooms.Values)
                    {
                        var moveState = new Part2State(newState);
                        // Don't move where a character already is
                        if (moveState.Rooms.Any(x => x == room.Name))
                            continue;

                        // Don't move to a useless valve
                        if (room.FlowRate == 0)
                            continue;

                        // Don't move to a valve that's already open
                        if (curr.OpenedValves.Contains(room.Name))
                            continue;

                        // Don't move where the other guy is heading
                        var otherIndex = index == 0 ? 1 : 0;
                        if (curr.Rooms[otherIndex] == room.Name)
                            continue;

                        // Set the destination for this character
                        moveState = SetDestination(moveState, room.Name, index);
                        atLeastOneMove = true;
                        newStates.Add(moveState);
                    }

                    if(!atLeastOneMove)
                    {
                        // We're done moving, max our arrival time
                        newState.Arrivals[index] = 26;
                        newStates.Add(newState);
                    }
                }

                // For each new state we built, simulate to the next character's arrival
                foreach (var state in newStates)
                {
                    Part2State tempState = state;
                    int minTime = tempState.Arrivals.Min();

                    while (tempState.Minutes < minTime)
                    {
                        tempState = Tick(tempState);
                    }
                    //Console.WriteLine($"  Adding state {tempState}...");
                    heap.Add(tempState);
                }


            }

            Console.WriteLine($"Best state found was {maxState}!");
            PrintState(maxState);
            return max.ToString(); ;
        }

    }
}
