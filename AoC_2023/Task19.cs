 using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task19
    {
        [Test]
        [TestCase(
            @"px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}",
            19114)]
        [TestCase(@"Task19.txt", 509597)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var splits = input.SplitEmpty("\n\n", "\r\n\r\n");
            var commands = new Dictionary<string, (Func<Item, bool> Check, string Target)[]>();
            foreach (var line in splits[0].SplitLines())
            {
                var command = new string(line.TakeWhile(c => c != '{').ToArray());
                var funcsStr = new string(line.SkipWhile(c => c != '{').ToArray()).Trim('{', '}').SplitEmpty(",");

                var funcs = new List<(Func<Item, bool> Check, string Target)>();
                foreach (var funcStr in funcsStr)
                {
                    var fSplit = funcStr.SplitEmpty(":");
                    var target = fSplit.Last();

                    if (fSplit.Length == 1)
                    {
                        bool TrueFunc(Item _) => true;
                        funcs.Add((TrueFunc, target));
                        continue;
                    }

                    var p = fSplit[0].First().ToString();
                    var op = fSplit[0][1].ToString();
                    var val = int.Parse(new string(fSplit[0].Where(char.IsDigit).ToArray()));

                    Func<int, bool> CheckInt = x => op == ">" ? x > val : x < val;

                    Func<Item, bool> Check = item =>
                    {
                        switch (p)
                        {
                            case "x": return CheckInt(item.X);  
                            case "m": return CheckInt(item.M);  
                            case "a": return CheckInt(item.A);  
                            case "s": return CheckInt(item.S);
                            default: throw new NotImplementedException();
                        }
                    };

                    funcs.Add((Check, target));
                }

                commands[command] = funcs.ToArray();
            }

            var result = 0L;
            foreach (var line in splits[1].SplitLines())
            {
                //{x=787,m=2655,a=1222,s=2876}
                var dataSplit = line.Trim('{', '}').SplitEmpty(",");
                var item = new Item
                {
                    X = int.Parse(dataSplit[0].Skip(2).ToArray()),
                    M = int.Parse(dataSplit[1].Skip(2).ToArray()),
                    A = int.Parse(dataSplit[2].Skip(2).ToArray()),
                    S = int.Parse(dataSplit[3].Skip(2).ToArray())
                };

                var currentCommand = commands["in"];
                while (true)
                {
                    var done = false;
                    foreach (var cmd in currentCommand)
                    {
                        if (cmd.Check(item))
                        {
                            if (cmd.Target == "R")
                            {
                                done = true;
                                break;
                            }
                            
                            if (cmd.Target == "A")
                            {
                                done = true;
                                result += item.Rate;
                                break;
                            }

                            currentCommand = commands[cmd.Target];
                            break;
                        }
                    }

                    if (done) break;
                }
            }

            result.Should().Be(expected);
        }

        class Item
        {
            public int X { get; set; }
            public int M { get; set; }
            public int A { get; set; }
            public int S { get; set; }

            public int Rate => X + M + A + S;
        }
    }
}