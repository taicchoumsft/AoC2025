// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;


// Default input file is "input.txt" in project root
string inputFile = args.Length > 0 ? args[0] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

// Example Part 1 solution stub
static object Part1(string[] lines)
{
    // Use LINQ, parsing helpers, etc.
    var numbers = lines.Select(int.Parse).ToList();
    return numbers.Sum();
}

// Example Part 2 solution stub
static object Part2(string[] lines)
{
    // Replace with your logic
    return lines.Length;
}

// 🔧 Utility helpers
static IEnumerable<int> ParseInts(IEnumerable<string> lines) =>
    lines.Select(int.Parse);

static IEnumerable<string[]> SplitLines(IEnumerable<string> lines, char sep = ' ') =>
    lines.Select(line => line.Split(sep, StringSplitOptions.RemoveEmptyEntries));

static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> source, int size)
{
    var chunk = new List<T>(size);
    foreach (var item in source)
    {
        chunk.Add(item);
        if (chunk.Count == size)
        {
            yield return chunk;
            chunk = new List<T>(size);
        }
    }
    if (chunk.Count > 0)
        yield return chunk;
}

