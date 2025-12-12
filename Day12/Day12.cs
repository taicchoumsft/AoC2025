// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.IO.Compression;

string inputFile = args.Length > 0 ? args[0] : "./Day12/sample.txt";
var lines = await File.ReadAllLinesAsync(inputFile);


static char[][] Rotate(char[][] shape)
{
    int m = shape.Length, n = shape[0].Length;
    char[][] rotated = new char[n][];
    for (int k=0; k<n; ++k) rotated[k] = new char[m];

    for (int i = 0; i<m; ++i)
    {
        for (int j=0; j<n;  ++j)
        {
            rotated[j][m - 1 - i] = shape[i][j];
        }
    }

    return rotated;
}

static char[][] flipHorizontal(char[][] shape)
{
    int m = shape.Length, n = shape[0].Length;
    char[][] flipped = new char[n][];
    for (int k=0; k<n; ++k) flipped[k] = new char[m];


    for (int j=0; j<n;  ++j)
    {
        int begin = 0;
        int end = m - 1;
        while (begin <= end)
        {
            flipped[end] = shape[begin];
            flipped[begin] = shape[end];
            begin++; end--;
        }
    }

    return flipped;
}

// set of shapes and their rotations/flips, then an list of (dimension, count) tuples
static (Dictionary<int, HashSet<char[][]>>, List<(int[], int[])>) Parse(string[] lines)
{
    Dictionary<int, HashSet<char[][]>> shapes = [];

    int lineNo = 0;
    while (lineNo < 30) // both sample and input have this limit
    {
        if (lines[lineNo].EndsWith(':'))
        {
            int idx = int.Parse(lines[lineNo][..^1]);
            lineNo++;
            List<char[]> shape = [];
            while (lineNo < lines.Length && !string.IsNullOrWhiteSpace(lines[lineNo]))
            {
                shape.Add(lines[lineNo++].ToArray());
            }
            var shapeArr = shape.ToArray();
            shapes[idx] = [];
            shapes[idx].Add(shapeArr);
            shapes[idx].Add(flipHorizontal(shapeArr));

            for (int i=0; i<3; ++i)
            {
                shapeArr = Rotate(shapeArr);
                shapes[idx].Add(shapeArr);
                shapes[idx].Add(flipHorizontal(shapeArr));
            }

            lineNo++;
        } 
    }
    
    List<(int[], int[])> arr = [];
    while (lineNo < lines.Length)
    {
        if (lines[lineNo].Contains(':'))
        {
            var tmp = lines[lineNo].Split(":", 2);
            var dim = tmp[0].Split('x').Select(int.Parse).ToArray();
            var cnts = tmp[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            arr.Add((dim, cnts));
        }
        lineNo++;
    }
    
    // foreach (var shape in shapes)
    // {
    //     System.Console.WriteLine(shape.Key);
    //     foreach (var st in shape.Value) {
    //         foreach (var sh in st)
    //         {
    //             System.Console.WriteLine(sh);
    //         }
    //         System.Console.WriteLine();
    //     }
    // }
    // foreach (var (dim, a) in arr)
    // {
    //     Console.WriteLine($"{dim[0]}, {dim[1]}: {string.Join(", ", a)}");
    // }
    return (shapes, arr);
}

var (shapes, boards) = Parse(lines);

Console.WriteLine("Part 1: " + Part1(shapes, boards));
Console.WriteLine("Part 2: " + Part2(lines));

// tl_i, tl_j representing first empty point (top left) as we do a scan
// the shape can have a hole in the first coordinate - so must anchor
// it to the first # in the first row
static bool CanPlace(char[][] board, char[][] shape, int tl_i, int tl_j)
{   
    // in any orientation, every shape should have a # in first row
    int j_offset = shape[0].IndexOf('#');
    
    for (int i=0; i<shape.Length; ++i)
    {
        for (int j=0; j<shape[0].Length; ++j)
        {
            int b_i = tl_i + i;
            int b_j = tl_j - j_offset + j;
            if (b_i < 0 || b_i >= board.Length || b_j < 0 || b_j >= board[0].Length) return false;
            if (shape[i][j] == '#' && (board[b_i][b_j] != '.' && board[b_i][b_j] != '\0')) return false;
        }
    }

    return true;
}

static void Place(char[][] board, char[][] shape, char symbol, int tl_i, int tl_j)
{
    int j_offset = shape[0].IndexOf('#');
    
    for (int i=0; i<shape.Length; ++i)
    {
        for (int j=0; j<shape[0].Length; ++j)
        {
            int b_i = tl_i + i;
            int b_j = tl_j - j_offset + j;
            if (shape[i][j] == '#') board[b_i][b_j] = symbol;
        }
    }
}

static void Remove(char[][] board, char[][] shape, int tl_i, int tl_j)
{
    int j_offset = shape[0].IndexOf('#');
    
    for (int i=0; i<shape.Length; ++i)
    {
        for (int j=0; j<shape[0].Length; ++j)
        {
            int b_i = tl_i + i;
            int b_j = tl_j - j_offset + j;
            if (shape[i][j] == '#') board[b_i][b_j] = '.';
        }
    }
}

// is there any other way but to brute force backtrack?
// for all available shapes, place it down in first free location. Then add the next
// piece. Keep going until we either place all, or not. 
// should we fail, we backtrack - rotate the piece and place next free location. 
// Try again. If that fails, flip the piece,
// try again. if all fails, move to the next free position, start the process all over again.

static object Part1(Dictionary<int, HashSet<char[][]>> shapes, List<(int[], int[])> boards)
{
    Dictionary<int, int> shapeCount = []; // store number of elements per shape
    foreach (var kvp in shapes) {
        var fst = kvp.Value.First();
        
        // take first shape, count #
        int cnt = 0;
        for (int i=0; i<fst.Length; ++i)
        {
            for (int j=0; j<fst[0].Length; ++j)
            {
                if (fst[i][j] == '#') cnt++;
            }
        }
        shapeCount[kvp.Key] = cnt;
    }

    int iterations = 0;

    bool Backtrack(char[][] board, int shapeIdx, int[] remaining)
    {
        // at this point, still takes too long since I haven't figured out the pruning
        // - kill the backtracking after certain amount
        // of iterations, see if we can get a solution to work
        iterations++;
        if (iterations > 1000) return false;

        if (remaining.All(x => x == 0)) {
            // foreach (var row in board)
            // {
            //     System.Console.WriteLine(string.Join("", row));
            // }
            // System.Console.WriteLine();
            return true;
        }
        if (shapeIdx >= shapes.Count) return false;

        if (remaining[shapeIdx] == 0)
        {
            return Backtrack(board, shapeIdx + 1, remaining);
        }
        
        // Prune - check that we have enough remaining cells on the board
        // int remCells = 0;
        // for (int i=0; i<board.Length; ++i)
        // {
        //     for (int j=0; j<board[0].Length; ++j)
        //     {
        //         if (board[i][j] == '.') remCells++;
        //     }
        // }
        // int requiredCells = remaining.Select((r, idx) => r * shapeCount[idx]).Sum();
        // if (remCells < requiredCells) {
        //     System.Console.WriteLine("success prune");
        //     return false;
        // }

        for (int i=0; i<board.Length; ++i)
        {
            for (int j=0; j<board[0].Length; ++j)
            {
                foreach (var shapeArr in shapes[shapeIdx])
                {
                    if (CanPlace(board, shapeArr, i, j))
                    {
                        // place the shape on the board - actually render it to debug
                        Place(board, shapeArr, (char) (shapeIdx + 'A'), i, j);

                        remaining[shapeIdx]--;
                        if (Backtrack(board, shapeIdx, remaining)) return true;
                        remaining[shapeIdx]++;
                        // remove the shape on the board
                        Remove(board, shapeArr, i, j);
                    }
                }
            }
        }
        return false;
    }

    int total = 0;
    foreach (var (dims, reqs) in boards)
    {
        char[][] board = new char[dims[0]][];
        for (int k=0; k< dims[0]; ++k) {
            board[k] = new char[dims[1]];
        }
        for (int i=0; i<dims[0]; ++i)
        {
            for (int j=0; j<dims[1]; ++j)
            {
                board[i][j] = '.'; // all this for debugging too
            }
        }

        iterations = 0;
        if (Backtrack(board, 0, reqs)) {
            Console.WriteLine($"solved for {dims[0]} x {dims[1]}");
            total++;
        }
    }
    return total;
}

static object Part2(string[] lines)
{
    return 0;
}

