// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

string inputFile = args.Length > 1 ? args[1] : "input.txt";
var line = (await File.ReadAllLinesAsync(inputFile))[0];

Console.WriteLine("Part 1: " + Part1(line));
Console.WriteLine("Part 2: " + Part2(line));

static IEnumerable<(string, string)> ParseRanges(string lines) =>
    lines.Split(",")
         .Select(ranges => ranges.Split("-"))
         .Select(arr => (arr[0], arr[1]));


static bool InvalidIDsCheck(ReadOnlySpan<char> numberStr, ReadOnlySpan<char> prefix, Predicate<int> p, int depth = 0)
{
    if (numberStr.Length == 0) return p(depth);

    if (numberStr.StartsWith(prefix))
    {
        return InvalidIDsCheck(numberStr[prefix.Length..], prefix, p, depth + 1);
    }

    return false;
}

static HashSet<long> FindInvalidIds(string line, Predicate<int> p)
{
    HashSet<long> invalidIds = [];

    foreach (var (left, right) in ParseRanges(line))
    {
        long leftNum = Int64.Parse(left);
        long rightNum = Int64.Parse(right);

        for (long k = leftNum; k <= rightNum; ++k)
        {
            string kStr = k.ToString();

            for (int i = 0; i < kStr.Length / 2; ++i)
            {
                if (InvalidIDsCheck(kStr, kStr.AsSpan(0, i + 1), p))
                {
                    invalidIds.Add(k);
                }
            }
        }
    }

    return invalidIds;
}

static object Part1(string line) => FindInvalidIds(line, (d) => d == 2).Sum();

// Although I'm thinking this is more a  digit DP problem, let's try bruteforce first
// my input only has 1,479,777 numbers to check.  Each number can be recursively checked pretty quickly, even wtih string conversions    
static object Part2(string line) => FindInvalidIds(line, (d) => d > 1).Sum();

// Not necessary at all to do thins hard way, just use recursion, input is small enough
// static object Part1(string line)
// {
//     long total = 0;
//     foreach (var (left, right) in ParseRanges(line))
//     {
//         var (l, r) = ValidRange(left, right);
//         if (l == -1 || r == -1) continue;
//         for (long k=l; k<=r; ++k)
//         {
//             long num = long.Parse($"{k}{k}");
//             total += num;
//         }
//     }
//     return total;
// }

// static (long, long) ValidRange(string left, string right)
// {
//     // halve every string. If first half <= second half of string, this string can be taken
//     // also increase odd length strings if possible to next value up or down - they cannot produce a proper half

//     int leftLen = left.Length;
//     int rightLen = right.Length;
//     //System.Console.WriteLine($"Original range: {left}-{right}");

//     long leftNum = Int64.Parse(left);
//     long rightNum = Int64.Parse(right);

//     // if odd, push left range to next higher
//     if ((leftLen & 1) == 1) {
//         leftNum = (long) Math.Pow(10, leftLen + 1);
//     } 

//     // push right range smaller
//     if ((rightLen & 1) == 1) {
//         rightNum = (long) Math.Pow(10, rightLen ) - 1;
//     } 

//     string newLN = leftNum.ToString();
//     string newRN = rightNum.ToString();
//     //System.Console.WriteLine($"Before truncating: {newLN} - {newRN}");
//     leftNum = Int64.Parse(newLN[..(newLN.Length/2)]);
//     rightNum = Int64.Parse(newRN[..(newRN.Length/2)]);

//     if (leftNum < Int64.Parse(newLN[(newLN.Length/2)..]))
//     {
//         leftNum++;
//     }

//     if (rightNum > Int64.Parse(newRN[(rightLen/2)..]))
//     {
//         // also adjust the number based on second half - if 2nd half is smaller we have to move up one
//         rightNum--;
//     }

//     //Console.WriteLine($"Adjusted range: {leftNum}-{rightNum}");
//     return (leftNum, rightNum);
// }