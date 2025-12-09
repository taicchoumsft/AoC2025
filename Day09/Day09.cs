// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


string inputFile = args.Length > 1 ? args[1] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);
var points = lines.Select(line => line.Split(',').Select(int.Parse).ToArray())
                    .Select(a => new Point(a[0], a[1])).ToArray();

Console.WriteLine("Part 1: " + Part1(points));
Console.WriteLine("Part 2: " + Part2(points));

static object Part1(Point[] points)
{
    long best = 0;
    for (int i = 0; i < points.Length; ++i)
    {
        for (int j = i + 1; j < points.Length; ++j)
        {
            best = Math.Max(best, points[i].Area(points[j]));
        }
    }
    return best;
}


static void FillPolygon(char[,] grid, int m, int n)
{
    List<(int, int)> dirs = [(1, 0), (0, 1), (-1, 0), (0, -1)];

    void Fill(int i, int j, char col = 'x')
    {   
        Queue<(int, int)> q = [];
        q.Enqueue((i, j));
        bool[,] seen = new bool[m, n];

        while (q.Count > 0)
        {
            var tmp = q.Dequeue();
            if (seen[tmp.Item1, tmp.Item2]) continue;
            seen[tmp.Item1, tmp.Item2] = true;
            
            if (grid[tmp.Item1, tmp.Item2] != '.') continue;
            grid[tmp.Item1, tmp.Item2] = col;

            foreach (var (d_i, d_j) in dirs)
            {
                int n_i = tmp.Item1 + d_i;
                int n_j = tmp.Item2 + d_j;

                if (n_i >= 0 && n_i < m && n_j >= 0 && n_j < n && !seen[n_i, n_j] && grid[n_i, n_j] == '.')
                {
                    q.Enqueue((n_i, n_j));
                }
            }
        }
    }
    // fill the corners in with some other color. These are unreachable

    for (int i=0; i<m; ++i)
    {
        Fill(i, 0, '-');
        Fill(i, n - 1, '-');
    }

     for (int j=0; j<n; ++j)
    {
        Fill(0, j, '-');
        Fill(m-1, j, '-');
    }

    for (int i=0; i<m; ++i)
    {
        for (int j=0; j<n; ++j)
        {
            if (grid[i, j] == '.') 
            // {
                Fill(i, j);
            //}
        }
    }
}

static bool InsidePoly(char[,] grid, int minI, int maxI, int minJ, int maxJ)
{
    for (int i=minI; i<=maxI; ++i)
    {
        for (int j=minJ; j<=maxJ; ++j)
        {
            if (!(grid[i, j] == 'o' || grid[i, j] == 'x')) return false; 
        }
    }    
    return true;
}

static object Part2(Point[] points)
{
    // path compress and build a smaller version of the grid
    // then bruteforce test against the smaller grid that
    // everything is inside.  Then path expand the solution again
    // the hope is that this is easier than point in polygon and 
    // can be run in time
    // once compressed we have a 247 * 247 grid to work with - doable with "brute force"

    // The better answer is to go with a Ray tracing test, but I'm opting for this conceptually easier method

    // First build map of point mappings to compressed indexes, and vice versa
    var mappingI = points.Select(p=>p.I).Order().Distinct().Select((p, i) => (p, i)).ToDictionary();
    var mappingJ = points.Select(p=>p.J).Order().Distinct().Select((p, i) => (p, i)).ToDictionary();

    var reverseI = mappingI.Select(kvp=>(kvp.Value, kvp.Key)).ToDictionary();
    var reverseJ = mappingJ.Select(kvp=>(kvp.Value, kvp.Key)).ToDictionary();


    var cPoints = points.Select(p => new Point(mappingI[p.I], mappingJ[p.J])).ToArray(); // compressed points

    // dimensions
    int m = mappingI.Values.Max() + 1;
    int n = mappingJ.Values.Max() + 1;

    // physically draw in the compressed grid for debugging
    char[,] grid = new char[m, n];
    for (int i=0; i<m; ++i)
    {
        for (int j=0; j<n; ++j)
        {
            grid[i,j] = '.';
        }
    }
    
    for (int k=0; k<cPoints.Length; ++k) 
    {
        Point curP = cPoints[k];
        Point nextP = cPoints[(k + 1) % cPoints.Length];

        if (curP.I == nextP.I)
        {
            for (int j=Math.Min(curP.J, nextP.J); j<=Math.Max(curP.J, nextP.J); ++j)
            {
                grid[curP.I, j] = 'x';
            }
        } else
        {
            for (int i=Math.Min(curP.I, nextP.I); i<=Math.Max(curP.I, nextP.I); ++i)
            {
                grid[i, curP.J] = 'x';
            }
        }

        grid[curP.I, curP.J] = 'o';
        grid[nextP.I, nextP.J] = 'o';
    }

    // Now fill the polygon in
    FillPolygon(grid, m, n);

    // debug
    // for (int i=0; i<m; ++i)
    // {
    //     for (int j=0; j<n; ++j)
    //     {
    //         System.Console.Write(grid[i, j]);
    //     }
    //     System.Console.WriteLine();
    // }

    // Finally, we test every pair of cPoints to see if all points of rect inside the compressed polygon
    // if they are, we expand the points out again
    long best = 0;
    for (int i=0; i<cPoints.Length; ++i)
    {
        for (int j=i + 1; j<cPoints.Length; ++j)
        {
            int minI = Math.Min(cPoints[i].I, cPoints[j].I);
            int maxI = Math.Max(cPoints[i].I, cPoints[j].I);

            int minJ = Math.Min(cPoints[i].J, cPoints[j].J);
            int maxJ = Math.Max(cPoints[i].J, cPoints[j].J);
            
            if (InsidePoly(grid, minI, maxI, minJ, maxJ))
            {
                var p1 = new Point(reverseI[cPoints[i].I], reverseJ[cPoints[i].J]);
                var p2 = new Point(reverseI[cPoints[j].I], reverseJ[cPoints[j].J]);
                best = Math.Max(best, p1.Area(p2));
            }
        }
    }
    return best;
}


public record Point(int I, int J)
{
    public long Area(Point other) => (long) (Math.Abs(I - other.I) + 1) * (Math.Abs(J - other.J) + 1);
}
