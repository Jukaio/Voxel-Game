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
    class Node
    {
        public bool[] directions = { false, false, false, false };
    }

    public static void build_path(Vector2Int start, Vector2Int size, int depth)
    {
        Node[,] grid = new Node[size.x, size.y];
        recursive_building(0, 0, ref grid, depth);

        bool[,] paths = new bool[size.x, size.y];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                foreach (var dir in grid[x, y].directions)
                {
                    if (dir) paths[x, y] = true;
                }
            }
        }
    }

    private static void recursive_building(int cx, int cy, ref Node[,] grid, int depth)
    {
        if (depth < 0)
            return;

        List<int> directions = new List<int> { N, S, E, W };
        directions.Sort((emp1, emp2) => Random.Range(0, 10).CompareTo(Random.Range(0, 10)));

        foreach (var dir in directions)
        {
            var nx = cx + DX[dir];
            var ny = cy + DY[dir];

            if (nx > 0 &&
               nx < grid.GetLength(0) &&
               ny > 0 &&
               ny < grid.GetLength(1))
            {
                grid[cx, cy].directions[dir] = true;
                grid[nx, ny].directions[OPP[dir]] = true;
                recursive_building(nx, ny, ref grid, depth - 1);
            }
        }
    }
}
