using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2021
{
    class Scanner
    {
        int m_id;

        public IList<Vec3> Beacons => m_beacons;
        List<Vec3> m_beacons;

        public IEnumerable<double> Distances => m_distances;
        List<double> m_distances;

        public Vec3Rotation Rotation => Vec3Rotation.None;
        public Vec3 Transform => Vec3.Zero;

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

        public override void ParseInputLine(string line)
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

        static Vec3Rotation[] Transforms =
        {
            Vec3Rotation.None,
            Vec3Rotation.XLeft,
            Vec3Rotation.YLeft,
            Vec3Rotation.ZLeft,
            Vec3Rotation.XRight,
            Vec3Rotation.YRight,
            Vec3Rotation.ZRight,
            Vec3Rotation.X180,
            Vec3Rotation.Y180,
            Vec3Rotation.Z180,
        };

        Random m_rand= new Random();

        public (bool, Vec3Rotation, Vec3) TryMatch(Scanner a, int i, Scanner b, int j)
        {
            Vec3 beacon1 = a.Beacons[i];
            Vec3 beacon2 = a.Beacons[j];

            Vec3Rotation rot = Transforms[m_rand.Next(Transforms.Length)];
            Vec3 beacon2Rot = beacon2 * rot;

            Vec3 trans = beacon1 - beacon2;

            int match = 0;
            foreach(Vec3 testBeacon in b.Beacons)
            {
                var test = testBeacon * rot;
                test = test + trans;
                if (a.Beacons.Count(x => x.Equals(test)) > 0)
                    match++;
            }

            if (match >= 12)
                return (true, rot, trans);
            else
                return (false, Vec3Rotation.None, Vec3.Zero);
        }

        public (bool, Vec3Rotation, Vec3) TryMatch(Scanner a, Scanner b)
        {
            int i = m_rand.Next(a.Beacons.Count);
            int j = m_rand.Next(b.Beacons.Count);
            return TryMatch(a, i, b, j);
        }

        public override string Part1()
        {
            List<Scanner> matched = new();
            List<Scanner> waiting = new(m_scanners);

            // Start with the first scanner in the list
            matched.Add(waiting.First());
            waiting.RemoveAt(0);

            while(true)
            {
                int a = m_rand.Next(matched.Count);
                int b = m_rand.Next(waiting.Count);

                var scannerA = matched[a];
                var scannerB = waiting[b];
                    
                var overlap = scannerA.Distances.Intersect(scannerB.Distances);
                if(overlap.Count() >= 66)
                {
                    (var success, var rot, var trans) = TryMatch(scannerA, scannerB);
                }
            }
        }

        public override string Part2()
        {
            throw new NotImplementedException();
        }
    }
}
