// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

static object Part1(string[] grid)
{
    int start = grid[0].IndexOf('S');
    HashSet<int> cols = [start];

    int split = 0;
    for (int i=1; i<grid.Length; ++i)
    {
        for (int j=0; j< grid[i].Length; ++j)
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
    // dp now
    // f(i, j) = count of num ways starting at f(i, j)
    // f(i, j) = f(i + 1, j) if not ^.  else f(i + 1, j - 1)) + f(i + 1, j + 1))
    // f(m, -) = 1
    int m = grid.Length, n = grid[0].Length;

    long[,] dp = new long[m + 1, n + 1];
    for (int j = 0; j < n; ++j) dp[m, j] = 1;

    for (int i = m - 1; i >= 0; --i)
    {
        for (int j=0; j<n; ++j)
        {
            if (grid[i][j] == '^')
            {
                dp[i, j] = dp[i + 1, j - 1] + dp[i + 1, j + 1];
            } else
            {
                dp[i, j] = dp[i + 1, j];
            }
        }
    }

    int col = grid[0].IndexOf('S');

    return dp[1, col];
}

