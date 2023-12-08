using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task08_2
    {
        [Test]
        [TestCase(
            @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)",
            6)]
        [TestCase(@"Task08.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            var inputs = input.SplitEmpty("\r\n\r\n");
            var commands = inputs[0];
            var map = new Dictionary<string, (string Left, string Right)>();
            foreach (var line in inputs[1].SplitEmpty("\r", "\n"))
            {
                var lineSplits = line.SplitEmpty(" = ");
                var leftRight = lineSplits[1].Trim('(', ')').SplitEmpty(", ");
                map[lineSplits[0]] = (leftRight[0], leftRight[1]);
            }

            var steps = 0;
            var paths = map.Keys.Where(c => c.EndsWith("A"));
            foreach (var path in paths)
            {
                var cycles = new Dictionary<string,(int StepCount, bool Repeated)>();
                var current = path;
                while (true)
                {
                    foreach (var command in commands)
                    {
                        current = command == 'L'
                            ? map[current].Left
                            : map[current].Right;

                        ++steps;
                        if (current.EndsWith("Z"))
                        {
                            
                        }
                    }

                    if (current == "ZZZ") break;
                }   
            }

            steps.Should().Be(expected);
        }
    }
}