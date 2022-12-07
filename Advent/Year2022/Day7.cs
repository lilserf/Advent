using Advent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent2020.Year2022
{
    internal class Day7 : DayInputFileStrategy
    {

        class FsFile
        {
            public FsFile(string name, long size) 
            {
                Name = name;
                Size = size;
            }
            public string Name { get; set; }
            public long Size { get; set; }
        }

        class FsDir
        {
            public FsDir(string name) 
            {
                Name = name;
            }

            public IList<FsDir> Dirs => m_dirs;
            List<FsDir> m_dirs = new();

            public IList<FsFile> Files => m_files;
            List<FsFile> m_files = new();

            public string Name { get; set; }
            public FsDir Parent { get; set; }

            private long m_size = 0;
            public long GetSize()
            {
                if (m_size == 0)
                {
                    m_size = Files.Sum(x => x.Size);
                    m_size += Dirs.Sum(x => x.GetSize());
                }

                return m_size;
            }
        }


        private FsDir m_root = new("/");
        private List<FsDir> m_flatDirs = new();

        public Day7(string inputFile) : base(inputFile)
        {
        }

        protected override void ParseInputStream(StreamReader sr)
        {
            m_flatDirs.Add(m_root);

            FsDir curr = m_root;
            var line = sr.ReadLine();

            while (true)
            {
                if (!line.StartsWith("$"))
                    throw new InvalidOperationException();

                var split = line.Split(' ');
                if (split[1] == "ls")
                {
                    line = sr.ReadLine();
                    while(!line.StartsWith("$"))
                    {
                        if(line.StartsWith("dir"))
                        {
                            var name = line.Substring(4);
                            FsDir newDir = new FsDir(name);
                            newDir.Parent = curr;
                            curr.Dirs.Add(newDir);
                            m_flatDirs.Add(newDir);
                        }
                        else
                        {
                            var fileSplit = line.Split(" ");
                            var size = long.Parse(fileSplit[0]);
                            var name = fileSplit[1];
                            FsFile newFile = new FsFile(name, size);
                            curr.Files.Add(newFile);
                        }

                        if (sr.EndOfStream)
                            return;

                        line = sr.ReadLine();
                    }
                }
                else if (split[1] == "cd")
                {
                    if (split[2] == "/")
                    {
                        curr = m_root;
                    }
                    else if (split[2] == "..")
                    {
                        curr = curr.Parent;
                    }
                    else
                    {
                        curr = curr.Dirs.Where(x => x.Name == split[2]).First();
                    }

                    if (sr.EndOfStream)
                        return;

                    line = sr.ReadLine();
                }

            }
        }

        public override string Part1()
        {
            m_root.GetSize();

            return m_flatDirs.Where(x => x.GetSize() <= 100000).Sum(x => x.GetSize()).ToString();
        }

        public override string Part2()
        {
            long total = 70000000;
            long target = 30000000;
            long currFree = total - m_root.GetSize();

            long needed = target - currFree;

            var best = m_flatDirs.Where(x => x.GetSize() > needed).Min(x => x.GetSize());

            return best.ToString();
        }
    }
}
