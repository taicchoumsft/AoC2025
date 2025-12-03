// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

// Standard t-nt DP
// let f(i, t) = max joltage up to index i, with t elements remaining
// then f(i, t) = max(j[i] + 10 * f(i - 1, t - 1), f(i - 1, t))
static object Part1(string[] lines) => lines.Select(l => Solve(l, 2)).Sum();

static object Part2(string[] lines) => lines.Select(l => Solve(l, 12)).Sum();

// 1D dp - remove idx dimension
static long Solve(ReadOnlySpan<char> line, int take)
{
    long[] dp = new long[take + 1];

    foreach (var ch in line)
    {
        for (int rem = take; rem >= 1; --rem)
        {
            dp[rem] = Math.Max(ch - '0' + 10 * dp[rem - 1], dp[rem]);
        }
    }
    return dp[take];
}

// 2D dp non space optimal
// static long Solve(ReadOnlySpan<char> line, int take)
// {
//     long[,] dp = new long[take + 1, line.Length + 1];
//     for (int rem = 1; rem <= take; ++rem)
//     {
//         for (int idx = 1; idx <= line.Length; ++idx)
//         {
//             dp[rem, idx] = Math.Max(line[idx - 1] - '0' + 10 * dp[rem - 1, idx - 1], dp[rem, idx - 1]);   
//         }
//     }
//     return dp[take, line.Length];
// }

// recursion with memo
// static long Recurse(ReadOnlySpan<char> line, int rem, int idx, long[,] memo)
// {
//     if (idx < 0 || rem == 0) return 0;
//     if (memo[rem, idx] > 0) return memo[rem, idx];
//     long skip = Recurse(line, rem, idx - 1, memo);
//     long take = (line[idx] - '0') + 10 * Recurse(line, rem - 1, idx - 1, memo);
//     return memo[rem, idx] = Math.Max(take, skip);
// }


