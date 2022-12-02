using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2021
{

    //#############
    //#...........#
    //###C#D#A#B###
    //  #B#A#D#C#
    //  #########

    enum Amphipod
    {
        Amber,
        Bronze,
        Copper,
        Desert,
        None
    };

    abstract class Node
    {
        public int Index { get; private set; }
        public Node(int index)
        {
            Index = index;
        }
        public abstract bool CanStopHere(Amphipod pod);
    }

    class Room : Node
    {
        Amphipod HomeType;

        public Room(int index, Amphipod homeType)
            : base(index)
        {
            HomeType = homeType;
        }

        public override bool CanStopHere(Amphipod pod)
        {
            return pod == HomeType;
        }

    }

    class Hallway : Node
    {
        public Hallway(int index)
            :base(index) {}

        public override bool CanStopHere(Amphipod pod)
        {
            return true;
        }

    }

    class EmptyHallway : Node
    {
        public EmptyHallway(int index)
            :base(index) { }
        public override bool CanStopHere(Amphipod pod)
        {
            return false;
        }

    }

 

    class Burrow : IComparable<Burrow>
    {
        static Node[] PART1_MAP =
        {
                new Hallway(0),
                new Hallway(1),
                new EmptyHallway(2),
                new Hallway(3),
                new EmptyHallway(4),
                new Hallway(5),
                new EmptyHallway(6),
                new Hallway(7),
                new EmptyHallway(8),
                new Hallway(9),
                new Hallway(10),
                new Room(11, Amphipod.Amber), new Room(12, Amphipod.Amber),
                new Room(13, Amphipod.Bronze), new Room(14, Amphipod.Bronze),
                new Room(15, Amphipod.Copper), new Room(16, Amphipod.Copper),
                new Room(17, Amphipod.Desert), new Room(18, Amphipod.Desert),
        };

        public static Dictionary<Amphipod, int> CostLookup = new()
        {
            [Amphipod.Amber] = 1,
            [Amphipod.Bronze] = 10,
            [Amphipod.Copper] = 100,
            [Amphipod.Desert] = 1000
        };

        public int CostSoFar { get; set; } = 0;

        public Node[] Map { get; } = PART1_MAP;

        public static int[][] Neighbors =
        {
            new int[] {1},
            new int[] {0, 2},
            new int[] {1, 3, 11},
            new int[] {2, 4},
            new int[] {3, 5, 13},
            new int[] {4, 6},
            new int[] {5, 7, 15},
            new int[] {6, 8},
            new int[] {7, 9, 17},
            new int[] {8, 10},
            new int[] {9},
            new int[] {2, 12},
            new int[] {11},
            new int[] {4, 14},
            new int[] {13},
            new int[] {6, 16},
            new int[] {15},
            new int[] {8, 18},
            new int[] {17}
        };


        public int CompareTo(Burrow other)
        {
            return CostSoFar - other.CostSoFar;
        }
    }


    class AmphiState : IComparable<AmphiState>
    {
        public AmphiState()
        {
        }

        public AmphiState(AmphiState other)
        {
            for(int i=0; i < 8; i++)
            {
                Positions[i] = other.Positions[i];
            }
            Cost = other.Cost;
        }

        public override string ToString()
        {
            return Positions.Select(x => x.ToString()).Aggregate( (s, x) => s + " " + x) + $" ({Cost})";
        }

        // Positions of the 'pods - A, A, B, B, C, C, D, D
        public int[] Positions { get; set; } =
        {
            12, 18, 11, 15, 13, 16, 14, 17
        };
        public int Cost = 0;

        public AmphiState ApplyMove(int pod, int position, int cost)
        {
            var newState = new AmphiState(this);
            newState.Positions[pod] = position;
            newState.Cost += cost;
            return newState;
        }

        public bool Done()
        {
            return
                (Positions[0] == 11 || Positions[0] == 12) &&
                (Positions[1] == 11 || Positions[1] == 12) &&
                (Positions[2] == 13 || Positions[2] == 14) &&
                (Positions[3] == 13 || Positions[3] == 14) &&
                (Positions[4] == 15 || Positions[4] == 16) &&
                (Positions[5] == 15 || Positions[5] == 16) &&
                (Positions[6] == 17 || Positions[6] == 18) &&
                (Positions[7] == 17 || Positions[7] == 18);
        }

        public int CompareTo(AmphiState other)
        {
            return Cost - other.Cost;
        }

        public static Amphipod GetPodType(int index)
        {
            switch(index)
            {
                case 0:
                case 1:
                    return Amphipod.Amber;
                case 2:
                case 3:
                    return Amphipod.Bronze;
                case 4:
                case 5:
                    return Amphipod.Copper;
                default:
                    return Amphipod.Desert;
            }
        }
    }

    internal class Day23 : DayLineLoaderBasic
    {
        public override void ParseInputLine(string line)
        {
            // Do nothing
        }

        public Burrow SetupTestMap()
        {
            var b = new Burrow();
            return b;
        }

        public bool IsOccupied(Burrow map, AmphiState state, int roomIndex)
        {
            return state.Positions.Any(x => x == roomIndex);
        }

        public bool IsStoppable(Burrow map, int room, Amphipod podType)
        {
            return map.Map[room].CanStopHere(podType);
        }

        public Amphipod TypeInRoom(AmphiState state, int room)
        {
            for(int i=0; i < state.Positions.Length; i++)
            {
                if (state.Positions[i] == room)
                    return AmphiState.GetPodType(i);
            }
            return Amphipod.None;
        }

        public bool PodMatchesRoom(Amphipod pod, int room)
        {
            switch(room)
            {
                case 11:
                case 12:
                    return pod == Amphipod.Amber;
                case 13:
                case 14:
                    return pod == Amphipod.Bronze;
                case 15:
                case 16:
                    return pod == Amphipod.Copper;
                case 17:
                case 18:
                    return pod == Amphipod.Desert;
            }

            return true;
        }

        public bool IsRoomHallway(int room)
        {
            return room < 11;
        }

        public bool CanEnter(Burrow map, AmphiState state, int srcRoom, int destRoom, Amphipod podType)
        {
            // If the room is occupied, we can't move there
            if (IsOccupied(map, state, destRoom))
                return false;

            // If we're already in our home room, we can't move out
            if (!IsRoomHallway(srcRoom) && PodMatchesRoom(podType, srcRoom))
                return false;

            // If we don't match the dest room
            if(!PodMatchesRoom(podType, destRoom))
            {
                // It's only okay if we don't match the source room either - meaning we were at the far end of a room to start
                return !PodMatchesRoom(podType, srcRoom);
            }

            // If we're in a hallway, we can't move to another hallway
            if(IsRoomHallway(srcRoom) && IsRoomHallway(destRoom))
                return false;

            // If we got here, apparently it's okay to enter this square
            return true;
        }

        // For the given map, state, and amphipod index in the State, return location indices it can reach and the cost to do so
        public IEnumerable<(int, int)> GetMoves(Burrow map, AmphiState state, int pod)
        {
            Amphipod podType = AmphiState.GetPodType(pod);
            int currRoom = state.Positions[pod];
            Queue<(int, int)> pending = new();
            pending.Enqueue((currRoom, 0));

            HashSet<int> visited = new();
            Dictionary<int, int> moves = new();

            while(pending.Count> 0)
            {
                (var room, var cost) = pending.Dequeue();
                visited.Add(room);

                // If we reached this room and can stop, put it in the move set
                if(!IsOccupied(map, state, room) && IsStoppable(map, room, podType))
                {
                    moves[room] = cost;
                }

                foreach(var neighbor in Burrow.Neighbors[room])
                {
                    if(visited.Contains(neighbor)) continue;
                    int newCost = cost + Burrow.CostLookup[podType];

                    // If we haven't found a move to here, or the move we know is more costly than this new one, queue it
                    if (!moves.ContainsKey(neighbor) || moves[neighbor] > newCost)
                    {
                        // But only if we're actually allowed to enter this square
                        if (CanEnter(map, state, currRoom, neighbor, podType))
                        {
                            pending.Enqueue((neighbor, newCost));
                        }
                    }
                }
            }

            foreach((var key, var value) in moves)
            {
                yield return (key, value);
            }
        }

        public override string Part1()
        {
            var map = SetupTestMap();

            AmphiState start = new();
            MinHeap<AmphiState> pending = new(100000);
            pending.Add(start);
            while(pending.Count() > 0)
            {
                var curr = pending.Pop();
                Console.WriteLine($"Checking state {curr}");
                if (curr.Done())
                    return $"Cost is {curr.Cost}";

                for(int i=0; i < 8; i++)
                {
                    var moves = GetMoves(map, curr, i);
                    foreach ((var position, var cost) in moves)
                    {
                        var newState = curr.ApplyMove(i, position, cost);

                        pending.Add(newState);
                    }
                }

            }

            return "";
        }

        public override string Part2()
        {
            return "";
        }
    }
}
