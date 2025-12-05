// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

string inputFile = args.Length > 1 ? args[1] : "input.txt";

static (long[][], long[]) Parse(string inputFile)
{
    IList<long[]> range = [];
    IList<long> ids = [];
    foreach (var line in File.ReadLines(inputFile))
    {
        if (line.Contains('-'))
        {
            range.Add([.. line.Split("-").Select(long.Parse)]);
        }
        else if (long.TryParse(line, out var id))
        {
            ids.Add(id);
        }
    }
    return ([.. range], [.. ids]);
}

var (ranges, ids) = Parse(inputFile);

Console.WriteLine("Part 1: " + Part1(ranges, ids));
Console.WriteLine("Part 2: " + Part2(ranges));

static object Part1(long[][] ranges, long[] ids) => 
    ids.Where(id => ranges.Any(range => range[0] <= id && id <= range[1])).Count();

static object Part2(long[][] ranges)
{
    // merge intervals - sort and combine, standard LC level stuff
    ranges.Sort((a, b) =>
    {
        // geezus this syntax is so unwieldly compared to python or C++
        int cmp = a[0].CompareTo(b[0]);
        if (cmp != 0) return cmp;
        return a[1].CompareTo(b[1]);
    });

    List<long[]> merged = [ranges[0]];

    // extend end of last interval if there's an overlap, otherwise create new interval
    for (int i = 1; i < ranges.Length; ++i)
    {
        long begin = ranges[i][0], end = ranges[i][1];
        ref long prev = ref merged[^1][^1];
        if (begin <= prev) prev = Math.Max(prev, end);
        else merged.Add(ranges[i]);
    }

    return merged.Select(rng => rng[1] - rng[0] + 1).Sum();
}

