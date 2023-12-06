using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task06
    {
        [Test]
        [TestCase(
            @"Time:      7  15   30
Distance:  9  40  200",
            288)]
        [TestCase(@"Task06.txt", 220320)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var lines = input.SplitLines();
            var ts = lines[0].SplitEmpty(":")[1].SplitEmpty(" ").Select(int.Parse).ToArray();
            var ls = lines[1].SplitEmpty(":")[1].SplitEmpty(" ").Select(int.Parse).ToArray();

            var result = 1L;

            for (var i = 0; i < ts.Length; ++i)
            {
                var T = ts[i];
                var L = ls[i];

                var D = T * T - 4 * L;
                var x1 = (T + Math.Sqrt(D)) / 2;
                var x2 = (T - Math.Sqrt(D)) / 2;

                var xs = new[] { x1, x2, }
                    .Where(x => x > 0 && x < T)
                    .Concat(new[] { 0.1, T - 0.1 }) //oh shit!
                    .OrderBy(x => x)
                    .ToArray();

                var sign = 1;
                for (var j = 1; j < xs.Length; ++j)
                {
                    if (sign < 0)
                    {
                        var prev = xs[j - 1];
                        var current = xs[j];
                        
                        var currentInt = (int)Math.Floor(current);
                        var prevInt = (int)Math.Ceiling(prev);

                        if (prevInt == prev) prevInt++;
                        if (currentInt == current) currentInt--;

                        result *= currentInt - prevInt + 1;
                    }
                    
                    sign = -sign;
                }
            }
            
            result.Should().Be(expected);
        }
    }
}