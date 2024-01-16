using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task24
    {
        [Test]
        [TestCase(
            @"19, 13, 30 @ -2,  1, -2
18, 19, 22 @ -1, -1, -2
20, 25, 34 @ -2, -2, -4
12, 31, 28 @ -1, -2, -1
20, 19, 15 @  1, -5, -3", 7, 27, 2)]
        [TestCase(@"Task24.txt", 200000000000000L, 400000000000000L, 15593)]
        public void Task(string input, long min, long max, int expected)
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

            var result = 0;
            for (var i = 0; i < lines.Count; i++)
            for (var j = i + 1; j < lines.Count; j++)
            {
                if (i == j) continue;

                var one = lines[i];
                var other = lines[j];

                var intersectPoint = GetIntersectionV(one, other);
                var intersectPoint2 = GetIntersectionV(other, one);
                if (!intersectPoint.HasValue || !intersectPoint2.HasValue) continue;

                //if (intersectPoint2 != intersectPoint) throw new NotImplementedException(); precision:(
                
                if (intersectPoint.Value.X < min || intersectPoint.Value.X > max
                                                 || intersectPoint.Value.Y < min || intersectPoint.Value.Y > max)
                    continue;
                
                result++;
                //if (CheckInside(intersectPoint.Value, min, max, one, other)) result++;
            }

            result.Should().Be(expected);
        }

        // private bool CheckInside(Vector2 point, long min, long max, Line one, Line other)
        // {
        //     if (min > max) return false;
        //
        //     var bools = new[]
        //     {
        //         CheckInside(point, one, min, max, tuple => tuple.X, line => line.Point.X, line => line.Velocity.X),
        //         CheckInside(point, one, min, max, tuple => tuple.Y, line => line.Point.Y, line => line.Velocity.Y),
        //
        //         CheckInside(point, other, min, max, tuple => tuple.X, line => line.Point.X, line => line.Velocity.X),
        //         CheckInside(point, other, min, max, tuple => tuple.Y, line => line.Point.Y, line => line.Velocity.Y),
        //     };
        //     return bools.All(x => x);
        // }

        // private bool CheckInside(Vector2 point, Line line, long min, long max,
        //     Func<Vector2, float> getDimension, Func<Line, float> getStart, Func<Line, float> getVelocity)
        // {
        //     var x = getDimension(point);
        //     if (x < min || x > max) return false;
        //
        //     if (
        //         getVelocity(line) > 0 && x < getStart(line)
        //         || getVelocity(line) < 0 && x > getStart(line)
        //     ) return false;
        //
        //     return true;
        // }

        // private (float X, float Y)? GetIntersection(Line one, Line other)
        // {
        //     //ax + by + c1 = 0 one
        //     //dx + ey + c2 = 0 other
        //
        //     // D = ae — bd
        //     var d = one.A * other.B - one.B * other.A;
        //
        //     //Dx = c1e — c2b
        //     var dx = one.C * other.B - other.C * one.B;
        //
        //     //Dy = ac2 — c1d
        //     var dy = one.A * other.C - one.C * other.A;
        //
        //     if (d == 0) return null;
        //
        //     return (-dx / d, -dy / d);
        // }

        private Vector2? GetIntersectionV(Line one, Line other)
        {
            var v2n = new Vector2(-other.Velocity.Y, other.Velocity.X);
            var t1 = (Vector2.Dot(other.Point, v2n) - (Vector2.Dot(one.Point, v2n))) / Vector2.Dot(one.Velocity, v2n);

            if (t1 < 0 || double.IsNegativeInfinity(t1)
                       || double.IsPositiveInfinity(t1)) return null;

            var v1 = one.Point + t1 * one.Velocity;
            return v1;
        }

        struct Line
        {
            public Vector2 Point { get; set; }
            public Vector2 Velocity { get; set; }

            // public float X1 => Point.X;
            // public float Y1 => Point.Y;
            //
            // public float X2 => Point.X + Velocity.X;
            // public float Y2 => Point.Y + Velocity.Y;

            //y — y1 = (y2 — y1) * (x — x1)
            // (x2 — x1) * y  - (x2 — x1) * y1 = (y2 — y1) * (x — x1)
            // (x2—x1)*y - (y2-y1)*x + (y2-y1)*x1-(x2—x1)*y1 = 0
            //  a      x    b      y   c  
            // (y2-y1)*x - (x2—x1)*y - (y2-y1)*x1+(x2—x1)*y1 = 0
            // public float A => Y2 - Y1;
            // public float B => -(X2 - X1);
            // public float C => -((Y2 - Y1) * X1) + (X2 - X1) * Y1;
        }
    }
}