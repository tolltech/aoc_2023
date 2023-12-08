using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task08
    {
        [Test]
        [TestCase(
            @"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)",
            2)]        
        [TestCase(
            @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)",
            6)]
        [TestCase(@"Task08.txt", 19199)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            var inputs = input.SplitEmpty("\r\n\r\n");
            var commands = inputs[0];
            var map = new Dictionary<string, (string Left, string Right)>();
            foreach (var line in inputs[1].SplitEmpty("\r","\n"))
            {
                var lineSplits = line.SplitEmpty(" = ");
                var leftRight = lineSplits[1].Trim('(', ')').SplitEmpty(", ");
                map[lineSplits[0]] = (leftRight[0], leftRight[1]);
            }

            var steps = 0;
            var current = "AAA";
            while (true)
            {
                foreach (var command in commands)
                {
                    current = command == 'L' 
                        ? map[current].Left 
                        : map[current].Right;

                    ++steps;
                    if (current == "ZZZ") break;
                }   
                if (current == "ZZZ") break;
            }
            
            steps.Should().Be(expected);
        }
        
    }
}