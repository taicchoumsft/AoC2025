#:package Microsoft.Z3@4.12.2

// dotnet run <file.cs> <input file>
using Microsoft.Z3;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

string inputFile = args.Length > 0 ? args[0] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

static List<(int, int[], int[])> Parse1(string[] lines)
{
    List<(int, int[], int[])> arr = [];

    foreach (var line in lines)
    {
        var fst = line.Split(" ");
        int ignition = fst[0][1..^1].Select((p, idx) => (p, idx)).Where(t => t.p == '#').Select(t => t.idx).Aggregate(0, (acc, idx) => acc |= 1 << idx);
        int[] keys = fst[1..^1].Select(str => str[1..^1].Split(",").Select(int.Parse).Aggregate(0, (acc, idx) => acc |= 1 << idx)).ToArray();
        int[] joltage = fst[^1][1..^1].Split(",").Select(int.Parse).ToArray();
        arr.Add((ignition, keys, joltage));
    }
    return arr;
}

static List<(int[][], int[])> Parse2(string[] lines)
{
    List<(int[][], int[])> arr = [];

    foreach (var line in lines)
    {
        var fst = line.Split(" ");
        int[][] keys = fst[1..^1].Select(str => str[1..^1].Split(",").Select(int.Parse).ToArray()).ToArray();
        int[] joltage = fst[^1][1..^1].Split(",").Select(int.Parse).ToArray();
        arr.Add((keys, joltage));
    }
    return arr;
}

Console.WriteLine("Part 1: " + Part1(Parse1(lines)));
Console.WriteLine("Part 2: " + Part2(Parse2(lines)));

// Standard BFS.  Encode as ints and xor the inputs since ignition input never exceeds 10 bits
static object Part1(List<(int, int[], int[])> arr)
{
    static int BFS(int ignition, int[] keys)
    {
        Queue<int> q = [];
        HashSet<int> seen = [];
        foreach (var k in keys) q.Enqueue(k);

        int depth = 1;
        while (q.Count > 0)
        {
            int sz = q.Count;
            while (sz-- > 0)
            {
                int cur = q.Dequeue();

                if (cur == ignition) return depth;

                if (seen.Contains(cur)) continue;
                seen.Add(cur);

                foreach (var k in keys) q.Enqueue(cur ^ k);
            }
            depth++;
        }
        return -1;
    }

    return arr.Sum(item => BFS(item.Item1, item.Item2));
}

// Had to look this up.  Clear that this was ILP, but couldn't solve on my own.
// Copied this answer from Grigoryan Artem: 
// https://github.com/GrigoryanArtem/advent-of-code/blob/master/Puzzles.Runner/2025/Day10.cs
// Uses Z3 solver to express the problem as an integer linear programming problem.
//
// ILP Formulation as explained by Opus 4.5:
// ----------------
// Variables: b_0, b_1, ..., b_{n-1} (one per key), representing how many times each key is used
// 
// Constraints:
//   1. b_i >= 0 for all i (non-negative usage counts)
//   2. For each requirement index j: sum of b_i where key[i] contains j == req[j]
//      (the total contributions to each joltage requirement must exactly match the target)
//
// Objective: Minimize sum(b_i) (minimize total key presses/usages)
//
// In mathematical notation:
//   minimize    Σ b_i
//   subject to  Σ b_i * A[i,j] = req[j]   for all j
//               b_i >= 0                   for all i
// where A[i,j] = 1 if key i contributes to requirement j, else 0
static object Part2(List<(int[][], int[])> arr)
{
    int SolveRow(int[][] keys, int[] req)
    {
        var ctx = new Context();
        var opt = ctx.MkOptimize();

        IntExpr[] vars = new IntExpr[keys.Length];

        for (var b = 0; b < keys.Length; b++)
        {
            vars[b] = ctx.MkIntConst($"b_{b}");
            opt.Add(ctx.MkGe(vars[b], ctx.MkInt(0)));
        }

        for (var j = 0; j < req.Length; j++)
        {
            var terms = new List<ArithExpr>();
            for (var b = 0; b < keys.Length; b++)
                if (keys[b].Contains(j))
                    terms.Add(vars[b]);

            var se = ctx.MkAdd([.. terms]);
            var te = ctx.MkInt(req[j]);

            opt.Add(ctx.MkEq(se, te));
        }

        opt.MkMinimize(ctx.MkAdd([.. vars.Cast<ArithExpr>()]));

        var status = opt.Check();
        System.Console.WriteLine(opt.Model);
        return vars.Sum(v => ((IntNum)opt.Model.Evaluate(v)).Int);
    }

    return arr.Sum(item => SolveRow(item.Item1, item.Item2));
}

