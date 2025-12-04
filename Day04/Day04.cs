// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;


// Default input file is "input.txt" in project root
string inputFile = args.Length > 1 ? args[1] : "input.txt";
var grid = (await File.ReadAllLinesAsync(inputFile)).Select(row => row.ToArray()).ToArray();

Console.WriteLine("Part 1: " + Part1(grid));
Console.WriteLine("Part 2: " + Part2(grid));

static HashSet<(int, int)> RemovableThisRound(char[][] grid)
{
    int m = grid.Length;
    int n = grid[0].Length;

    HashSet<(int, int)> set = [];

    for (int i = 0; i < m; ++i)
    {
        for (int j = 0; j < n; ++j)
        {
            if (grid[i][j] == '@')
            {
                int cnt = 0;
                for (int d_i = -1; d_i <=1; ++d_i)
                {
                    for (int d_j = -1; d_j <= 1; ++d_j)
                    {
                        if (d_i == 0 && d_j == 0) continue;

                        int n_i = i + d_i;
                        int n_j = j + d_j;

                        if (n_i >= 0 && n_i < m && n_j >= 0 && n_j < n &&
                            grid[n_i][n_j] == '@')
                        {
                            cnt++;
                        }
                    }
                }

                if (cnt < 4) set.Add((i, j));
            }
        }
    }
    return set;
}

static object Part1(char[][] grid) => RemovableThisRound(grid).Count;

static object Part2(char[][] grid)
{
    var total = 0;
    HashSet<(int, int)> st;
    while ((st = RemovableThisRound(grid)).Count > 0)
    {
        foreach (var (i, j) in st)
        {
            grid[i][j] = '.';
        }
        total += st.Count;
    }
    
    return total;
}

