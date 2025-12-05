// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

string inputFile = args.Length > 1 ? args[1] : "./Day05/sample.txt";

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

Console.WriteLine("Part 1: " + Part1(inputFile));
Console.WriteLine("Part 2: " + Part2(inputFile));

static bool DumbCheck(IEnumerable<long[]> ranges, long id)
{
    foreach (var range in ranges)
    {
        int idx = range.BinarySearch(id);
        if (idx >= 0) return true;
        else if (idx < 0)
        {
            idx = ~idx;
            if (idx == 1) return true;
        }
    }
    return false;
}

static object Part1(string inputFile)
{
    var (rng, ids) = Parse(inputFile);

    return ids.Where(id => DumbCheck(rng, id)).Count();    
}

static object Part2(string inputFile)
{
    var (rng, _) = Parse(inputFile);

    // merge intervals - sort and combine, standard LC level stuff
    rng.Sort((a, b) =>
    {
        // geezus this syntax is so unwieldly compared to python or C++
        int cmp = a[0].CompareTo(b[0]);
        if (cmp != 0) return cmp;
        return a[1].CompareTo(b[1]);
    });

    List<long[]> merged = [rng[0]];

    for (int i = 1; i < rng.Length; ++i)
    {
        long begin = rng[i][0], end = rng[i][1];
        long prev = merged[^1][^1];
        if (begin <= prev) merged[^1][^1] = Math.Max(prev, end);
        else merged.Add(rng[i]);
    }

    return merged.Select(rng => rng[1] - rng[0] + 1).Sum();
}

