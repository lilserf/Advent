using Advent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Advent.Year2022
{
    class RegisterState
    {
        List<(int, long)> m_states = new();
        public IList<(int, long)> States => m_states;

        public RegisterState()
        {
            m_states.Add((0, 1));
        }
        public long GetValueAt(int cycle)
        {
            return m_states.Where(x => x.Item1 <= cycle).OrderBy(x => x.Item1).LastOrDefault().Item2;
        }
        
        public void AddState(int cycle, long value)
        {
            m_states.Add((cycle, value));
        }
    }

    class MachineState
    {
        public MachineState()
        {
            m_registers["X"] = new RegisterState();
        }
        public int Cycle { get; set; }
        Dictionary<string, RegisterState> m_registers = new();
        public IDictionary<string, RegisterState> Registers => m_registers;
    }


    abstract class Instruction
    {
        public abstract void Execute(ref MachineState s);
    }

    class AddX : Instruction
    {
        long m_param;
        public AddX(long param)
        {
            m_param = param;
        }

        public override void Execute(ref MachineState s)
        {
            long curr = s.Registers["X"].GetValueAt(s.Cycle);
            s.Cycle += 2;
            s.Registers["X"].AddState(s.Cycle, curr + m_param) ;
        }
    }

    class Noop : Instruction
    {
        public override void Execute(ref MachineState s)
        {
            s.Cycle++;
        }
    }


    internal class Day10 : DayLineLoaderBasic
    {
        List<Instruction> m_instructions = new();

        public override void ParseInputLine(string line)
        {
            if(line.StartsWith("noop"))
            {
                m_instructions.Add(new Noop());
            }
            else if(line.StartsWith("addx"))
            {
                var split = line.Split(' ');
                var param = long.Parse(split[1]);
                m_instructions.Add(new AddX(param));
            }
        }

        MachineState m_state = new();
        public override string Part1()
        { 

            foreach(var instruction in m_instructions)
            {
                instruction.Execute(ref m_state);
            }

            foreach(var s in m_state.Registers["X"].States)
            {
                Console.WriteLine($"Cycle {s.Item1} = {s.Item2}");
            }

            long sum = 0;
            for(int c = 20; c <= 220; c+=40)
            {
                long val = m_state.Registers["X"].GetValueAt(c-1);
                Console.WriteLine($"Value during cycle {c} is {val}");
                sum += (val * c);
            }

            return sum.ToString();
        }

        public override string Part2()
        {
            for(int row = 0; row < 6; row++)
            {
                for(int col = 0; col < 40; col++)
                {
                    int cycle = row * 40 + col + 1;

                    long xval = m_state.Registers["X"].GetValueAt(cycle - 1);
                    char print = '.';
                    if(xval - 1 <= col && col <= xval + 1)
                    {
                        print = '#';
                    }
                    Console.Write(print);
                }
                Console.WriteLine();
            }

            return "";
        }
    }
}
