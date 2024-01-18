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
20, 19, 15 @  1, -5, -3", 47, @"-2 -1 -6 -1 44
-3 0 -12 1 35
-3 -1 -18 -7 38
-6 -3 -6 1 164")]
        [TestCase(@"Task24.txt", 757031940316991, @"111 42 30673372039693 -81256763489248 -55702178944306158
88 46 -40839158753897 -196862178240725 -56494097761039673
-19 -140 -42596643915115 -195718097842865 32744403721419363
648 90 152171277853722 -121902626197932 -195309352421864964")]
        public void Task(string input, long expected, string expectedK)
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
                    Point = new VectorLong(pointSplits[0], pointSplits[1]),
                    Velocity = new VectorLong(vSplits[0], vSplits[1])
                });
            }

            var result = 0L;
            var list = new List<(VectorLong Point, VectorLong Velocity, long C)>();
            for (var i = 0; i < lines.Count; i++)
            for (var j = i + 1; j < lines.Count; j++)
            {
                if (i == j) continue;

                var one = lines[i];
                var other = lines[j];

                list.Add(GetNumbers(one, other));
            }

            var k = string.Join("\r\n",
                list.Take(4).Select(x => $"{x.Point.X} {x.Point.Y} {x.Velocity.X} {x.Velocity.Y} {x.C}"));
            
            //155272940103072
            //386989974246822
            //214769025967097
            //var v = 155272940103072 + 386989974246822 + 214769025967097;//757031940316991
            k.Should().Be(expectedK);
        }

        private (VectorLong Point, VectorLong Velocity, long C) GetNumbers(Line one, Line other)
        {
            var p1 = one.Point;
            var p2 = other.Point;
            var v1 = one.Velocity;
            var v2 = other.Velocity;

            return (
                new VectorLong(v2.Y - v1.Y, v1.X - v2.X),
                new VectorLong(p1.Y - p2.Y, p2.X - p1.X),
                VectorLong.Dot(p2, GetOrt(v2)) - VectorLong.Dot(p1, GetOrt(v1))
            );
        }

        VectorLong GetOrt(VectorLong v)
        {
            return new VectorLong(-v.Y, v.X);
        }

        [DebuggerDisplay("{Point.X},{Point.Y},{Point.Z}@{Velocity.X},{Velocity.Y},{Velocity.Z}")]
        struct Line
        {
            public VectorLong Point { get; set; }
            public VectorLong Velocity { get; set; }
        }

        struct VectorLong
        {
            public VectorLong(long x, long y)
            {
                X = x;
                Y = y;
            }

            public long X { get; set; }
            public long Y { get; set; }

            public static long Dot(VectorLong left, VectorLong right)
            {
                return left.X * right.X + left.Y * right.Y;
            }
        }
    }
}