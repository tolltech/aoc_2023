using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task15
    {
        [Test]
        [TestCase(
            @"rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7",
            1320)]
        [TestCase(@"Task15.txt", 510273)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var commands = input.SplitEmpty(",");
            
            var result = 0L;

            foreach (var command in commands)
            {
                result += CalcHash(command);
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