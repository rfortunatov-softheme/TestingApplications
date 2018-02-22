// <copyright company="Quest Software Inc.">
//     Confidential and Proprietary
//     Copyright 2016 Quest Software Inc.
//     ALL RIGHTS RESERVED.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace PathFinder
{
    public class PathFinder
    {
        public HashSet<PathPoint> Find(bool[][] matrix, PathPoint start, PathPoint end)
        {
            if (matrix.Length == 0)
            {
                throw new ArgumentException(nameof(matrix));
            }

            if (matrix.Length < start.Top || matrix[start.Top].Length < start.Left || !matrix[start.Top][start.Left])
            {
                throw new ArgumentException(nameof(start));
            }

            if (matrix.Length < end.Top || matrix[end.Top].Length < end.Left || !matrix[end.Top][end.Left])
            {
                throw new ArgumentException(nameof(end));
            }

            var optimalPath = new HashSet<PathPoint>();
            var allPaths = new List<ISet<PathPoint>>();
            var visitedPoints = new HashSet<PathPoint>();
            var processedStart = new PathPoint(start, matrix);
            ProcessNeighbors(matrix, end, processedStart, visitedPoints, allPaths, optimalPath);
            return optimalPath;
        }

        private static void ProcessNeighbors(bool[][] matrix, PathPoint end, PathPoint start, ISet<PathPoint> visitedPoints, ICollection<ISet<PathPoint>> allPaths, ISet<PathPoint> optimalPath, ISet<PathPoint> currentPath = null)
        {
            var path = currentPath ?? new HashSet<PathPoint>();
            var currentPathPoint = start;
            if (currentPathPoint.Equals(end))
            {
                visitedPoints.Add(currentPathPoint);
                path.Add(currentPathPoint);
                allPaths.Add(path);
                if (optimalPath.Count == 0 || optimalPath.Count > path.Count)
                {
                    optimalPath.Clear();
                    foreach (var point in path)
                    {
                        optimalPath.Add(point);
                    }

                    return;
                }
            }

            while (currentPathPoint.AvailableNeighbors(matrix).Any())
            {
                if (visitedPoints.Contains(currentPathPoint))
                {
                    break;
                }

                visitedPoints.Add(currentPathPoint);
                if (currentPathPoint.Equals(end))
                {
                    visitedPoints.Add(currentPathPoint);
                    path.Add(currentPathPoint);
                    allPaths.Add(path);
                    if (optimalPath.Count == 0 || optimalPath.Count > path.Count)
                    {
                        optimalPath.Clear();
                        foreach (var point in path)
                        {
                            optimalPath.Add(point);
                        }

                        break;
                    }
                }

                path.Add(currentPathPoint);
                if (currentPathPoint.AvailableNeighbors(matrix).Count == 1)
                {
                    currentPathPoint = currentPathPoint.AvailableNeighbors(matrix).First();
                }
                else
                {
                    foreach (var neighbor in currentPathPoint.AvailableNeighbors(matrix))
                    {
                        // Stack overflow here on more or less big matrix. Though I don't want to fix it.
                        ProcessNeighbors(matrix, end, neighbor, new HashSet<PathPoint>(visitedPoints), allPaths, optimalPath, new HashSet<PathPoint>(path));
                    }
                }
            }
        }
    }

    public class PathPoint
    {
        private HashSet<PathPoint> _availableNeighbors;
        public PathPoint(uint left, uint top)
        {
            Left = left;
            Top = top;
            CanVisit = true;
        }

        public PathPoint(uint left, uint top, bool[][] matrix, bool checkNeighbors)
        {
            Left = left;
            Top = top;
            CanVisit = matrix.Length >= top && matrix[top].Length >= left && matrix[top][left];
        }

        public PathPoint(PathPoint point, bool[][] matrix)
        {
            Left = point.Left;
            Top = point.Top;
            CanVisit = matrix.Length >= point.Top && matrix[point.Top].Length >= point.Left && matrix[point.Top][point.Left];
        }

        public uint Left { get; }

        public uint Top { get; }

        public bool CanVisit { get; set; }

        public HashSet<PathPoint> AvailableNeighbors(bool[][] matrix)
        {
            if (_availableNeighbors != null)
            {
                return _availableNeighbors;
            }

            _availableNeighbors = new HashSet<PathPoint>();
            if (CanVisit)
            {
                if (Top > 0)
                {
                    var neighbor = new PathPoint(Left, Top - 1, matrix, false);
                    if (neighbor.CanVisit)
                    {
                        _availableNeighbors.Add(neighbor);
                    }
                }

                if (Top < matrix.Length - 1)
                {
                    var neighbor = new PathPoint(Left, Top + 1, matrix, false);
                    if (neighbor.CanVisit)
                    {
                        _availableNeighbors.Add(neighbor);
                    }
                }

                if (Left > 0)
                {
                    var neighbor = new PathPoint(Left - 1, Top, matrix, false);
                    if (neighbor.CanVisit)
                    {
                        _availableNeighbors.Add(neighbor);
                    }
                }

                if (Left < matrix[Top].Length - 1)
                {
                    var neighbor = new PathPoint(Left + 1, Top, matrix, false);
                    if (neighbor.CanVisit)
                    {
                        _availableNeighbors.Add(neighbor);
                    }
                }
            }

            return _availableNeighbors;
        }

        public override bool Equals(object other)
        {
            if (other is PathPoint otherPoint)
            {
                return Equals(otherPoint);
            }

            return base.Equals(other);
        }

        protected bool Equals(PathPoint other)
        {
            return Left == other.Left && Top == other.Top;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Left * 397) ^ (int)Top;
            }
        }
    }
}