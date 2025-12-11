// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

static Dictionary<string, List<string>> ParseGraph(string[] lines)
{
    Dictionary<string, List<string>> adjList = [];

    foreach (var line in lines)
    {
        var tmp = line.Split(":", 2);
        var key = tmp[0];
        adjList[key] = tmp[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
    return adjList;
}

var adjList = ParseGraph(lines);

Console.WriteLine("Part 1: " + Part1(adjList));
Console.WriteLine("Part 2: " + Part2(adjList));

static object Part1(Dictionary<string, List<string>> adjList)
{
    // basic DFS, from question it is assumed DAG so no need for seen lists or loop checking
    int Dfs(string node)
    {
        if (node == "out") return 1;

        int total = 0;

        if (adjList.TryGetValue(node, out var nbrList))
        {
            foreach (var nbr in nbrList)
            {
                total += Dfs(nbr);
            }
        }
        return total;
    }

    return Dfs("you");
}

static object Part2(Dictionary<string, List<string>> adjList)
{
    // DFS with DP probably, minor state management to track if we passed fft and dac nodes
    // will use bitmask for this - fft = 0, dac = 1
    Dictionary<(string, int), long> memo = [];

    long Dfs(string node, int bm)
    {
        if (node == "out") return (bm == 3) ? 1 : 0;

        if (memo.ContainsKey((node, bm))) return memo[(node, bm)];

        long total = 0;

        if (adjList.TryGetValue(node, out var nbrList))
        {
            foreach (var nbr in nbrList)
            {
                int newBm = nbr switch
                {
                    "fft" => bm | 1,
                    "dac" => bm | 2,
                    _ => bm
                };
                total += Dfs(nbr, newBm);
            }
        }

        return memo[(node, bm)] = total;
    }

    return Dfs("svr", 0);
}
