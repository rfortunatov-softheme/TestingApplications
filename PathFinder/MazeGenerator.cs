// <copyright company="Quest Software Inc.">
//     Confidential and Proprietary
//     Copyright 2016 Quest Software Inc.
//     ALL RIGHTS RESERVED.
// </copyright>

using System;
using System.Threading;

namespace PathFinder
{
    // Fairly copy-pasted from some forum.
    public class MazeGenerator
    {
        private int mazeHeight, mazeWidth;
        public bool[][] maze { get; private set; }

        public bool[][] GenerateMaze(int height, int width, out int startTop, out int startLeft)
        {
            maze = new bool[height][];
            for (var i = 0; i < height; i++)
            {
                maze[i] = new bool[width];
            }

            // Initialize
            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
                maze[i][j] = false;

            var rand = new Random();
            // r for row、c for column
            // Generate random r
            startTop = rand.Next(height);
            while (startTop % 2 == 0) startTop = rand.Next(height);
            // Generate random c
            startLeft = rand.Next(width);
            while (startLeft % 2 == 0) startLeft = rand.Next(width);
            // Starting cell
            maze[startTop][startLeft] = true;

            mazeHeight = height;
            mazeWidth = width;

            //　Allocate the maze with recursive method
            Recursion(startTop, startLeft);

            return maze;
        }

        public void Recursion(int r, int c)
        {
            var directions = new[] {1, 2, 3, 4};

            Shuffle(directions);

            for (var i = 0; i < directions.Length; i++)
                switch (directions[i])
                {
                    case 1:
                        if (r - 2 <= 0)
                            continue;
                        if (!maze[r - 2][c])
                        {
                            maze[r - 2][c] = true;
                            maze[r - 1][c] = true;
                            Recursion(r - 2, c);
                        }

                        break;
                    case 2:
                        if (c + 2 >= mazeWidth - 1)
                            continue;
                        if (!maze[r][c + 2])
                        {
                            maze[r][c + 2] = true;
                            maze[r][c + 1] = true;
                            Recursion(r, c + 2);
                        }

                        break;
                    case 3:
                        if (r + 2 >= mazeHeight - 1)
                            continue;
                        if (!maze[r + 2][c])
                        {
                            maze[r + 2][c] = true;
                            maze[r + 1][c] = true;
                            Recursion(r + 2, c);
                        }

                        break;
                    case 4:
                        if (c - 2 <= 0)
                            continue;
                        if (!maze[r][c - 2])
                        {
                            maze[r][c - 2] = true;
                            maze[r][c - 1] = true;
                            Recursion(r, c - 2);
                        }

                        break;
                }
        }

        public void Shuffle<T>(T[] array)
        {
            var _random = new Random();
            for (var i = array.Length; i > 1; i--)
            {
                var j = _random.Next(i);
                var tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }

            Thread.Sleep(1);
        }
    }
}