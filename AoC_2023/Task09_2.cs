using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task09_2
    {
        [Test]
        [TestCase(
            @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45",
            2)]        
        [TestCase(@"Task09.txt", 928)]
        public void Task(string input, int expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;
            
            var result = 0;

            foreach (var line in input.SplitLines())
            {
                var rows = new List<List<int>>
                {
                    new(line.SplitEmpty(" ").Select(int.Parse))
                };

                while (true)
                {
                    var lastRow = rows.Last();
                    var newRow = new List<int>(lastRow.Count - 1);
                    for (var i = 1; i < lastRow.Count; ++i)
                    {
                        newRow.Add(lastRow[i] - lastRow[i - 1]);
                    }
                    
                    rows.Add(newRow);
                    if (newRow.All(x => x == 0)) break;
                }

                for (var i = rows.Count - 2; i >= 0; --i)
                {
                    var newItem = rows[i].First() - rows[i + 1].First();
                    rows[i].Insert(0, newItem);
                }

                result += rows[0].First();
            } 
            
            result.Should().Be(expected);
        }
        
    }
}