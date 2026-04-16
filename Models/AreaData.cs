namespace ReverseGeocodeLib.Models
{
    public class AreaData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<List<List<double>>> Coordinates { get; set; }

        public AreaData(string id, string name, List<List<List<double>>> coordinates)
        {
            Id = id;
            Name = name;
            Coordinates = coordinates;
        }

        /// <summary>
        /// Determines if a point (latitude, longitude) is inside this polygon.
        /// Handles both regular Polygons (with holes) and flattened MultiPolygons.
        /// Simple approach: test all rings independently.
        /// </summary>
        public bool ContainsPoint(double latitude, double longitude)
        {
            if (Coordinates == null || Coordinates.Count == 0)
                return false;

            // If we have many rings (>10), treat them as separate polygons (flattened MultiPolygon)
            if (Coordinates.Count > 10)
            {
                foreach (var ring in Coordinates)
                {
                    if (IsPointInPolygonRing(latitude, longitude, ring))
                        return true;
                }
                return false;
            }

            // For 2-10 rings, we can't reliably distinguish between holes and separate polygons
            // (we lost this info when flattening MultiPolygons from GeoJSON)
            // So we test all rings independently
            // If point is in any ring, return true
            if (Coordinates.Count >= 2)
            {
                foreach (var ring in Coordinates)
                {
                    if (IsPointInPolygonRing(latitude, longitude, ring))
                        return true;
                }
                return false;
            }

            // Single ring - simple case
            return IsPointInPolygonRing(latitude, longitude, Coordinates[0]);
        }

        /// <summary>
        /// Gets the bounding box of a ring.
        /// </summary>
        private (double minLat, double maxLat, double minLon, double maxLon) GetRingBounds(List<List<double>> ring)
        {
            if (ring.Count == 0)
                return (0, 0, 0, 0);

            double minLat = ring[0][1];
            double maxLat = ring[0][1];
            double minLon = ring[0][0];
            double maxLon = ring[0][0];

            for (int i = 1; i < ring.Count; i++)
            {
                minLat = Math.Min(minLat, ring[i][1]);
                maxLat = Math.Max(maxLat, ring[i][1]);
                minLon = Math.Min(minLon, ring[i][0]);
                maxLon = Math.Max(maxLon, ring[i][0]);
            }

            return (minLat, maxLat, minLon, maxLon);
        }

        /// <summary>
        /// Ray Casting algorithm to determine if a point is inside a polygon ring.
        /// Public version for testing/debugging.
        /// </summary>
        public static bool IsPointInRing(double latitude, double longitude, List<List<double>> ring)
        {
            return IsPointInPolygonRing(latitude, longitude, ring);
        }

        /// <summary>
        /// Ray Casting algorithm to determine if a point is inside a polygon ring.
        /// </summary>
        private static bool IsPointInPolygonRing(double latitude, double longitude, List<List<double>> ring)
        {
            int n = ring.Count;
            if (n < 3)
                return false;

            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                double xi = ring[i][0], yi = ring[i][1];  // longitude, latitude
                double xj = ring[j][0], yj = ring[j][1];

                // Check if point's y-coordinate is between the edge's y-coordinates
                bool yIntersect = ((yi > latitude) != (yj > latitude));

                if (yIntersect)
                {
                    // Calculate x-intersection of the horizontal line through the point with the edge
                    double xIntersect = (xj - xi) * (latitude - yi) / (yj - yi) + xi;

                    // If the point's x-coordinate is to the left of the intersection, toggle inside
                    if (longitude < xIntersect)
                        inside = !inside;
                }
            }

            return inside;
        }
    }    
}

