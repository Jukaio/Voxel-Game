using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class RecursiveBacktracking  
{
    public enum Directions
    {
        North,
        South,
        East,
        West
    }
    static int N = 0;
    static int S = 1;
    static int E = 2;
    static int W = 3;

    static int[] DX = { 0, 0, 1, -1 };
    static int[] DY = { 1, -1, 0, 0 };
    static int[] OPP = { S, N, W, E };

    static int count;
    class Node
    {
        public bool[] directions = { false, false, false, false };
    }

    public static bool[,] build_path(Vector2Int start, Vector2Int size, int room_count)
    {
        count = room_count;
        Node[,] grid = new Node[size.x, size.y];
        grid[start.x, start.y] = new Node();
        count--; // Start room is first room
        recursive_building(start.x, start.y, ref grid);

        bool[,] paths = new bool[size.x, size.y];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] != null)
                {
                    foreach (var dir in grid[x, y].directions)
                    {
                        if (dir) paths[x, y] = true;
                    }
                }
                else
                {
                    paths[x, y] = false;
                }
            }
        }
        return paths;
    }

    private static void recursive_building(int cx, int cy, ref Node[,] grid)
    {
        List<int> directions = new List<int> { N, S, E, W };
        directions.Sort((lhs, rhs) => Random.Range(0, 3).CompareTo(Random.Range(0, 3)));
        

        foreach (var dir in directions)
        {
            if(dir == N || dir == S)
            {
                if (cx % 2 == 0)
                    continue;
            }
            else if (dir == E || dir == W)
            {
                if (cy % 2 == 0)
                    continue;
            }

            var nx = cx + DX[dir];
            var ny = cy + DY[dir];

            if (nx >= 0 &&
                nx < grid.GetLength(0) &&
                ny >= 0 &&
                ny < grid.GetLength(1) &&
                grid[nx, ny] == null)
            {
                count--;
                if (count < 0)
                    return;

                grid[nx, ny] = new Node();
                grid[cx, cy].directions[dir] = true;
                grid[nx, ny].directions[OPP[dir]] = true;

                recursive_building(nx, ny, ref grid);
            }
        }
    }
}
