using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2021
{
    class Scanner
    {
        public int Id => m_id;
        int m_id;

        public IList<Vec3> Beacons => m_beacons;
        List<Vec3> m_beacons;

        public IEnumerable<double> Distances => m_distances;
        List<double> m_distances;

        public Vec3Rotation Rotation { get; set; } = Vec3Rotation.None;
        public Vec3 Translation { get; set; } = Vec3.Zero;

        public Scanner(int id)
        {
            m_id = id;
            m_beacons= new List<Vec3>();
        }

        public void CalculateDistances()
        {
            m_distances = new List<double>();
            foreach(var a in m_beacons)
            {
                foreach(var b in m_beacons)
                {
                    if (a == b) continue;

                    m_distances.Add(Vec3.Distance(a, b));
                }
            }
        }

    }

    internal class Day19 : DayLineLoaderBasic
    {
        List<Scanner> m_scanners = new();

        int m_nextId = 0;
        Scanner m_currScanner;

        public override void ParseInputLine(string line, int lineNum)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if(line.StartsWith("---"))
            {
                if(m_currScanner != null)
                {
                    m_currScanner.CalculateDistances();
                    m_scanners.Add(m_currScanner);
                }
                m_currScanner = new Scanner(m_nextId++);
            }
            else
            {
                m_currScanner.Beacons.Add(Vec3.Parse(line));
            }
        }

        public override void ParseInputLinesEnd(StreamReader sr)
        {
            base.ParseInputLinesEnd(sr);

            m_currScanner.CalculateDistances();
            m_scanners.Add(m_currScanner);
        }

        static Vec3Rotation[] Transforms =
        {
            Vec3Rotation.None,
            Vec3Rotation.None * Vec3Rotation.X270,
            Vec3Rotation.None * Vec3Rotation.X180,
            Vec3Rotation.None * Vec3Rotation.X90,
            Vec3Rotation.Z90,
            Vec3Rotation.Z90 * Vec3Rotation.Y270,
            Vec3Rotation.Z90 * Vec3Rotation.Y180,
            Vec3Rotation.Z90 * Vec3Rotation.Y90,
            Vec3Rotation.Z180,
            Vec3Rotation.Z180 * Vec3Rotation.X270,
            Vec3Rotation.Z180 * Vec3Rotation.X180,
            Vec3Rotation.Z180 * Vec3Rotation.X90,
            Vec3Rotation.Z270,
            Vec3Rotation.Z270 * Vec3Rotation.Y270,
            Vec3Rotation.Z270 * Vec3Rotation.Y180,
            Vec3Rotation.Z270 * Vec3Rotation.Y90,
            Vec3Rotation.Y90,
            Vec3Rotation.Y90 * Vec3Rotation.Z90,
            Vec3Rotation.Y90 * Vec3Rotation.Z180,
            Vec3Rotation.Y90 * Vec3Rotation.Z270,
            Vec3Rotation.Y270,
            Vec3Rotation.Y270 * Vec3Rotation.Z90,
            Vec3Rotation.Y270 * Vec3Rotation.Z180,
            Vec3Rotation.Y270 * Vec3Rotation.Z270,
        };

        Random m_rand= new Random();

        public (bool, Vec3Rotation, Vec3) TryMatch(Scanner a, int i, Scanner b, int j)
        {
            Vec3 beacon1 = a.Beacons[i];
            Vec3 beacon2 = b.Beacons[j];
            //Console.WriteLine($"  Testing scanner {a.Id} beacon {i} {beacon1} and scanner {b.Id} beacon {j} {beacon2}...");

            foreach (var rot in Transforms)
            {
                //Console.WriteLine($"   Testing rotation {rot}...");
                Vec3 beacon2Rot = beacon2 * rot;
                //Console.WriteLine($"\tBeacon is at {beacon2}, rotated to {beacon2Rot}");

                Vec3 trans = beacon1 - beacon2Rot;
                //Console.WriteLine($"\tTranslation would be {trans}");

                int match = 0;
                foreach (Vec3 testBeacon in b.Beacons)
                {
                    var test = testBeacon * rot;
                    test = test + trans;
                    if (a.Beacons.Contains(test))
                        match++;
                }

                //Console.WriteLine($"\tFound {match} matching beacons.");
                if (match >= 12)
                    return (true, rot, trans);
            }

            return (false, Vec3Rotation.None, Vec3.Zero);
        }

        public (bool, Vec3Rotation, Vec3) TryMatch(Scanner a, Scanner b)
        {
            Console.WriteLine($"Testing scanner {a.Id} and {b.Id}...");
            for(int i=0; i < a.Beacons.Count; i++)
            {
                for(int j=0; j < b.Beacons.Count; j++)
                {
                    (var success, var rot, var trans) = TryMatch(a, i, b, j);
                    if(success)
                    {
                        return (success, rot, trans);
                    }
                }
            }

            return (false, Vec3Rotation.None, Vec3.Zero);
        }

        public override string Part1()
        {
            List<Scanner> matched = new();
            List<Scanner> waiting = new(m_scanners);

            // Start with the first scanner in the list
            matched.Add(waiting.First());
            waiting.RemoveAt(0);

            long count = 0;
            while(waiting.Count > 0)
            {
                int a = m_rand.Next(matched.Count);
                int b = m_rand.Next(waiting.Count);

                var scannerA = matched[a];
                var scannerB = waiting[b];
                    
                var overlap = scannerA.Distances.Intersect(scannerB.Distances);
                if(overlap.Count() >= 66)
                {
                    (var success, var rot, var trans) = TryMatch(scannerA, scannerB);
                    if(success)
                    {
                        Console.WriteLine($"Scanner {scannerA.Id} matched scanner {scannerB.Id} at {trans}");

                        // Rotate and translate all points to the reference frame we matched
                        List<Vec3> points = scannerB.Beacons.ToList();
                        scannerB.Beacons.Clear();
                        foreach(var point in points )
                        {
                            scannerB.Beacons.Add(point * rot + trans);
                        }
                        scannerB.Rotation = rot;
                        scannerB.Translation = trans;
                        matched.Add(scannerB);
                        waiting.Remove(scannerB);
                    }
                }

                count++;
            }

            Console.WriteLine("All scanners matched!");

            m_allBeacons = new HashSet<Vec3>();
            foreach( Scanner scanner in matched )
            {
                foreach(Vec3 beacon in scanner.Beacons)
                {
                    var transformed = beacon * scanner.Rotation + scanner.Translation;
                    if(!m_allBeacons.Contains(transformed))
                    {
                        m_allBeacons.Add(transformed);
                        Console.WriteLine(transformed);
                    }
                }
            }

            return $"{m_allBeacons.Count}";
        }

        HashSet<Vec3> m_allBeacons;

        public override string Part2()
        {
            int largest = 0;

            foreach(Scanner a in m_scanners)
            {
                foreach(Scanner b in m_scanners)
                {
                    largest = Math.Max(largest, Vec3.ManhattanDistance(a.Translation, b.Translation));
                }
            }


            return $"{largest}";
        }
    }
}
