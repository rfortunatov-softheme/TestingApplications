using System;

namespace PathFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            const int height = 150;
            const int width = 150;
            var generator = new MazeGenerator();
            var matrix = generator.GenerateMaze(height, width, out var startTop, out var startLeft);

            var start = new PathPoint((uint)startLeft, (uint)startTop);
            PathPoint end = null;
            for (var i = height - 1; i > 0; i--)
            for (var j = width - 1; j > 0; j--)
            {
                if (matrix[i][j])
                {
                    end = new PathPoint((uint)j, (uint)i);
                    break;
                }
            }

            Console.WriteLine("Maze");
            foreach (var row in matrix)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(Environment.NewLine);
                foreach (var point in row)
                {
                    if (point)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(" ");
                    }
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            Console.WriteLine();
            var finder = new PathFinder();
            var res = finder.Find(matrix, start, end);

            var falseMatrix = GetFalseMatrix(height, width);
            foreach (var pathPoint in res)
            {
                falseMatrix[pathPoint.Top][pathPoint.Left] = true;
            }

            Console.WriteLine("Path");
            foreach (var row in falseMatrix)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(Environment.NewLine);
                foreach (var point in row)
                {
                    if (point)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(" ");
                    }
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();
        }

        static bool[][] GetFalseMatrix(int height, int width)
        {
            var falseMatrix = new bool[height][];
            for (var i = 0; i < height; i++)
            {
                falseMatrix[i] = new bool[width];
                for (var j = 0; j < width; j++)
                {
                    falseMatrix[i][j] = false;
                }
            }

            return falseMatrix;
        }
    }
}
