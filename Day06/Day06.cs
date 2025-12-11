// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


string inputFile = args.Length > 0 ? args[0] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

static object Part1(string[] lines)
{
    string[] ops = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
    long[] arr = ops.Select(op => (op == "+") ? 0L : 1L).ToArray();

    for (int i = 0; i < lines.Length - 1; ++i)
    {
        var row = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
        for (int j = 0; j < row.Length; ++j)
        {
            if (ops[j] == "+")
                arr[j] += row[j];
            else
                arr[j] *= row[j];
        }
    }
    return arr.Sum();
}

static object Part2(string[] lines)
{
    // Use the last col as the col start marker
    List<int> colMarkers = lines[^1].Select((ch, idx) => (ch != ' ') ? idx : -1)
                                    .Where(idx => idx > -1)
                                    .ToList();

    colMarkers.Add(lines[^1].Length + 1);

    long total = 0;
    for (int k = 0; k < colMarkers.Count - 1; ++k)
    {
        int begin = colMarkers[k];
        int end = colMarkers[k + 1] - 1;
        char op = lines[^1][begin];
        long subTotal = (op == '+') ? 0 : 1;

        for (int j = begin; j < end; ++j)
        {
            long cur = 0;

            for (int i = 0; i < lines.Length - 1; ++i)
            {
                if (lines[i][j] == ' ') continue;
                cur = 10 * cur + lines[i][j] - '0';
            }

            if (op == '+')
                subTotal += cur;
            else
                subTotal *= cur;
        }

        total += subTotal;
    }

    return total;
}

