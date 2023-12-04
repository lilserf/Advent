using Advent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    internal class Day13 : DayLineLoaderBasic
    {
       
        (int, int) ExtractInt(string line, int start)
        {
            if (line[start] < '0' || line[start] > '9')
                throw new InvalidOperationException();

            int pos = start + 1;

            while(pos < line.Length)
            {
                char c = line[pos];
                if(c == ',' || c == ']')
                {
                    return (int.Parse(line.Substring(start, pos - start)), pos);
                }
                else if(c == '[')
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    pos++;
                }
            }

            throw new InvalidOperationException();
        }

        (List<Object>, int) ExtractList(string line, int start)
        {
            if (line[start] != '[')
                throw new InvalidOperationException();

            List<Object> list = new();

            int pos = start + 1;
            while(pos < line.Length)
            {
                char c = line[pos];

                if(c == '[')
                {
                    // Recursively build a list from here
                    (var subList, var end) = ExtractList(line, pos);
                    list.Add(subList);
                    pos = end;
                }
                else if(c >= '0' && c <= '9')
                {
                    // Get an int starting here
                    (var num, var end) = ExtractInt(line, pos);
                    list.Add(num);
                    pos = end;
                }
                else if (c == ']')
                {
                    return (list, pos+1);
                }
                else
                {
                    pos++;
                }
            }

            throw new InvalidOperationException();
        }

        string ListToString(List<Object> list)
        {
            string val = "[";
            List<string> elements = new();
            foreach(var obj in list)
            {
                if(obj is List<Object> sublist)
                {
                    elements.Add(ListToString(sublist));
                }
                else
                {
                    elements.Add(obj.ToString());
                }
            }
            val += string.Join(",", elements);
            val += "]";
            return val;
        }

        List<List<Object>> m_pairs = new();

        public override void ParseInputLine(string line, int lineNum)
        {
            if (!string.IsNullOrEmpty(line))
            {
                (var list, _) = ExtractList(line, 0);
                Console.WriteLine(ListToString(list));
                m_pairs.Add(list);
            }
        }

        public int CompareInts(int a, int b)
        {
            return a - b;
        }

        public int CompareLists(List<Object> a, List<Object> b)
        {
            int min = Math.Min(a.Count, b.Count);

            for(int i=0; i <min; i++)
            {
                int result = CompareObjects(a[i], b[i]);
                if (result != 0)
                    return result;
            }

            return a.Count - b.Count;
        }

        public int CompareObjects(Object a, Object b)
        {
            if(a is List<Object> aList && b is List<Object> bList)
            {
                return CompareLists(aList, bList);
            }
            else if(a is int aNum && b is int bNum)
            {
                return CompareInts(aNum, bNum);
            }
            else
            {
                if(a is int aNum2)
                {
                    var aList2 = new List<Object>();
                    aList2.Add(aNum2);
                    return CompareLists(aList2, b as List<Object>);
                }
                else if(b is int bNum2)
                {
                    var bList2 = new List<Object>();
                    bList2.Add(bNum2);
                    return CompareLists(a as List<Object>, bList2);
                }
            }

            throw new InvalidOperationException();
        }

        public override string Part1()
        {
            int sum = 0;
            for(int i=0; i < m_pairs.Count; i+=2)
            {
                int index = i / 2 + 1;
                List<Object> a = m_pairs[i];
                List<Object> b = m_pairs[i + 1];

                int result = CompareLists(a, b);
                Console.WriteLine($"Comparing {ListToString(a)} and {ListToString(b)}...");
                Console.WriteLine($"Order is { (result < 0 ? "correct" : "wrong") }");

                if(result < 0)
                {
                    sum += index;
                }
            }

            return sum.ToString();
        }



        public override string Part2()
        {
            var div2 = ExtractList("[[2]]", 0).Item1;
            var div6 = ExtractList("[[6]]", 0).Item1;

            m_pairs.Add(div2);
            m_pairs.Add(div6);

            m_pairs.Sort(CompareLists);

            foreach(var x in m_pairs)
            {
                Console.WriteLine(ListToString(x));
            }

            int i2 = m_pairs.IndexOf(div2) + 1;
            int i6 = m_pairs.IndexOf(div6) + 1;

            return (i2 * i6).ToString();
        }

    }
}
