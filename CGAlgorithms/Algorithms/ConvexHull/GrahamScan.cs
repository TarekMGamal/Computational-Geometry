using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public double DotProduct(Point a, Point b)
        {
            double ret = a.X * b.X + a.Y * b.Y;
            return ret;
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // get minimum point
            
            double min_y = 1000000;
            int index = -1;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < min_y)
                {
                    min_y = points[i].Y;
                    index = i;
                }
            }

            Point a = points[index];
            Point virtual_point = new Point(a.X + 5, a.Y);
            Line line = new Line(a, virtual_point);
            

            List<KeyValuePair<double, Point>> Sorted_Points = new List<KeyValuePair<double, Point>>();
            double cross_product, dot_product, angle;
            Point vector1 = a.Vector(virtual_point);
            
            for (int i = 0; i < points.Count; i++) // O(n)
            {
                if (i == index) continue;

                Point vector2 = a.Vector(points[i]);
                dot_product = DotProduct(vector1, vector2);
                cross_product = HelperMethods.CrossProduct(vector1, vector2);
                angle = (180 / Math.PI) * Math.Atan2(dot_product, cross_product);
                Sorted_Points.Add(new KeyValuePair<double, Point>(angle, points[i]));
            }
            Sorted_Points.Sort((x,y) => x.Key.CompareTo(y.Key));  // O(n log(n))
            Sorted_Points.Add(new KeyValuePair<double, Point>(0 , a));

            // stack process

            Stack<Point> stack = new Stack<Point>();
            stack.Push(a);
            stack.Push(Sorted_Points[0].Value);
            

            for (int i = 1; i < Sorted_Points.Count; i++) // O(n)
            {
                Point top = stack.Pop();
                Point preTop = stack.Pop();

                Line segment = new Line(top, preTop);

                stack.Push(preTop);
                stack.Push(top);
                
                // if turn of point is right or collinear then discard
                while (stack.Count > 2 && HelperMethods.CheckTurn(segment, Sorted_Points[i].Value) != Enums.TurnType.Left) // O(1)
                {
                    stack.Pop(); // discard point

                    top = stack.Pop();
                    preTop = stack.Pop();

                    segment = new Line(top, preTop);

                    stack.Push(preTop);
                    stack.Push(top);
                }

                stack.Push(Sorted_Points[i].Value);
            }

            while (stack.Count > 1) outPoints.Add(stack.Pop());
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
