using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;
                return;
            }

            int start = 0 , end = 0 , initial = 0;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Y < points[start].Y) start = i;
            }
            initial = start;

            
            outPoints.Add(points[start]);
            while (true)
            {
                if (start == end) end = (start + 1) % points.Count;
                

                for (int i = 0; i < points.Count; i++)
                {
                    if (i == start || i == end) continue;

                    Enums.TurnType tmp = HelperMethods.CheckTurn(new Line(points[start], points[end]), points[i]);

                    bool condition1 = (tmp == Enums.TurnType.Right);
                    // if end is inside line
                    bool condition2 = (tmp == Enums.TurnType.Colinear && HelperMethods.PointOnSegment(points[end], points[start], points[i]));

                    if (condition1 || condition2)
                    {
                        end = i;
                    }

                }

                if (initial == end) break;

                start = end;

                if (!outPoints.Contains(points[end])) outPoints.Add(points[end]);
                
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
