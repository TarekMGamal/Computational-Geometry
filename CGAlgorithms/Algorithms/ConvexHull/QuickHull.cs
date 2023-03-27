using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point a = new Point(0,0), b = new Point(0,0);
            double min_left = 1000000.0 , max_right = -1000000.0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].X < min_left)
                {
                    min_left = points[i].X;
                    a = points[i];
                }
                if (points[i].X > max_right)
                {
                    max_right = points[i].X;
                    b = points[i];
                }
            }

            // O(n^2)
            List<Point> right = Quick_Hull(points, a, b, false);
            List<Point> left = Quick_Hull(points, a, b, true);

            for (int i = 0; i < left.Count; ++i)
                right.Add(left[i]);

            for (int i = 0; i < right.Count; i++)
            {
                if (!outPoints.Contains(right[i])) outPoints.Add(right[i]);
            }
        }

        public double Max_Distance(Point a, Point b, Point c)
        {
            return Math.Abs((c.Y - a.Y) * (b.X - a.X) - (b.Y - a.Y) * (c.X - a.X));
        }

        public List<Point> Quick_Hull(List<Point> points, Point a, Point b, bool is_left)
        {
            if (points.Count == 0) return new List<Point>();

            var turn = (is_left ? Enums.TurnType.Left : Enums.TurnType.Right);

            int index = -1;
            double max_distance = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double distance = Max_Distance(a, b, points[i]);
                Line line = new Line(a.X, a.Y, b.X, b.Y);
                if (distance > max_distance && HelperMethods.CheckTurn(line, points[i]) == turn)
                {
                    index = i;
                    max_distance = distance;
                }
            }


            if (index == -1)
            {
                List<Point> ret = new List<Point>();
                ret.Add(a);
                ret.Add(b);

                return ret;
            }

            Point c = points[index];
            List<Point> p1, p2;

            turn = HelperMethods.CheckTurn(new Line(c.X, c.Y, a.X, a.Y), b);
            is_left = (turn != Enums.TurnType.Left);
            p1 = Quick_Hull(points, c, a, is_left);


            turn = HelperMethods.CheckTurn(new Line(c.X, c.Y, b.X, b.Y), a);
            is_left = (turn != Enums.TurnType.Left);
            p2 = Quick_Hull(points, c, b, is_left);

            for (int i = 0; i < p2.Count; ++i) {
                p1.Add(p2[i]);
            }

            return p1;
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
