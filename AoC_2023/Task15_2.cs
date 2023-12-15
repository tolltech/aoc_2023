using System.Collections.Specialized;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task15_2
    {
        [Test]
        [TestCase(
            @"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7",
            145)]
        [TestCase(@"Task15.txt", 212449)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var commands = input.SplitEmpty(",");

            var boxes = Enumerable.Range(0, 256).Select(x => new OrderedDictionary()).ToArray();

            foreach (var command in commands)
            {
                var label = new string(command.TakeWhile(char.IsLetter).ToArray());
                var operation = command.SkipWhile(char.IsLetter).Take(1).Single();
                var val = command.Any(char.IsDigit) ? int.Parse(new string(command.Where(char.IsDigit).ToArray())) : 0;
                var hash = CalcHash(label);

                if (operation == '=') boxes[hash][label] = val;
                else boxes[hash].Remove(label);
            }

            var result = 0L;

            for (var index = 0; index < boxes.Length; index++)
            {
                var box = boxes[index];
                var values = new int[box.Count];
                box.Values.CopyTo(values, 0);

                result += values.Select((val, i) => (index + 1) * val * (i + 1)).Sum();
            }
            
            result.Should().Be(expected);
        }

        private int CalcHash(string command)
        {
            var result = 0;
            foreach (var c in Encoding.ASCII.GetBytes(command))
            {
                result += c;
                result *= 17;
                result %= 256;
            }

            return result;
        }
    }
}