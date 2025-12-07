// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

static object Part1(string[] grid)
{
    int start = grid[0].IndexOf('S');
    HashSet<int> cols = [start];

    int split = 0;
    for (int i = 1; i < grid.Length; ++i)
    {
        for (int j = 0; j < grid[i].Length; ++j)
        {
            if (grid[i][j] == '^' && cols.Contains(j))
            {
                cols.Add(j - 1);
                cols.Add(j + 1);
                cols.Remove(j);
                ++split;
            }
        }
    }
    return split;
}

static object Part2(string[] grid)
{
    // dp
    // f(i, j) = count of num ways starting at f(i, j)
    // f(i, j) = f(i + 1, j) if not ^.  else f(i + 1, j - 1)) + f(i + 1, j + 1))
    // f(m, -) = 1
    int m = grid.Length, n = grid[0].Length;

    long[] dp = [.. Enumerable.Repeat(1L, n + 1)];

    for (int i = m - 1; i >= 0; --i)
    {
        for (int j = 1; j <= n; ++j)
        {
            if (grid[i][j - 1] == '^')
            {
                dp[j] = dp[j - 1] + dp[j + 1];
            }
        }
    }

    return dp[grid[0].IndexOf('S') + 1];
}

