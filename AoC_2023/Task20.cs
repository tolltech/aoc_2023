using System.Collections;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task20
    {
        [Test]
        [TestCase(
            @"broadcaster -> a, b, c
%a -> b
%b -> c
%c -> inv
&inv -> a",
            32000000)]
        [TestCase(
            @"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output",
            11687500)]
        [TestCase(@"Task20.txt", 0)]
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
                    Targets = splits[1].SplitEmpty(", ")
                };
            }

            foreach (var con in ops.Where(x => x.Value.Op == Op.Conjunction))
            {
                ops[con.Key].InputMem = ops.Where(x => x.Value.Targets.Contains(con.Key))
                    .ToDictionary(x => x.Key, x => false);
            }

            ops["output"] = new Device
            {
                Op = Op.Output
            };

            var highResults = 0L;
            var lowResults = 0L;
            for (var i = 0; i < 1000; ++i)
            {
                var (high, low) = SendSignal(ops, "broadcaster", string.Empty, false);

                highResults += high;
                lowResults += low;
            }

            var result = highResults * lowResults;
            result.Should().Be(expected);
        }

        private (long High, long Low) SendSignal(Dictionary<string, Device> ops, string target, string prevTarget,
            bool highSignal)
        {
            var high = 0L;
            var low = 0L;

            var pulses = new Queue<(string Target, string PrevTarget, bool HighSignal)>();

            pulses.Enqueue((target, prevTarget, highSignal));
            while (pulses.Count > 0)
            {
                var currentSignal = pulses.Dequeue();

                if (currentSignal.HighSignal) high++;
                else low++;

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
                    case Op.Output: continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var newTarget in current.Targets)
                {
                    if (ops.ContainsKey(newTarget))
                        pulses.Enqueue((newTarget, currentSignal.Target, newSignal));
                }
            }

            return (high, low);
        }

        enum Op
        {
            FlipFlop,
            Conjunction,
            BroadCaster,
            Output
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
        }
    }
}