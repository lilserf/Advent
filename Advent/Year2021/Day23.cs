using Advent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2021
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

        public AmphiState(int a1, int a2, int b1, int b2, int c1, int c2, int d1, int d2)
        {
            Positions = new int[] { a1, a2, b1, b2, c1, c2, d1, d2 };
        }

        public static AmphiState TestMap() => new AmphiState(12, 18, 11, 15, 13, 16, 14, 17);
        public static AmphiState Problem() => new AmphiState(12, 18, 1, 9, 0, 16, 10, 17);

        public AmphiState(AmphiState other)
        {
            for(int i=0; i < 8; i++)
            {
                Positions[i] = other.Positions[i];
            }
            Cost = other.Cost;
        }

        public override bool Equals(object obj)
        {
            if(obj is AmphiState other)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (other.Positions[i] != Positions[i])
                        return false;
                }
                return true;

            }

            return false;
        }

        public bool Equals(int a1, int a2, int b1, int b2, int c1, int c2, int d1, int d2)
        {
            return
                (Positions[0] == a1) &&
                (Positions[1] == a2) &&
                (Positions[2] == b1) &&
                (Positions[3] == b2) &&
                (Positions[4] == c1) &&
                (Positions[5] == c2) &&
                (Positions[6] == d1) &&
                (Positions[7] == d2);

        }

        //#############
        //#...........#
        //###C#D#A#B###
        //  #B#A#D#C#
        //  #########

        public void Draw(int indent = 0)
        {
            var Indent = (int x) =>
            {
                for (int k = 0; k < x; k++)
                {
                    Console.Write(" ");
                }
            };

            Indent(indent);
            Console.WriteLine("#############");
            Indent(indent);
            Console.Write("#");

            var DrawCell = (int x) =>
            {
                string toDraw = ".";
                for (int j = 0; j < Positions.Length; j++)
                {
                    if (Positions[j] == x)
                    {
                        switch (GetPodType(j))
                        {
                            case Amphipod.Amber:
                                toDraw ="A"; break;
                            case Amphipod.Bronze:
                                toDraw = "B"; break;
                            case Amphipod.Copper:
                                toDraw = "C"; break;
                            case Amphipod.Desert:
                                toDraw = "D"; break;
                        }
                    }
                }

                Console.Write(toDraw);
            };

            for(int i=0; i < 11; i++)
            {
                DrawCell(i);
            }
            Console.WriteLine("#");

            Indent(indent);
            Console.Write("###");
            DrawCell(11);
            Console.Write("#");
            DrawCell(13);
            Console.Write("#");
            DrawCell(15);
            Console.Write("#");
            DrawCell(17);
            Console.WriteLine("###");

            Indent(indent);
            Console.Write("  #");
            DrawCell(12);
            Console.Write("#");
            DrawCell(14);
            Console.Write("#");
            DrawCell(16);
            Console.Write("#");
            DrawCell(18);
            Console.WriteLine("#");

            Indent(indent);
            Console.WriteLine("  #########");
        }

        public override string ToString()
        {
            return Positions.Select(x => x.ToString()).Aggregate( (s, x) => s + " " + x) + $" ({Cost} & {DoneScore()})";
        }

        public override int GetHashCode()
        {
            return Positions.GetHashCode();
        }

        // Positions of the 'pods - A, A, B, B, C, C, D, D
        public int[] Positions { get; set; } = new int[8];
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

        private bool IsPodCorrect(int x)
        {
            int type = x / 2;
            int curr = Positions[x];
            if (curr <= 10) return false;
            return type == (curr - 11) / 2;
        }

        private bool IsPodBlocked(int x)
        {
            // If in the right spot, not blocked
            if (IsPodCorrect(x)) return false;
            int curr = Positions[x];
            // If in hallway, not blocked
            if (curr <= 10) return false;
            // If in top row of homes, not blocked
            if(((curr-11) % 2) == 0) return false;

            // Return true/false based on the guy above us
            int top = curr - 1;
            for(int i=0; i < 8; i++)
            {
                if (Positions[i] == top)
                    return true;
            }

            return false;
        }

        public int DoneScore()
        {
            int penalty = 100000;
            int score = 0;
            for(int i=0; i < 8; i++)
            {
                if (!IsPodCorrect(i))
                    score += penalty;
                //if(IsPodBlocked(i))
                //    score += penalty;
            }

            return score;
        }

        public int CompareTo(AmphiState other)
        {
            return (Cost + DoneScore()) - (other.Cost + other.DoneScore());
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

        public bool IsStoppable(Burrow map, int room, int previous, Amphipod podType)
        {
            // If we started in a hallway, we can't stop in one
            if (IsRoomHallway(room) && IsRoomHallway(previous))
                return false;

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
            if (room < 11) return true;
            return pod == CorrectTypeForRoom(room);
        }

        public Amphipod CorrectTypeForRoom(int room)
        {
            if (room < 11) return Amphipod.None;

            switch((room - 11) / 2)
            {
                case 0: return Amphipod.Amber;
                case 1: return Amphipod.Bronze;
                case 2: return Amphipod.Copper;
                case 3: return Amphipod.Desert;
                default: return Amphipod.None;
            }
        }

        public bool IsRoomHallway(int room)
        {
            return room < 11;
        }

        public bool HomeIsDone(AmphiState state, int room)
        {
            int home = (room-11) / 2;

            return ((TypeInRoom(state, home + 11) == CorrectTypeForRoom(home + 11) || TypeInRoom(state, home + 11) == Amphipod.None) &&
                TypeInRoom(state, home + 12) == CorrectTypeForRoom(home + 12));
        }

        public bool CanEnter(Burrow map, AmphiState state, int srcRoom, int destRoom, Amphipod podType)
        {
            // If the room is occupied, we can't move there
            if (IsOccupied(map, state, destRoom))
                return false;

            // If we're already in our home room, we can't move out
            if (!IsRoomHallway(srcRoom) && HomeIsDone(state, srcRoom))
                return false;

            // If we don't match the dest room
            if(!PodMatchesRoom(podType, destRoom))
            {
                // It's only okay if we don't match the source room either - meaning we were at the far end of a room to start
                return !PodMatchesRoom(podType, srcRoom);
            }

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
                if(!IsOccupied(map, state, room) && IsStoppable(map, room, currRoom, podType))
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

            //AmphiState start = AmphiState.Problem();
            AmphiState start = AmphiState.TestMap();
            MinHeap<AmphiState> pending = new(100000);
            pending.Add(start);
            
            Dictionary<AmphiState, int> visited = new();

            int DoneScore = int.MaxValue;

            while(pending.Count() > 0)
            {
                var curr = pending.Pop();
                visited.Add(curr, curr.Cost);
                Console.WriteLine($"Checking state {curr}");
                curr.Draw();
                if (curr.Done())
                    return $"Cost is {curr.Cost}";

                if (curr.Equals(12, 18, 11, 3, 13, 16, 14, 17)) // Step 2
                //if (curr.Equals(12, 18, 11, 3, 15, 16, 14, 17)) // Step 3
                {
                    Debugger.Break();
                }


                for (int i=0; i < 8; i++)
                {
                    var moves = GetMoves(map, curr, i);
                    Console.WriteLine($"  Checking Amphipod {i}... {moves.Count()} moves!");

                    foreach ((var position, var cost) in moves)
                    {
                        Console.WriteLine($"    Move to {position} for cost {cost}");
                        var newState = curr.ApplyMove(i, position, cost);
                        Console.WriteLine($"      New state {newState}");
//                        newState.Draw(8);

                        if (visited.ContainsKey(newState))
                            Debugger.Break();
                        pending.Add(newState);

                    }
                }

                //Console.ReadKey();

                if(curr.DoneScore() < DoneScore)
                {
                    DoneScore= curr.DoneScore();
                    Console.ReadKey();
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
