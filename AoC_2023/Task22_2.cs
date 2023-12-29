using System.Collections;
using System.Diagnostics;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace AoC_2023
{
    [TestFixture]
    public class Task22_2
    {
        [Test]
        [TestCase(
            @"1,0,1~1,2,1
0,0,2~2,0,2
0,2,3~2,2,3
0,0,4~0,2,4
2,0,5~2,2,5
0,1,6~2,1,6
1,1,8~1,1,9",
            7)]
        [TestCase(@"Task22.txt", 83519, Ignore = "long")]
        public void Task(string input, long expected)
        {
            input = File.Exists(input) ? File.ReadAllText(input) : input;

            var cubes = input.SplitLines().Select((line, i) =>
                {
                    var splits = line.SplitEmpty("~");
                    var start = splits[0].SplitEmpty(",").Select(int.Parse).ToArray();
                    var end = splits[1].SplitEmpty(",").Select(int.Parse).ToArray();
                    return new Cube
                    {
                        Start = (start[0], start[1], start[2]),
                        End = (end[0], end[1], end[2]),
                        Label = Convert.ToChar('A' + i).ToString()
                    };
                })
                .OrderBy(x => x.MinZ)
                .ToArray();

            Gravitate(cubes);

            var safeCubes = GetSafeCubes(cubes);
            
            var result = 0L;
            for (var i = 0; i < cubes.Length; i++)
            {
                var cube = cubes[i];
                
                if (safeCubes.Contains(cube)) continue;
                
                var copy = cubes.Where(x => !Equals(x, cube)).ToArray();
                result += Gravitate(copy);
            }

            result.Should().Be((int)expected);
        }

        private HashSet<Cube> GetSafeCubes(Cube[] cubes)
        {
            var upCubes = cubes.ToDictionary(x => x, x => new List<Cube>());
            var downCubes = cubes.ToDictionary(x => x, x => new List<Cube>());

            foreach (var downCube in cubes)
            foreach (var upCube in cubes)
            {
                if (downCube.Equals(upCube)) continue;
                if (IsSupport(downCube, upCube))
                {
                    upCubes[upCube].Add(downCube);
                    downCubes[downCube].Add(upCube);
                }
            }

            return downCubes
                .Where(x => 
                    x.Value.All(u => upCubes[u].Count > 1 )
                )
                .Select(x => x.Key).ToHashSet();
        }

        private int Gravitate(Cube[] cubes)
        {
            var result = 0;
            for (var i = 0; i < cubes.Length; ++i)
            {
                var cube = cubes[i];
                if (cube.MinZ == 1) continue;

                var newZ = cubes.Where(x => x.MaxZ < cube.MinZ).Where(x => Intersect(x.Rectangle, cube.Rectangle))
                    .MaxOrDefault(x => x.MaxZ) + 1;

                if (newZ != cube.MinZ) result++;

                cubes[i] = cube.MoveDown(cube.MinZ - newZ);
            }

            return result;
        }

        private bool IsSupport(Cube downCube, Cube upCube)
        {
            return downCube.MaxZ == upCube.MinZ - 1
                   && Intersect(downCube.Rectangle, upCube.Rectangle);
        }

        private bool Intersect(((int X, int Y) LeftTop, (int X, int Y) RightBottom) one,
            ((int X, int Y) LeftTop, (int X, int Y) RightBottom) other)
        {
            return Intersect((one.LeftTop.X, one.RightBottom.X), (other.LeftTop.X, other.RightBottom.X))
                   && Intersect((one.RightBottom.Y, one.LeftTop.Y), (other.RightBottom.Y, other.LeftTop.Y));
        }

        private bool Intersect((int Left, int Right) one, (int Left, int Right) other)
        {
            var sorted = new[]
            {
                (Left: Math.Min(one.Left, one.Right), Right: Math.Max(one.Left, one.Right)),
                (Left: Math.Min(other.Left, other.Right), Right: Math.Max(other.Left, other.Right))
            }.OrderBy(x => x.Left).ThenBy(x => x.Right).ToArray();

            return sorted[0].Right >= sorted[1].Left;
        }

        [DebuggerDisplay("{Label}->{Start.X},{Start.Y},{Start.Z}~{End.X},{End.Y},{End.Z}")]
        struct Cube
        {
            public string Label { get; set; }
            public (int X, int Y, int Z) Start;
            public (int X, int Y, int Z) End;

            public (int X, int Y, int Z)[] Points => new[] { Start, End };

            public (int X, int Y) LeftTop =>
                Points.OrderBy(x => x.X).ThenByDescending(x => x.Y).Select(x => (x.X, x.Y)).First();

            public (int X, int Y) RightBottom =>
                Points.OrderBy(x => x.X).ThenByDescending(x => x.Y).Select(x => (x.X, x.Y)).Last();

            public ((int X, int Y) LeftTop, (int X, int Y) RightBottom) Rectangle => (LeftTop, RightBottom);

            public int MaxZ => Math.Max(Start.Z, End.Z);
            public int MinZ => Math.Min(Start.Z, End.Z);

            public Cube MoveDown(int z)
            {
                return new Cube
                {
                    Start = (Start.X, Start.Y, Start.Z - z),
                    End = (End.X, End.Y, End.Z - z),
                    Label = Label
                };
            }
        }
    }
}