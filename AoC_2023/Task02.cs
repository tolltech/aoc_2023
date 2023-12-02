using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task02
    {
        [Test]
        [TestCase(
            @"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green",
            8)]
        [TestCase(@"Task02.txt", 3059)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;
            //only 12 red cubes, 13 green cubes, and 14 blue cubes
            foreach (var line in input.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var sets = line
                    .SplitEmpty(":")[1]
                    .Split(";")
                    .Select(set =>
                        set.SplitEmpty(",")
                            .Select(cubes =>
                            (
                                Count: int.Parse(cubes.SplitEmpty(" ")[0]),
                                Color: cubes.SplitEmpty(" ")[1]))
                            .ToDictionary(x => x.Color, x => x.Count))
                    .ToArray();
                var possible = true;
                foreach (var set in sets)
                {
                    if (set.SafeGet("red") > 12
                        || set.SafeGet("green") > 13
                        || set.SafeGet("blue") > 14)
                    {
                        possible = false;
                        break;
                    }
                }

                if (possible)
                    result += int.Parse(line.Split(":")[0].Split(" ")[1]);
            }

            result.Should().Be(expected);
        }
    }
}