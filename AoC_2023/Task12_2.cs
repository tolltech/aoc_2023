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
        [TestCase(@"Task12.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var result = 0;

            foreach (var line in input.SplitLines())
            {
                var number = line.SplitEmpty(" ")[0].Replace(".", "0").Replace("#", "1");
                var mask = line.SplitEmpty(" ")[1].SplitEmpty(",").Select(int.Parse).ToArray();

                var cnt = number.Count(c => c == '?');
                for (var variant = 0; variant < (1 << cnt); ++variant)
                {
                    var realNumber = GetNumber(number, variant);
                    if (Check(realNumber, mask)) ++result;
                }
            }

            result.Should().Be(expected);
        }

        private bool Check(string realNumber, int[] mask)
        {
            var splits = realNumber.SplitEmpty("0");
            if (splits.Length != mask.Length) return false;
            for (var i = 0; i < splits.Length; ++i)
            {
                if (splits[i].Length != mask[i]) return false;
            }

            return true;
        }

        private string GetNumber(string number, int variant)
        {
            var charNumber = number.ToArray();
            var cnt = 0;
            for (var i = charNumber.Length - 1; i >= 0; i--)
            {
                if (charNumber[i] == '?')
                {
                    charNumber[i] = (variant & (1 << cnt)) > 0 ? '1' : '0';
                    cnt++;
                }
            }

            return new string(charNumber);
        }
    }
}