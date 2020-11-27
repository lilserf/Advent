Program.cs is the entry point.
Set up your IDay implementations in Program.cs's s_allDays, putting new ones at the top.
There is a simple IDay implementation which takes an IDayStrategy.
There are 2 useful, abstract IDayStategies that read input files in different ways, see comments.
If the day doesn't use an input file, just make a new IDayStrategy impl for that day; it's super simple.
Or you can make your own IDay impl instead as well, that also works fine.