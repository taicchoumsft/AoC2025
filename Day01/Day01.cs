// dotnet run <file.cs> <input file>
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

string inputFile = args.Length > 0 ? args[0] : "input.txt";
var lines = await File.ReadAllLinesAsync(inputFile);

Console.WriteLine("Part 1: " + Part1(lines));
Console.WriteLine("Part 2: " + Part2(lines));

static object Part1(string[] lines)
{
    int pos = 50;
    int atZero = 0;

    foreach (var line in lines)
    {
        var dir = line[0];
        if (int.TryParse(line[1..], out int rot))
        {
            pos = dir switch
            {
                'L' => ((pos - rot) % 100 + 100) % 100,
                'R' => (pos + rot) % 100,
                _ => throw new ArgumentException("Only expecting L or R")
            };
        }
        if (pos == 0) atZero++;
    }

    return atZero;
}

static object Part2(string[] lines)
{
    int pos = 50;
    int passOrAtZero = 0;

    foreach (var line in lines)
    {
        char dir = line[0];
        
        if (int.TryParse(line[1..], out int rot))
        {
            if (dir == 'L')
            {
                // int curRot = rot;
                // if (curRot - curPos >= 0)
                // {
                //     passZero += 1 + (curRot - curPos) / 100;
                    
                //     //Console.WriteLine($"{dir}: {rot}: {curPos}: {1 + (curRot - curPos) / 100}");
                // }
                
                // simulate subtract leg, can't figure out the math
                int curRot = rot;
                int tmpPos = (pos == 0) ? 100: pos;
                int prevPos = (pos == 0) ? 100: pos;
                
                while (curRot > 0)
                {
                    int offset = Math.Min(100, curRot);
                    tmpPos -= offset;
                    curRot -= offset;
                    if (prevPos > 0 && tmpPos <= 0) {
                        passOrAtZero++;
                        tmpPos = (tmpPos + 100) % 100;
                        if (tmpPos == 0) tmpPos = 100;
                    }

                    prevPos = tmpPos;
                }

                pos = ((pos - rot) % 100 + 100) % 100;
            } 
            else
            {   
                if (rot >= 100 - pos)
                {
                    passOrAtZero += 1 + (rot - (100 - pos)) / 100;
                }

                pos = (pos + rot) % 100;
            }
        }
    }
    
    return passOrAtZero;
}
