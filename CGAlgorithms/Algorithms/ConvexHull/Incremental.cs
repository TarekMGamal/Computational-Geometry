using CGUtilities;
using CGUtilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public Line BaseLine;

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            double x = (points[0].X + points[1].X + points[2].X) / 3;
            double y = (points[0].Y + points[1].Y + points[2].Y) / 3;
            Point Base = new Point(x, y);
            Point NB = new Point(x + 1, y);
            BaseLine = new Line(Base, NB);

            OrderedSet<Point> CH = new OrderedSet<Point>(Angle_Comparer)
            {
                points[0],
                points[1],
                points[2]
            };

            Point prev, next;

            for (int i = 3; i < points.Count; i++) // O(n)
            {
                Point x_point = points[i];
                KeyValuePair<Point, Point> _ = CH.DirectUpperAndLower(x_point); // O(1)
                prev = _.Value;
                next = _.Key;

                if (HelperMethods.CheckTurn(new Line(prev, next), x_point) == Enums.TurnType.Right)
                {
                    // new prev
                    _ = CH.DirectUpperAndLower(prev);
                    Point new_prev = _.Value;
                    Point tmp = _.Key;
                    if (new_prev == default && tmp != default) new_prev = CH[CH.Count - 1];

                    while (HelperMethods.CheckTurn(new Line(x_point, prev), new_prev) != Enums.TurnType.Right) // O(1)
                    {
                        CH.Remove(prev); // O(log(n))
                        prev = new_prev;
                        _ = CH.DirectUpperAndLower(prev);
                        new_prev = _.Value;
                        tmp = _.Key;
                        if (new_prev == default && tmp != default) new_prev = CH[CH.Count - 1];
                    }

                    // new next
                    _ = CH.DirectUpperAndLower(next);
                    Point new_next = _.Key;
                    tmp = _.Value;
                    if (new_next == default && tmp != default) new_next = CH[0];

                    while (HelperMethods.CheckTurn(new Line(x_point, next), new_next) != Enums.TurnType.Left) // O(1)
                    {
                        CH.Remove(next); // O(log(n))
                        next = new_next;
                        _ = CH.DirectUpperAndLower(next);
                        new_next = _.Key;
                        tmp = _.Value;
                        if (new_next == default && tmp != default) new_next = CH[0];
                    }

                    CH.Add(x_point); // O(log(n))
                }
            }

            outPoints = CH.ToList();
        }

        public static double DotProduct(Point a, Point b)
        {
            return a.X * b.X + a.Y * b.Y;
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

        public static double Distance_Between_Two_Points(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        public int Angle_Comparer(Point a, Point b)
        {
            double angle1 = Get_Angle(BaseLine.Start, a);
            double angle2 = Get_Angle(BaseLine.Start, b);

            if (angle1 > angle2) return 1;
            else if (angle1 < angle2) return -1;
            else
            {
                double dis1 = Distance_Between_Two_Points(BaseLine.Start, a);
                double dis2 = Distance_Between_Two_Points(BaseLine.Start, b);

                if (dis1 > dis2) return 1;
                else if (dis1 < dis2) return -1;
                else return 0;
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
