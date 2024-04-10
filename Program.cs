using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


public class Day5
{
    public static void Main(string[] args)
    {
        var input = File.ReadLines("data/day5example.txt");        
        // var input = File.ReadLines("data/day5input.txt");

        Enumerable.Range(1, 10).Chunk(2).ToList().ForEach(x => Console.WriteLine(x[0]));

        var result = Process(input);

        Console.WriteLine(result.Part1); // 174137457
        Console.WriteLine(result.Part2); // 1493866
    }

    private static readonly Regex number = new Regex(@"(\d+)");

    static List<long> GetNumbersFromLine(string line)
    {
        return number.Matches(line)
            .Select(match => long.Parse(match.Groups[1].Value))
            .ToList();
    }

    static List<long> MakeSeq(IEnumerable<string> input)
    {   
        var stream = input.GetEnumerator();
        stream.MoveNext();
        var result = GetNumbersFromLine(stream.Current);
        stream.MoveNext(); // skip blank line
        return result;
    }

    record Triple(long Base, long Start, long Length) {}

    static (bool, Func<long, long>) MakeMap(IEnumerable<string> input)
    {

        var stream = input.GetEnumerator();
        stream.MoveNext();
        Console.WriteLine(stream.Current);

        var ranges = new List<Triple>();
        string line;
        bool done = false;

        while (done = stream.MoveNext()) {
            line = stream.Current;
            if (string.IsNullOrEmpty(line)) break;
            var nums = GetNumbersFromLine(line);
            Console.WriteLine(string.Join(" ", nums));
            ranges.Add(new Triple(nums[0], nums[1], nums[2]));
        }

        long TheMap(long i)
        {
            var range = ranges.FirstOrDefault(r => r.Start <= i && i < r.Start + r.Length);
            if (range == null) return i;
            return range.Base + i - range.Start;
        };

        return (done, TheMap);
    }

    record Result(long Part1, long Part2) {}

    static Result Process(IEnumerable<string> input)
    {
        var seeds = MakeSeq(input);

        Console.WriteLine("seeds: " + string.Join(" ", seeds));

        IEnumerable<(bool, Func<long, long>)> makeMaps() {
            while (true) {
                yield return MakeMap(input);
            }
        }

        var seedToLocation = makeMaps()
                .TakeWhile(t => t.Item1)
                .Select(t => t.Item2)
                .Aggregate((f, g) => s => g(f(s)));

        var part1 = seeds
            .Select(seedToLocation)
            .Min();
        Console.Error.WriteLine($"part1: {part1}");

        IEnumerable<long> CreateRange(long start, long end) {
            Console.WriteLine($"CreateRange {start} {end}");
            for (long i = start; i < end; i++) {
                yield return i;
            }
        }

        var part2 = seeds
            .Chunk(2)
            .Select(p =>
                CreateRange(p[0], p[0] + p[1])
                    .Select(s => seedToLocation(s))
                    .Min()
            )
            .Min();
        Console.Error.WriteLine($"part2: {part2}");

        return new Result(part1, part2);
    }
}
