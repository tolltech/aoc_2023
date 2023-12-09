using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task09
    {
        [Test]
        [TestCase(
            @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45",
            114)]        
        [TestCase(@"Task09.txt", 0)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            
            var result = 0;

            foreach (var line in input.SplitLines())
            {
                var rows = new List<List<int>>();
                rows.Add(new List<int>(line.SplitEmpty(" ").Select(int.Parse)));
                
                while (true)
                {
                    var newRow = rows.Last().Aggregate();
                }
            } 
            
            result.Should().Be(expected);
        }
        
    }
}