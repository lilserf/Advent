using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Year2018
{
	enum DamageType
	{
		Bludgeoning,
		Cold,
		Fire,
		Slashing,
		Radiation
	}

	class Group
	{
		static int _nextId = 0;

		public int Id { get; private set; }
		public bool Attacking { get; private set; }
		public int Count { get; private set; }
		public int HitPoints { get; private set; }
		public int AttackDamage { get; private set; }
		public DamageType AttackType { get; private set; }
		public int Initiative { get; private set; }

		public int Boost { get; set; }
		private int _originalCount;

		public int EffectivePower
		{
			get
			{
				return Count * (AttackDamage + Boost);
			}
		}

		private List<DamageType> WeakTo;
		public void AddWeakTo(DamageType type)
		{
			WeakTo.Add(type);
		}

		private List<DamageType> ImmuneTo;
		public void AddImmuneTo(DamageType type)
		{
			ImmuneTo.Add(type);
		}

		public Group Target { get; set; }
		public Group TargetedBy { get; set; }

		public Group(int count, int hp, DamageType[] immuneTo, DamageType[] weakTo, int ad, DamageType type, int init, bool attacking = true)
		{
			Id = _nextId;
			_nextId++;
			Attacking = attacking;
			Count = count;
			_originalCount = count;
			HitPoints = hp;
			AttackDamage = ad;
			AttackType = type;
			Initiative = init;
			WeakTo = new List<DamageType>();
			WeakTo.AddRange(weakTo);
			ImmuneTo = new List<DamageType>();
			ImmuneTo.AddRange(immuneTo);

			Target = null;
			TargetedBy = null;
		}

		public void Reset()
		{
			Count = _originalCount;
		}

		public int GetDamageMultiplier(DamageType dmg)
		{
			if (ImmuneTo.Contains(dmg))
			{
				return 0;
			}
			else if (WeakTo.Contains(dmg))
			{
				return 2;
			}
			else
			{
				return 1;
			}
		}

		public void PickTarget(List<Group> opponents)
		{
			opponents = opponents.Where(x => x.Count > 0).Where(x => x.TargetedBy == null).ToList();

			Group bestTarget = null;
			int bestDamage = 0;

			foreach(Group g in opponents)
			{
				int damage = g.GetDamageMultiplier(AttackType) * EffectivePower;
				if(damage > bestDamage)
				{
					bestTarget = g;
					bestDamage = damage;
				}
				else if(damage == bestDamage)
				{
					if(g.EffectivePower > (bestTarget?.EffectivePower ?? 0))
					{
						bestTarget = g;
						bestDamage = damage;
					}
					else if(g.EffectivePower == (bestTarget?.EffectivePower ?? 0))
					{
						if(g.Initiative > (bestTarget?.Initiative ?? 0))
						{
							bestTarget = g;
							bestDamage = damage;
						}
					}
				}
			}

			if (bestDamage > 0)
			{
				Target = bestTarget;
				if (bestTarget != null)
				{
					bestTarget.TargetedBy = this;
				}
			}
		}

		public int TakeDamage(int damage)
		{
			int killed = damage / HitPoints;
			Count -= killed;

			if (Count < 0)
				Count = 0;

			TargetedBy = null;
			return killed;
		}

		public void PerformAttack()
		{
			if (Target != null)
			{
				int damage = Target.GetDamageMultiplier(AttackType) * EffectivePower;

				int killed = Target.TakeDamage(damage);

				//Console.WriteLine($"! Group {Id} attacks group {Target.Id} for {damage} damage, killing {killed} units - leaving {Target.Count} units remaining.");
			}
			else
			{
				//Console.WriteLine($"! Group {Id} has no target.");
			}

			Target = null;

		}

		public override string ToString()
		{
			string desc = $"Group {Id} (attacking: {Attacking}) : [EP {EffectivePower}] {Count} units each with {HitPoints} hit points ";
			bool closeParen = false;
			if(ImmuneTo.Count > 0 || WeakTo.Count > 0)
			{
				desc += "(";
				closeParen = true;
			}

			if (ImmuneTo.Count > 0)
			{
				desc += "immune to ";
				foreach (var dmg in ImmuneTo)
					desc += $"{dmg}, ";
			}

			if(WeakTo.Count > 0)
			{
				desc += "weak to ";
				foreach (var dmg in WeakTo)
					desc += $"{dmg}, ";
			}

			if (closeParen)
			{
				desc += ")";
			}

			desc += $" with an attack that does {AttackDamage} {AttackType} damage at initiative {Initiative}";
			return desc;
		}
	}

	class Day24 : IDay
	{
		List<Group> m_defendingGroups = new List<Group>();
		List<Group> m_attackingGroups = new List<Group>();

		static bool s_test = false;

		public void Initialize()
		{
			if (s_test)
			{
				//Immune System:
				//17 units each with 5390 hit points(weak to radiation, bludgeoning) with an attack that does 4507 fire damage at initiative 2
				m_defendingGroups.Add(new Group(17, 5390, new DamageType[] { }, new DamageType[] { DamageType.Radiation, DamageType.Bludgeoning }, 4507, DamageType.Fire, 2, false));
				//989 units each with 1274 hit points(immune to fire; weak to bludgeoning, slashing) with an attack that does 25 slashing damage at initiative 3
				m_defendingGroups.Add(new Group(989, 1274, new DamageType[] { DamageType.Fire }, new DamageType[] { DamageType.Slashing, DamageType.Bludgeoning }, 25, DamageType.Slashing, 3, false));

				//Infection:
				//801 units each with 4706 hit points(weak to radiation) with an attack that does 116 bludgeoning damage at initiative 1
				m_attackingGroups.Add(new Group(801, 4706, new DamageType[] { }, new DamageType[] { DamageType.Radiation }, 116, DamageType.Bludgeoning, 1));
				//4485 units each with 2961 hit points(immune to radiation; weak to fire, cold) with an attack that does 12 slashing damage at initiative 4
				m_attackingGroups.Add(new Group(4485, 2961, new DamageType[] { DamageType.Radiation }, new DamageType[] { DamageType.Fire, DamageType.Cold }, 12, DamageType.Slashing, 4));
			}
			else
			{
				//Immune System:
				//2317 units each with 2435 hit points (weak to bludgeoning, cold; immune to fire) with an attack that does 10 cold damage at initiative 2
				m_defendingGroups.Add(new Group(2317, 2435, new DamageType[] { DamageType.Fire }, new DamageType[] { DamageType.Bludgeoning, DamageType.Cold }, 10, DamageType.Cold, 2, false));
				//967 units each with 5447 hit points (immune to cold; weak to slashing, radiation) with an attack that does 50 fire damage at initiative 3
				m_defendingGroups.Add(new Group(967, 5447, new DamageType[] { DamageType.Cold }, new DamageType[] { DamageType.Slashing, DamageType.Radiation }, 50, DamageType.Fire, 3, false));
				//851 units each with 11516 hit points with an attack that does 106 cold damage at initiative 7
				m_defendingGroups.Add(new Group(851, 11516, new DamageType[] { }, new DamageType[] { }, 106, DamageType.Cold, 7, false));
				//4857 units each with 2273 hit points with an attack that does 4 radiation damage at initiative 17
				m_defendingGroups.Add(new Group(4857, 2273, new DamageType[] { }, new DamageType[] { }, 4, DamageType.Radiation, 17, false));
				//1929 units each with 2540 hit points (immune to fire) with an attack that does 12 radiation damage at initiative 4
				m_defendingGroups.Add(new Group(1929, 2540, new DamageType[] { DamageType.Fire }, new DamageType[] { }, 12, DamageType.Radiation, 4, false));
				//4673 units each with 3705 hit points with an attack that does 5 fire damage at initiative 16
				m_defendingGroups.Add(new Group(4673, 3705, new DamageType[] { }, new DamageType[] { }, 5, DamageType.Fire, 16, false));
				//2312 units each with 6698 hit points (immune to radiation) with an attack that does 26 slashing damage at initiative 15
				m_defendingGroups.Add(new Group(2312, 6698, new DamageType[] { DamageType.Radiation }, new DamageType[] { }, 26, DamageType.Slashing, 15, false));
				//3526 units each with 3316 hit points (weak to cold) with an attack that does 9 fire damage at initiative 20
				m_defendingGroups.Add(new Group(3526, 3316, new DamageType[] { }, new DamageType[] { DamageType.Cold }, 9, DamageType.Fire, 20, false));
				//3207 units each with 2502 hit points (weak to cold, fire; immune to slashing) with an attack that does 6 radiation damage at initiative 11
				m_defendingGroups.Add(new Group(3207, 2502, new DamageType[] { DamageType.Slashing }, new DamageType[] { DamageType.Fire, DamageType.Cold }, 6, DamageType.Radiation, 11, false));
				//5213 units each with 9656 hit points (immune to radiation) with an attack that does 17 fire damage at initiative 6
				m_defendingGroups.Add(new Group(5213, 9656, new DamageType[] { DamageType.Radiation }, new DamageType[] { }, 17, DamageType.Fire, 6, false));

				//Infection:
				//8026 units each with 11443 hit points (immune to bludgeoning, slashing, fire) with an attack that does 2 cold damage at initiative 14
				m_attackingGroups.Add(new Group(8026, 11443, new DamageType[] { DamageType.Bludgeoning, DamageType.Slashing, DamageType.Fire }, new DamageType[] { }, 2, DamageType.Cold, 14));
				//4465 units each with 36617 hit points (weak to radiation) with an attack that does 16 cold damage at initiative 1
				m_attackingGroups.Add(new Group(4465, 36617, new DamageType[] { }, new DamageType[] { DamageType.Radiation }, 16, DamageType.Cold, 1));
				//378 units each with 18782 hit points (weak to radiation; immune to fire) with an attack that does 98 slashing damage at initiative 10
				m_attackingGroups.Add(new Group(378, 18782, new DamageType[] { DamageType.Fire }, new DamageType[] { DamageType.Radiation }, 98, DamageType.Slashing, 10));
				//1092 units each with 17737 hit points (immune to bludgeoning, fire; weak to slashing) with an attack that does 25 slashing damage at initiative 18
				m_attackingGroups.Add(new Group(1092, 17737, new DamageType[] { DamageType.Bludgeoning, DamageType.Fire }, new DamageType[] { DamageType.Slashing }, 25, DamageType.Slashing, 18));
				//270 units each with 19361 hit points (weak to bludgeoning, slashing) with an attack that does 104 fire damage at initiative 9
				m_attackingGroups.Add(new Group(270, 19361, new DamageType[] { }, new DamageType[] { DamageType.Bludgeoning, DamageType.Slashing }, 104, DamageType.Fire, 9));
				//2875 units each with 30650 hit points (weak to fire) with an attack that does 16 slashing damage at initiative 5
				m_attackingGroups.Add(new Group(2875, 30650, new DamageType[] { }, new DamageType[] { DamageType.Fire }, 16, DamageType.Slashing, 5));
				//1024 units each with 43191 hit points (weak to bludgeoning, cold) with an attack that does 76 cold damage at initiative 12
				m_attackingGroups.Add(new Group(1024, 43191, new DamageType[] { }, new DamageType[] { DamageType.Bludgeoning, DamageType.Cold }, 76, DamageType.Cold, 12));
				//892 units each with 10010 hit points (immune to radiation; weak to slashing) with an attack that does 15 radiation damage at initiative 19
				m_attackingGroups.Add(new Group(892, 10010, new DamageType[] { DamageType.Radiation }, new DamageType[] { DamageType.Slashing }, 15, DamageType.Radiation, 19));
				//2708 units each with 40667 hit points (immune to cold; weak to fire) with an attack that does 21 cold damage at initiative 13
				m_attackingGroups.Add(new Group(2708, 40667, new DamageType[] { DamageType.Cold }, new DamageType[] { DamageType.Fire }, 21, DamageType.Cold, 13));
				//780 units each with 44223 hit points (immune to bludgeoning, radiation, slashing) with an attack that does 104 cold damage at initiative 8
				m_attackingGroups.Add(new Group(780, 44223, new DamageType[] { DamageType.Bludgeoning, DamageType.Radiation, DamageType.Slashing }, new DamageType[] { }, 104, DamageType.Cold, 8));
			}
		}

		public void SelectTargets()
		{
			List<Group> allGroups = m_attackingGroups.ToList().Concat(m_defendingGroups.ToList()).ToList();

			allGroups.ForEach(x =>
			{
				x.Target = null;
				x.TargetedBy = null;
			});

			allGroups = allGroups.Where(x => x.Count > 0).ToList();

			allGroups.Sort(delegate (Group a, Group b)
			{
				if (a.EffectivePower == b.EffectivePower)
				{
					return b.Initiative - a.Initiative;
				}
				else
					return b.EffectivePower - a.EffectivePower;
			});

			foreach(Group g in allGroups)
			{
				if(g.Attacking)
				{
					g.PickTarget(m_defendingGroups);
				}
				else
				{
					g.PickTarget(m_attackingGroups);
				}

				//Console.WriteLine(g);
			}

			foreach(Group g in allGroups)
			{
				//Console.WriteLine($"Group {g.Id} is targeting group {g.Target?.Id}");
			}
		}

		public void Attack()
		{
			List<Group> allGroups = m_attackingGroups.ToList().Concat(m_defendingGroups.ToList()).ToList();

			allGroups = allGroups.Where(x => x.Count > 0).ToList();

			allGroups.Sort(delegate (Group a, Group b)
			{
				return b.Initiative - a.Initiative;
			});

			foreach(Group g in allGroups)
			{
				g.PerformAttack();
			}
		}

		public bool Test()
		{
			int counts = 0;

			while (m_attackingGroups.Count(x => x.Count > 0) > 0 && m_defendingGroups.Count(x => x.Count > 0) > 0)
			{
				SelectTargets();
				Attack();
				//Console.ReadKey();

				var newCounts = m_defendingGroups.Select(x => x.Count).Sum();
				newCounts += m_attackingGroups.Select(x => x.Count).Sum();
				if (newCounts == counts)
				{
					return false;
				}
				counts = newCounts;
			}
			return true;
		}

		public void PrintResults()
		{
			int total = 0;
			Console.WriteLine("Attacking Groups:");
			foreach (var g in m_attackingGroups)
			{
				if (g.Count > 0)
				{
					Console.WriteLine(g);
					//Console.WriteLine($"Group {g.Id} has {g.Count} units left");
					total += g.Count;
				}
			}
			Console.WriteLine($"Total units: {total}");

			total = 0;
			Console.WriteLine("Defending Groups:");
			foreach (var g in m_defendingGroups)
			{
				if (g.Count > 0)
				{
					Console.WriteLine(g);
					//Console.WriteLine($"Group {g.Id} has {g.Count} units left");
					total += g.Count;
				}
			}
			Console.WriteLine($"Total units: {total}");
		}

		public void Part1()
		{
			Test();

			PrintResults();
		}
		
		public bool ImmuneSystemWon()
		{
			return m_defendingGroups.Count(x => x.Count > 0) > 0;
		}

		public void Part2()
		{
			bool completed;
			do
			{
				Console.WriteLine($"### Testing Boost of {m_defendingGroups.First().Boost}\n");
				m_attackingGroups.ForEach(x => x.Reset());
				m_defendingGroups.ForEach(x => x.Reset());
				completed = Test();
				m_defendingGroups.ForEach(x => x.Boost++);
			}
			while (!completed || !ImmuneSystemWon());

			PrintResults();
		}

		public void Reset()
		{
			
		}
	}
}
