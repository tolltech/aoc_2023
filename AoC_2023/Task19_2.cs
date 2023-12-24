using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task19_2
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
            167409079868000L)]
        [TestCase(@"Task19.txt", 143219569011526L)]
        [TestCase(@"in{s<3001:a,b}
a{s>1000:A,R}
b{s>3500:R,A}", 4000 * 4000 * 4000L * (2000 + 500))]
        [TestCase(@"in{s<3001:a,b}
a{m>1000:A,R}
b{s>3500:R,A}", 4000*4000*4000L* 500 + 3000L * 3000 * 4000 * 4000L)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var splits = input.SplitEmpty("\n\n", "\r\n\r\n");
            var commands = new Dictionary<string, (string P, string Op, int Val, bool End, string Target)[]>();
            foreach (var line in splits[0].SplitLines())
            {
                var command = new string(line.TakeWhile(c => c != '{').ToArray());
                var funcsStr = new string(line.SkipWhile(c => c != '{').ToArray()).Trim('{', '}').SplitEmpty(",");

                var funcs = new List<(string P, string Op, int Val, bool End, string Target)>();
                foreach (var funcStr in funcsStr)
                {
                    var fSplit = funcStr.SplitEmpty(":");
                    var target = fSplit.Last();

                    if (fSplit.Length == 1)
                    {
                        funcs.Add((string.Empty, string.Empty, 0, true, target));
                        continue;
                    }

                    var p = fSplit[0].First().ToString();
                    var op = fSplit[0][1].ToString();
                    var val = int.Parse(new string(fSplit[0].Where(char.IsDigit).ToArray()));

                    funcs.Add((p, op, val, false, target));
                }

                commands[command] = funcs.ToArray();
            }

            var ranges = new Dictionary<string, (int Min, int Max)>()
            {
                { "x", (1, 4000) },
                { "m", (1, 4000) },
                { "a", (1, 4000) },
                { "s", (1, 4000) },
            };
            var result = Dfs(commands, "in", ranges);

            result.Should().Be(expected);
        }

        private long Dfs(Dictionary<string, (string P, string Op, int Val, bool End, string Target)[]> commands,
            string cmd, Dictionary<string, (int Min, int Max)> ranges)
        {
            if (ranges.Any(x => x.Value.Min > x.Value.Max)) return 0;
            if (cmd == "R") return 0;
            if (cmd == "A")
            {
                var r = 1L;
                foreach (var range in ranges)
                {
                    r *= range.Value.Max - range.Value.Min + 1;
                }

                return r;
            }

            var nextCmds = commands[cmd];
            var result = 0L;
            
            var newFalseRanges = ranges.ToDictionary(x => x.Key, x => x.Value);

            // px{a<2006:qkq,m>2090:A,rfg}
            // pv{a>1716:R,A}
            // lnx{m>1548:A,A}
            // rfg{s<537:gd,x>2440:R,A}
            foreach (var nextCmd in nextCmds)
            {
                if (nextCmd.End)
                {
                    result += Dfs(commands, nextCmd.Target, newFalseRanges);
                    break;
                }

                var oldMin = newFalseRanges[nextCmd.P].Min;
                var oldMax = newFalseRanges[nextCmd.P].Max;

                int trueMin, trueMax, falseMin, falseMax;

                if (nextCmd.Op == ">")
                {
                    trueMin = nextCmd.Val + 1;
                    trueMax = oldMax;
                    falseMax = nextCmd.Val;
                    falseMin = oldMin;
                }
                else
                {
                    trueMin = oldMin;
                    trueMax = nextCmd.Val - 1;
                    falseMax = oldMax;
                    falseMin = nextCmd.Val;
                }

                var newTrueRanges = newFalseRanges.ToDictionary(x => x.Key, x => x.Value);
                newTrueRanges[nextCmd.P] = (trueMin, trueMax);

                result += Dfs(commands, nextCmd.Target, newTrueRanges);

                newFalseRanges[nextCmd.P] = (falseMin, falseMax);
            }

            return result;
        }
    }
}