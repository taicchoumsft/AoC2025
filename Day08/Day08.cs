// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var coords = lines.Select(l => l.Split(",").Select(long.Parse).ToArray()).Select(arr => new Point(arr[0], arr[1], arr[2])).ToArray();

Console.WriteLine("Part 1: " + Part1(coords, 1000));
Console.WriteLine("Part 2: " + Part2(coords));



static object Part1(Point[] coords, int numConnections)
{
    PriorityQueue<(int, int), long> minHeap = new();

    for (int i = 0; i < coords.Length; ++i)
    {
        for (int j = i + 1; j < coords.Length; ++j)
        {
            long dist = coords[i].SquareDist(coords[j]);
            minHeap.Enqueue((i, j), dist);
        }
    }

    // from here it's straight Union Find
    UnionFind uf = new(coords.Length);

    int cnt = numConnections;
    while (minHeap.Count > 0 && cnt > 0)
    {
        (int idx_a, int idx_b) = minHeap.Dequeue();
        uf.Join(idx_a, idx_b);
        cnt--;
    }
    return uf.Top(3);
}

static object Part2(Point[] coords)
{
    PriorityQueue<(int, int), long> minHeap = new();

    for (int i = 0; i < coords.Length; ++i)
    {
        for (int j = i + 1; j < coords.Length; ++j)
        {
            long dist = coords[i].SquareDist(coords[j]);
            minHeap.Enqueue((i, j), dist);
        }
    }

    // from here it's straight Union Find
    UnionFind uf = new(coords.Length);

    while (minHeap.Count > 0)
    {
        (int idx_a, int idx_b) = minHeap.Dequeue();
        uf.Join(idx_a, idx_b);
        
        // suboptimal impl but more than fast enough
        if (uf.AllConnected())
        {
            return coords[idx_a].X * coords[idx_b].X;
        }
    }
    return -1;
}

public record Point(long X, long Y, long Z)
{
    public long SquareDist(Point o)
    {
        long total = (X - o.X) * (X - o.X);
        total += (Y - o.Y) * (Y - o.Y);
        total += (Z - o.Z) * (Z - o.Z);
        return total;
    }
}

public class UnionFind
{
    readonly int[] arr;

    public UnionFind(int n)
    {
        arr = Enumerable.Range(0, n).ToArray();
    }

    int Parent(int i)
    {
        if (arr[i] == i) return i;
        return arr[i] = Parent(arr[i]);
    }

    public bool Join(int i, int j)
    {
        int p_i = Parent(i);
        int p_j = Parent(j);

        if (p_i == p_j) return false;

        arr[p_j] = p_i;
        return true;
    }

    public bool AllConnected()
    {
        return arr.Select(Parent).ToHashSet().Count == 1;
    }

    public int Top(int num)
    {
        // return num of the biggest circuits per the question

        Dictionary<int, int> cnt = [];
        foreach (var a in arr)
        {
            int p = Parent(a);
            if (cnt.TryGetValue(p, out int val))
            {
                cnt[p] = val + 1;
            }
            else
            {
                cnt[p] = 1;
            }
        }

        PriorityQueue<int, int> pq = new();

        foreach (var kvp in cnt)
        {
            pq.Enqueue(kvp.Value, kvp.Value);
            if (pq.Count > num)
            {
                pq.Dequeue();
            }
        }

        int total = 1;
        while (pq.Count > 0)
        {
            total *= pq.Dequeue();
        }
        return total;
    }
}

