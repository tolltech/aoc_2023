using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task24_2
    {
        [Test]
        [TestCase(
            @"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3", 1)]
        [TestCase(@"Task24.txt", 1)]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var lines = new List<Line>();
            foreach (var line in input.SplitLines())
            {
                var splits = line.SplitEmpty("@");
                var pointSplits = splits[0].SplitEmpty(", ").Select(long.Parse).ToArray();
                var vSplits = splits[1].SplitEmpty(", ").Select(long.Parse).ToArray();

                lines.Add(new Line
                {
                    Point = new Vector2(pointSplits[0], pointSplits[1]),
                    Velocity = new Vector2(vSplits[0], vSplits[1])
                });
            }

            var result = 0L;
            var list = new List<(Vector2 Point, Vector2 Velocity, float C)>();
            for (var i = 0; i < lines.Count; i++)
            for (var j = i + 1; j < lines.Count; j++)
            {
                if (i == j) continue;

                var one = lines[i];
                var other = lines[j];

                list.Add(GetNumbers(one, other));
            }
            
            result.Should().Be(expected);
        }

        private (Vector2 Point, Vector2 Velocity, float C) GetNumbers(Line one, Line other)
        {
            var p1 = one.Point;
            var p2 = other.Point;
            var v1 = one.Velocity;
            var v2 = other.Velocity;

            return (
                new Vector2(v2.Y - v1.Y, v1.X - v2.X),
                new Vector2(p1.Y - p2.Y, p2.X - p1.X),
                Vector2.Dot(p2, GetOrt(v2)) - Vector2.Dot(p1, GetOrt(v1))
            );
        }

        Vector2 GetOrt(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        [DebuggerDisplay("{Point.X},{Point.Y},{Point.Z}@{Velocity.X},{Velocity.Y},{Velocity.Z}")]
        struct Line
        {
            public Vector2 Point { get; set; }
            public Vector2 Velocity { get; set; }
        }
    }
}