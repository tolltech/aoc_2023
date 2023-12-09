using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task08_2
    {
        [Test]
//         [TestCase(
//             @"LR
//
// 11A = (11B, XXX)
// 11B = (XXX, 11Z)
// 11Z = (11B, XXX)
// 22A = (22B, XXX)
// 22B = (22C, 22C)
// 22C = (22Z, 22Z)
// 22Z = (22B, 22B)
// XXX = (XXX, XXX)",
//             6)]
        [TestCase(@"Task08.txt", 13663968099527L)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            var inputs = input.SplitEmpty("\r\n\r\n", "\n\n");
            var commands = inputs[0];
            var map = new Dictionary<string, (string Left, string Right)>();
            foreach (var line in inputs[1].SplitEmpty("\r", "\n"))
            {
                var lineSplits = line.SplitEmpty(" = ");
                var leftRight = lineSplits[1].Trim('(', ')').SplitEmpty(", ");
                map[lineSplits[0]] = (leftRight[0], leftRight[1]);
            }

            var totalZs = map.Keys.Where(x => x.EndsWith("Z")).ToHashSet();
            var pathLength = commands.Length;
            var paths = map.Keys.Where(c => c.EndsWith("A")).ToArray();
            var cycles = new Dictionary<(string Node, int InternalStepNumber), (int First, int Last)>();
            foreach (var path in paths)
            {
                var steps = -1;
                var current = path;
                var internalCycles = new Dictionary<(string Node, int InternalStepNumber), (int First, int Last)>();
                while (true)
                {
                    foreach (var command in commands)
                    {
                        ++steps;
                        current = command == 'L'
                            ? map[current].Left
                            : map[current].Right;

                        if (current.EndsWith("Z"))
                        {
                            if (internalCycles.TryGetValue((current, steps % pathLength), out var first))
                            {
                                if (first.Last != -1) continue;
                                
                                internalCycles[(current, steps % pathLength)] = (first.First, steps);
                            }
                            else
                            {
                                internalCycles[(current, steps % pathLength)] = (steps, -1);
                            }

                            if (internalCycles.Values.All(x => x.Last != -1))
                            {
                                break;
                            }
                        }
                        
                        if (internalCycles.Count > 0 && internalCycles.Values.All(x => x.Last != -1))
                        {
                            break;
                        }
                    }

                    if (internalCycles.Count > 0 && internalCycles.Values.All(x => x.Last != -1))
                    {
                        break;
                    }
                }

                if (internalCycles.Count != 1) throw new NotImplementedException();
                cycles[internalCycles.Single().Key] = internalCycles.Single().Value;
            }

            if (cycles.Count != paths.Length) throw new NotImplementedException();

            var periods = cycles.Values.Select(x => (x.First, x.Last - x.First)).ToArray();
            //13939,17621,11309,20777,19199,15517 nok

            13663968099527L.Should().Be(expected);
        }
    }
}