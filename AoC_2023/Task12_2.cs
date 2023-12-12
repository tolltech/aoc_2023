using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task12_2
    {
        [Test]
        [TestCase(
            @"???.### 1,1,3
.??..??...?##. 1,1,3
?#?#?#?#?#?#?#? 1,3,1,6
????.#...#... 4,1,1
????.######..#####. 1,6,5
?###???????? 3,2,1",
            525152)]
        [TestCase(@"???.### 1,1,3", 1)]
        [TestCase(@".??..??...?##. 1,1,3", 16384)]
        [TestCase(@"?#?#?#?#?#?#?#? 1,3,1,6", 1)]
        [TestCase(@"????.#...#... 4,1,1", 16)]
        [TestCase(@"????.######..#####. 1,6,5", 2500)]
        [TestCase(@"?###???????? 3,2,1", 506250)]
        [TestCase(@"Task12.txt", 0)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0L;

            foreach (var line in input.SplitLines())
            {
                var number = Enumerable.Range(0, 5).Select(x => line.SplitEmpty(" ")[0]).JoinToString("?");
                var mask = Enumerable.Repeat(line.SplitEmpty(" ")[1].SplitEmpty(",").Select(int.Parse).ToArray(), 5)
                    .SelectMany(x => x).ToArray();

                result += Calculate(number, mask);
            }

            result.Should().Be(expected);
        }

        private long Calculate(string number, int[] mask, int numberIndex = 0, int maskIndex = 0, int hashAcc = 0, Dictionary<(int,int,int), long>? cache = null)
        {
            cache ??= new Dictionary<(int, int, int), long>();
            var cacheKey = (numberIndex, maskIndex, maskInternalIndex: hashAcc);
            if (cache.TryGetValue(cacheKey, out var val)) return val;

            if (numberIndex == number.Length)
            {
                return maskIndex == mask.Length && hashAcc == 0
                       || maskIndex == mask.Length - 1 && hashAcc == mask.Last()
                    ? 1
                    : 0;
            }

            var count = 0L;

            var currentC = number[numberIndex];
            if (currentC == '.')
            {
                if (maskIndex < mask.Length && hashAcc == mask[maskIndex]) count += Calculate(number, mask, numberIndex + 1, maskIndex + 1, 0, cache);
                if (hashAcc == 0) count += Calculate(number, mask, numberIndex + 1, maskIndex, 0, cache);
            }

            if (currentC == '#')
            {
                count += Calculate(number, mask, numberIndex + 1, maskIndex, hashAcc + 1, cache);
            }

            if (currentC == '?')
            {
                if (maskIndex < mask.Length && hashAcc == mask[maskIndex]) count += Calculate(number, mask, numberIndex + 1, maskIndex + 1, 0, cache);
                if (hashAcc == 0) count += Calculate(number, mask, numberIndex + 1, maskIndex, 0, cache);
                
                count += Calculate(number, mask, numberIndex + 1, maskIndex, hashAcc + 1, cache);
            }
            
            cache[cacheKey] = count;
            return count;
        }
    }
}