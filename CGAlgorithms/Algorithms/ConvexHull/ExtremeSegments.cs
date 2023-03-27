using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count < 3)
            {
                outPoints = points;
                return;
            }
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i+1; j < points.Count; j++)
                {
                    Line line = new Line(points[i], points[j]);

                    int left_cnt = 0;
                    int right_cnt = 0;
                    int coll_cnt = 0;

                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j) continue;
                        
                        Enums.TurnType tmp = HelperMethods.CheckTurn(line , points[k]);
                        if (tmp == Enums.TurnType.Left) left_cnt++;
                        if (tmp == Enums.TurnType.Right) right_cnt++;
                        if (tmp == Enums.TurnType.Colinear && !HelperMethods.PointOnSegment(points[k], points[i], points[j])) coll_cnt++;
                    }

                    if (coll_cnt > 0) continue;

                    if (left_cnt == 0 || right_cnt == 0)
                    {
                        outLines.Add(line);
                        if (!outPoints.Contains(points[i])) outPoints.Add(points[i]);
                        if (!outPoints.Contains(points[j])) outPoints.Add(points[j]);
                    }
                }
            }   
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
