using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points.Sort((a, b) => a.X == b.X ? a.Y.CompareTo(b.Y) : a.X.CompareTo(b.X));
            outPoints = Get_Convex_Hull(points);
        }

        public static double DotProduct(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        List<Point> Get_Convex_Hull(List<Point> points)
        {
            if (points.Count <= 3) return Sort_By_Angle(points);

            double sum = 0;
            for (int i = 0; i < points.Count; i++)
            {
                sum += points[i].X;
            }

            double avg = sum / points.Count;
            List<Point> left_points = new List<Point>();
            List<Point> right_points = new List<Point>();

            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p.X < avg) left_points.Add(p);
                else right_points.Add(p);
            }

            if (left_points.Count == 0) return new List<Point> { right_points[0], right_points[right_points.Count - 1] };
            if (right_points.Count == 0) return new List<Point> { left_points[0], left_points[left_points.Count - 1] };

            List<Point> left_hull = Get_Convex_Hull(left_points);
            List<Point> right_hull = Get_Convex_Hull(right_points);

            return Merge(left_hull, right_hull);
        }

        List<Point> Merge(List<Point> LHull, List<Point> RHull)
        {
            int l = 0, r = 0;
            double maxX = -10000, minX = 10000;

            for (int i = 1; i < LHull.Count; i++)
            {
                if (LHull[i].X > maxX)
                {
                    maxX = LHull[i].X;
                    l = i;
                }
            }

            for (int i = 1; i < RHull.Count; i++)
            {
                if (RHull[i].X > minX)
                {
                    minX = RHull[i].X;
                    r = i;
                }
            }

            Tuple<int, int> lower = Get_Tangent(LHull, RHull, l, r);
            Tuple<int, int> upper = Get_Tangent(RHull, LHull, r, l);
            List<Point> merged_list = new List<Point>();

            for (int i = upper.Item2; i != lower.Item1; i = (i + 1) % LHull.Count)
            {
                merged_list.Add(LHull[i]);
            }
            merged_list.Add(LHull[lower.Item1]);

            for (int i = lower.Item2; i != upper.Item1; i = (i + 1) % RHull.Count)
            {
                merged_list.Add(RHull[i]);
            }
            merged_list.Add(RHull[upper.Item1]);
            
            for (int i = 0; i < merged_list.Count; i++)
            {
                Line l1 = new Line(merged_list[(i - 1 + merged_list.Count) % merged_list.Count], merged_list[i]);
                if (HelperMethods.CheckTurn(l1, merged_list[(i + 1) % merged_list.Count]) != Enums.TurnType.Colinear) continue;

                merged_list.RemoveAt(i);
            }

            return merged_list;
        }

        Tuple<int, int> Get_Tangent(List<Point> left_hull, List<Point> right_hull, int l, int r)
        {
            Line tan = new Line(left_hull[l], right_hull[r]);

            while (!(HelperMethods.CheckTurn(tan, left_hull[(l + 1) % left_hull.Count]) != Enums.TurnType.Right &&
                    HelperMethods.CheckTurn(tan, right_hull[(r + 1) % right_hull.Count]) != Enums.TurnType.Right &&
                    HelperMethods.CheckTurn(tan, left_hull[(l - 1 + left_hull.Count) % left_hull.Count]) != Enums.TurnType.Right &&
                    HelperMethods.CheckTurn(tan, right_hull[(r - 1 + right_hull.Count) % right_hull.Count]) != Enums.TurnType.Right))
            {
                while (HelperMethods.CheckTurn(tan, left_hull[(l + 1) % left_hull.Count]) == Enums.TurnType.Right ||
                       HelperMethods.CheckTurn(tan, left_hull[(l - 1 + left_hull.Count) % left_hull.Count]) == Enums.TurnType.Right)
                {
                    l = (l - 1 + left_hull.Count) % left_hull.Count;
                    tan = new Line(left_hull[l], right_hull[r]);
                }

                while (HelperMethods.CheckTurn(tan, right_hull[(r + 1) % right_hull.Count]) == Enums.TurnType.Right ||
                       HelperMethods.CheckTurn(tan, right_hull[(r - 1 + right_hull.Count) % right_hull.Count]) == Enums.TurnType.Right)
                {
                    r = (r + 1) % right_hull.Count;
                    tan = new Line(left_hull[l], right_hull[r]);
                }
            }

            return Tuple.Create(l, r);
        }

        List<Point> Sort_By_Angle(List<Point> points)
        {
            int index = 0;
            double min_x = 10000.0;

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < min_x)
                {
                    min_x = points[i].X;
                    index = i;
                }
            }

            Line virtual_line = new Line(points[index], new Point(points[index].X + 1, points[index].Y));
            List<KeyValuePair<double, int>> list = new List<KeyValuePair<double, int>>();

            for (int i = 0; i < points.Count; i++)
            {
                if (i == index) continue;

                double angle = Get_Angle(virtual_line.Start, points[i]);
                list.Add(new KeyValuePair<double, int>(angle, i));
            }

            list.Sort((a, b) => a.Key == b.Key ? a.Value.CompareTo(b.Value) : a.Key.CompareTo(b.Key));

            List<Point> sorted_points = new List<Point>
            {
                points[index]
            };

            sorted_points.AddRange(list.Select(item => points[item.Value]));

            return sorted_points;
        }

        public static double Get_Angle(Point a, Point b)
        {
            double x = a.X + 1;
            double y = a.Y;
            Point tmp = new Point(x, y);
            Point vec1 = a.Vector(tmp);
            Point vec2 = a.Vector(b);

            double cross_product = HelperMethods.CrossProduct(vec1, vec2);
            double dot_product = DotProduct(vec1, vec2);

            double angle = Math.Atan2(cross_product, dot_product) * (180 / Math.PI);
            if (angle < 0) angle += 360;

            return angle;
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
