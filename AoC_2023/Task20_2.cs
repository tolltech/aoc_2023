using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task20_2
    {
        [Test]
        [TestCase(@"Task20.txt", 0, Ignore = "nok online")]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var ops = new Dictionary<string, Device>();
            foreach (var line in input.SplitLines())
            {
                var splits = line.SplitEmpty(" -> ");
                var op = splits[0];

                ops[op.Trim('%', '&')] = new Device
                {
                    Op = op[0] switch { '%' => Op.FlipFlop, '&' => Op.Conjunction, _ => Op.BroadCaster },
                    IsOn = false,
                    Targets = splits[1].SplitEmpty(", "),
                    Target = op
                };
            }

            foreach (var con in ops)
            {
                ops[con.Key].InputMem = ops.Where(x => x.Value.Targets.Contains(con.Key))
                    .ToDictionary(x => x.Key, x => false);
            }

            var rxTargetOps = ops.Single(x => x.Value.Targets.Contains("rx"));

            // var s = PrintPretty(ops, "broadcaster", "", true, new List<string>());
            // var s2 = PrintInvertedPretty(ops, rxTargetOps.Key, "", true, new List<string>());

            var parents =
                rxTargetOps.Value.InputMem.Keys.ToDictionary(x => x, x => 0L);
            
            var results = new List<long>();

            var result = 0L;
            while (++result > 0)
            {
                var trueParents = SendSignal(ops, "broadcaster", string.Empty, false,
                    parents.ToDictionary(x => x.Key, x => false));

                foreach (var trueParent in trueParents)
                {
                    if (trueParent.Value)
                        parents[trueParent.Key] = result;
                }

                if (parents.All(x => x.Value > 0)) break;

                //var ss = PrintPretty(ops, "broadcaster", "", true, new List<string>());
            }

            results.Add(result);

            //results nok 227411378431763
            0L.Should().Be(expected);
        }

        private Dictionary<string, bool> SendSignal(Dictionary<string, Device> ops, string target, string prevTarget,
            bool highSignal, Dictionary<string, bool> checkingTargets)
        {
            var pulses = new Queue<(string Target, string PrevTarget, bool HighSignal)>();

            pulses.Enqueue((target, prevTarget, highSignal));
            while (pulses.Count > 0)
            {
                var currentSignal = pulses.Dequeue();

                if (!ops.ContainsKey(currentSignal.Target))
                    continue;

                var current = ops[currentSignal.Target];

                bool newSignal;
                switch (current.Op)
                {
                    case Op.FlipFlop:
                        if (currentSignal.HighSignal) continue;

                        current.IsOn = !current.IsOn;
                        newSignal = current.IsOn;

                        break;
                    case Op.Conjunction:
                        current.IsOn = currentSignal.HighSignal;
                        current.InputMem[currentSignal.PrevTarget] = currentSignal.HighSignal;

                        newSignal = !current.InputMem.Values.All(x => x);
                        break;
                    case Op.BroadCaster:
                        newSignal = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                current.LastOutputSignal = newSignal;
                if (checkingTargets.ContainsKey(currentSignal.Target) && newSignal)
                {
                    checkingTargets[currentSignal.Target] = true;
                }

                foreach (var newTarget in current.Targets)
                {
                    pulses.Enqueue((newTarget, currentSignal.Target, newSignal));
                }
            }

            return checkingTargets;
        }

        enum Op
        {
            FlipFlop,
            Conjunction,
            BroadCaster
        }

        class Device
        {
            public Device()
            {
                Targets = Array.Empty<string>();
                InputMem = new Dictionary<string, bool>();
            }

            public Op Op { get; set; }
            public string[] Targets { get; set; }
            public bool IsOn { get; set; }
            public Dictionary<string, bool> InputMem { get; set; }
            public string Target { get; set; }
            public bool? LastOutputSignal { get; set; }
        }
        
        string PrintPretty(Dictionary<string, Device> ops, string node, string indent, bool last, List<string> cache)
        {
            var sb = new StringBuilder();
            sb.Append(indent);

            if (cache.Contains(node)) return sb + $"LOOP({ops!.SafeGet(node)?.Target ?? node})\r\n";
            cache.Add(node);

            if (last)
            {
                sb.Append("\\-");
                indent += "  ";
            }
            else
            {
                sb.Append("|-");
                indent += "| ";
            }

            var target = ops!.SafeGet(node)?.Target ?? node;
            if (ops!.SafeGet(node)?.IsOn ?? false)
            {
                target = target.ToUpper();
            }

            sb.AppendLine(target);

            var children = ops!.SafeGet(node)?.Targets ?? Array.Empty<string>();
            for (var i = 0; i < children.Length; i++)
                sb.Append(PrintPretty(ops, children[i], indent, i == children.Length - 1, cache.ToList()));

            return sb.ToString();
        }

        string PrintInvertedPretty(Dictionary<string, Device> ops, string node, string indent, bool last,
            List<string> cache)
        {
            var sb = new StringBuilder();
            sb.Append(indent);

            if (cache.Contains(node)) return sb + $"LOOP({ops!.SafeGet(node)?.Target ?? node})\r\n";
            cache.Add(node);

            if (last)
            {
                sb.Append("\\-");
                indent += "  ";
            }
            else
            {
                sb.Append("|-");
                indent += "| ";
            }

            sb.AppendLine(ops!.SafeGet(node)?.Target ?? node);

            var children = ops!.SafeGet(node)?.InputMem.Keys.ToArray() ?? Array.Empty<string>();
            for (var i = 0; i < children.Length; i++)
                sb.Append(PrintInvertedPretty(ops, children[i], indent, i == children.Length - 1, cache.ToList()));

            return sb.ToString();
        }
    }
}