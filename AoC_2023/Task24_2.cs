using System.Collections;
using System.Diagnostics;
using System.Numerics;
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
                    Point = new Vector3(pointSplits[0], pointSplits[1], pointSplits[2]),
                    Velocity = new Vector3(vSplits[0], vSplits[1], vSplits[2])
                });
            }

            var result = 0L;
            for (var i = 0; i < lines.Count; i++)
            for (var j = i + 1; j < lines.Count; j++)
            {
                if (i == j) continue;

                var one = lines[i];
                var other = lines[j];

                if (IsSamePlate(one, other))
                {
                    result++;
                }
            }

            result.Should().Be(expected);
        }
        
        private bool IsSamePlate(Line one, Line other)
        {
            var m1m2 = other.Point - one.Point;

            var m = Vector3.Dot(one.Point, (other.Point * m1m2));
            return m == 0;
        }

        [Test]
        public void TestT()
        {
            var p1 = new Vector3(-2, 4, 6);
            var m1 = new Vector3(-4, -5, 6);

            var p2 = new Vector3(1, -2, 3);
            var m2 = new Vector3(0, 1, -3);

            var m1m2 = m2 - m1;
            var m = Vector3.Dot(p1, Vector3.Cross(p2, m1m2));

            var ss = Vector3.Cross(p2, m1m2);
            var a = p2;
            var b = m1m2;
            var sss = new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
            m.Should().Be(0);
        }
        
        [Test]
        public void TestT2()
        {
            var m1 = new Vector3(0, 1, 4);
            var p1 = new Vector3(1, 2, 3);

            var m2 = new Vector3(3, 6, 11);
            var p2 = new Vector3(2, 3, 4);

            var m1m2 = m2 - m1;
            var m = Vector3.Dot(p1, Vector3.Cross(p2, m1m2));
            
            m.Should().Be(0);
        }


        [DebuggerDisplay("{Point.X},{Point.Y},{Point.Z}@{Velocity.X},{Velocity.Y},{Velocity.Z}")]
        struct Line
        {
            public Vector3 Point { get; set; }
            public Vector3 Velocity { get; set; }

            public float X1 => Point.X;
            public float Y1 => Point.Y;

            public float X2 => Point.X + Velocity.X;
            public float Y2 => Point.Y + Velocity.Y;

            //y — y1 = (y2 — y1) * (x — x1)
            // (x2 — x1) * y  - (x2 — x1) * y1 = (y2 — y1) * (x — x1)
            // (x2—x1)*y - (y2-y1)*x + (y2-y1)*x1-(x2—x1)*y1 = 0
            //  a      x    b      y   c  
            // (y2-y1)*x - (x2—x1)*y - (y2-y1)*x1+(x2—x1)*y1 = 0
            public float A => Y2 - Y1;
            public float B => -(X2 - X1);
            public float C => -((Y2 - Y1) * X1) + (X2 - X1) * Y1;
        }
    }
}