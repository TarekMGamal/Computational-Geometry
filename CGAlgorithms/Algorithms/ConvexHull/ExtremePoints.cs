using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                bool yes = true;
                for (int index = i+1; index < points.Count; index++)
                {
                    if (i == index) continue;
                    if (p.Equals(points[index])) yes = false;
                }
                
                for (int j = 0; j < points.Count && yes; j++)
                {
                    for (int k = j+1; k < points.Count && yes; k++)
                    {
                        for (int l = k+1; l < points.Count && yes; l++)
                        {
                            Point p1 = points[j];
                            Point p2 = points[k];
                            Point p3 = points[l];

                            if (p1.Equals(p) || p2.Equals(p) || p3.Equals(p)) continue;
                            bool coll = HelperMethods.PointOnSegment(p1, p2, p3);
                            coll &= HelperMethods.PointOnSegment(p2, p1, p3);
                            coll &= HelperMethods.PointOnSegment(p3, p1, p2);

                            if (coll) continue;

                            Enums.PointInPolygon tmp = HelperMethods.PointInTriangle(p, p1, p2, p3);
                            if (tmp != Enums.PointInPolygon.Outside) // on edge or inside
                            {
                                yes = false;
                            }
                        }
                    }
                }
                if (yes)
                {
                    outPoints.Add(p);
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
