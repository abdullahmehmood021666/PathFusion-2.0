using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;


namespace PathFusion.Core.Algorithms
{

    public class AllPathsFinder
    {
        private List<Route> _allPaths;
        private List<Node> _visited;
        private readonly int _maxPathLength;
        private readonly double _maxDistance;

        public AllPathsFinder(int maxPathLength = 20, double maxDistance = 5000)
        {
            _allPaths = new();
            _visited = new();
            _maxPathLength = maxPathLength;
            _maxDistance = maxDistance;
        }

        // ============================================================================
        // MAIN PATH FINDING METHODS
        // ============================================================================

        /// <summary>
        /// Find ALL paths from source to destination using DFS
        /// Returns ordered by: distance, then hops, then time
        /// </summary>
        public List<Route> FindAllPaths(Node source, Node destination, List<Edge> edges)
        {
            if (source == null || destination == null || edges == null || edges.Count == 0)
                return new List<Route>();

            _allPaths = new();
            _visited = new();

            var path = new List<Node> { source };
            var pathDistance = 0.0;
            var pathTime = 0.0;

            DfsAllPaths(source, destination, edges, path, pathDistance, pathTime);

            return _allPaths
                .OrderBy(p => p.TotalDistance)
                .ThenBy(p => p.Hops)
                .ThenBy(p => p.TravelTime)
                .ToList();
        }

        /// <summary>
        /// Recursive DFS to find all paths
        /// </summary>
        private void DfsAllPaths(
            Node current,
            Node destination,
            List<Edge> edges,
            List<Node> path,
            double distance,
            double time)
        {
            // Base case: reached destination
            if (current.Id == destination.Id)
            {
                var route = new Route
                {
                    SourceNodeId = path.First().Id,
                    DestinationNodeId = destination.Id,
                    Path = new List<Node>(path),
                    TotalDistance = distance,
                    Hops = path.Count - 1,
                    TravelTime = time,
                    Edges = BuildEdgesForPath(path, edges),
                    CreatedAt = DateTime.UtcNow,
                    RouteQuality = CalculateRouteQuality(path.Count - 1, distance, time)
                };

                _allPaths.Add(route);
                return;
            }

            // Pruning conditions
            if (path.Count > _maxPathLength || distance > _maxDistance)
                return;

            // Explore neighbors
            var neighbors = GetNeighbors(current, edges, path);

            foreach (var (neighbor, edge) in neighbors)
            {
                path.Add(neighbor);
                DfsAllPaths(
                    neighbor,
                    destination,
                    edges,
                    path,
                    distance + edge.Distance,
                    time + CalculateTime(edge.Distance)
                );
                path.RemoveAt(path.Count - 1);
            }
        }

        /// <summary>
        /// Get unvisited neighbors of current node
        /// </summary>
        private List<(Node neighbor, Edge edge)> GetNeighbors(Node current, List<Edge> edges, List<Node> path)
        {
            var pathIds = path.Select(n => n.Id).ToHashSet();

            return edges
                .Where(e => (e.SourceNodeId == current.Id || e.DestNodeNodeId == current.Id) &&
                            !pathIds.Contains(e.SourceNodeId == current.Id ? e.DestNodeNodeId : e.SourceNodeId))
                .Select(e =>
                {
                    var neighborId = e.SourceNodeId == current.Id ? e.DestNodeNodeId : e.SourceNodeId;
                    return (new Node { Id = neighborId, Name = $"Node_{neighborId}" }, e);
                })
                .ToList();
        }

        // ============================================================================
        // FILTERED PATH FINDING METHODS
        // ============================================================================

        /// <summary>
        /// Find paths filtered by vehicle type
        /// </summary>
        public List<Route> FindPathsByVehicle(
            Node source,
            Node destination,
            List<Edge> edges,
            VehicleTypeDefinitions.VehicleType vehicle)
        {
            var allPaths = FindAllPaths(source, destination, edges);

            return allPaths
                .Where(p => IsPathCompatibleWithVehicle(p, vehicle))
                .ToList();
        }

        /// <summary>
        /// Check if path is compatible with vehicle
        /// </summary>
        private bool IsPathCompatibleWithVehicle(Route path, VehicleTypeDefinitions.VehicleType vehicle)
        {
            // Vehicle must support all nodes in path
            return path.Path.All(node => vehicle.RouteCompatibility.Contains(node.Id));
        }

        /// <summary>
        /// Find paths within distance limit
        /// </summary>
        public List<Route> FindPathsByDistance(
            Node source,
            Node destination,
            List<Edge> edges,
            double maxDistance)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths.Where(p => p.TotalDistance <= maxDistance).ToList();
        }

        /// <summary>
        /// Find paths within hop count limit
        /// </summary>
        public List<Route> FindPathsByHops(
            Node source,
            Node destination,
            List<Edge> edges,
            int maxHops)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths.Where(p => p.Hops <= maxHops).ToList();
        }

        /// <summary>
        /// Find paths within time limit
        /// </summary>
        public List<Route> FindPathsByTime(
            Node source,
            Node destination,
            List<Edge> edges,
            double maxHours)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths.Where(p => p.TravelTime <= maxHours).ToList();
        }

        /// <summary>
        /// Find top N shortest paths
        /// </summary>
        public List<Route> FindTopPaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 5)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths.Take(count).ToList();
        }

        // ============================================================================
        // TRANSPORT MODE FILTERING
        // ============================================================================

        /// <summary>
        /// Find paths using only road transport
        /// </summary>
        public List<Route> FindRoadOnlyPaths(Node source, Node destination, List<Edge> edges)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .Where(p => p.Edges.All(e => e.TransportMode == "Road"))
                .ToList();
        }

        /// <summary>
        /// Find paths using only rail transport
        /// </summary>
        public List<Route> FindRailOnlyPaths(Node source, Node destination, List<Edge> edges)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .Where(p => p.Edges.All(e => e.TransportMode == "Rail"))
                .ToList();
        }

        /// <summary>
        /// Find paths using only air transport
        /// </summary>
        public List<Route> FindAirOnlyPaths(Node source, Node destination, List<Edge> edges)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .Where(p => p.Edges.All(e => e.TransportMode == "Air"))
                .ToList();
        }

        /// <summary>
        /// Find multi-modal paths (mixed transport modes)
        /// </summary>
        public List<Route> FindMultimodalPaths(Node source, Node destination, List<Edge> edges)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .Where(p => p.Edges.Select(e => e.TransportMode).Distinct().Count() > 1)
                .ToList();
        }

        // ============================================================================
        // COST & EMISSIONS FILTERING
        // ============================================================================

        /// <summary>
        /// Find cheapest paths (by cost per km)
        /// </summary>
        public List<Route> FindCheapestPaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 3)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .OrderBy(p => p.EstimatedCost)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Find eco-friendly paths (lowest emissions)
        /// </summary>
        public List<Route> FindEcoFriendlyPaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 3)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .OrderBy(p => p.EstimatedEmissions)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Find fastest paths
        /// </summary>
        public List<Route> FindFastestPaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 3)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .OrderBy(p => p.TravelTime)
                .Take(count)
                .ToList();
        }

        // ============================================================================
        // SAFETY & COMFORT FILTERING
        // ============================================================================

        /// <summary>
        /// Find safest paths (highest safety rating)
        /// </summary>
        public List<Route> FindSafestPaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 3)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .OrderByDescending(p => p.Edges.Average(e => e.SafetyRating))
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Find most comfortable paths (highest comfort rating)
        /// </summary>
        public List<Route> FindMostComfortablePaths(
            Node source,
            Node destination,
            List<Edge> edges,
            int count = 3)
        {
            var allPaths = FindAllPaths(source, destination, edges);
            return allPaths
                .OrderByDescending(p => p.Edges.Average(e => e.ComfortRating))
                .Take(count)
                .ToList();
        }

        // ============================================================================
        // ANALYSIS & STATISTICS
        // ============================================================================

        /// <summary>
        /// Get statistics about all found paths
        /// </summary>
        public PathStatistics AnalyzeAllPaths(Node source, Node destination, List<Edge> edges)
        {
            var allPaths = FindAllPaths(source, destination, edges);

            if (allPaths.Count == 0)
                return null;

            return new PathStatistics
            {
                TotalPathsFound = allPaths.Count,
                ShortestPath = allPaths.OrderBy(p => p.TotalDistance).First(),
                FastestPath = allPaths.OrderBy(p => p.TravelTime).First(),
                CheapestPath = allPaths.OrderBy(p => p.EstimatedCost).First(),
                EcoFriendliestPath = allPaths.OrderBy(p => p.EstimatedEmissions).First(),
                SafestPath = allPaths.OrderByDescending(p => p.Edges.Average(e => e.SafetyRating)).First(),
                MostComfortablePath = allPaths.OrderByDescending(p => p.Edges.Average(e => e.ComfortRating)).First(),
                AverageDistance = allPaths.Average(p => p.TotalDistance),
                AverageHops = allPaths.Average(p => p.Hops),
                AverageTime = allPaths.Average(p => p.TravelTime),
                MinDistance = allPaths.Min(p => p.TotalDistance),
                MaxDistance = allPaths.Max(p => p.TotalDistance),
                MinHops = allPaths.Min(p => p.Hops),
                MaxHops = allPaths.Max(p => p.Hops),
                AnalyzedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Compare two specific paths
        /// </summary>
        public PathComparison ComparePaths(Route path1, Route path2)
        {
            return new PathComparison
            {
                Path1 = path1,
                Path2 = path2,
                DistanceDifference = Math.Abs(path1.TotalDistance - path2.TotalDistance),
                TimeDifference = Math.Abs(path1.TravelTime - path2.TravelTime),
                CostDifference = Math.Abs(path1.EstimatedCost - path2.EstimatedCost),
                EmissionDifference = Math.Abs(path1.EstimatedEmissions - path2.EstimatedEmissions),
                HopDifference = Math.Abs(path1.Hops - path2.Hops),
                BetterPath = CalculateBetterPath(path1, path2)
            };
        }

        /// <summary>
        /// Determine which path is better overall
        /// </summary>
        private Route CalculateBetterPath(Route path1, Route path2)
        {
            var score1 = (path1.TotalDistance * 0.3) + (path1.TravelTime * 0.3) + (path1.EstimatedCost * 0.2) + (path1.EstimatedEmissions * 0.2);
            var score2 = (path2.TotalDistance * 0.3) + (path2.TravelTime * 0.3) + (path2.EstimatedCost * 0.2) + (path2.EstimatedEmissions * 0.2);

            return score1 < score2 ? path1 : path2;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Build edge list for a given path
        /// </summary>
        private List<Edge> BuildEdgesForPath(List<Node> path, List<Edge> allEdges)
        {
            var pathEdges = new List<Edge>();

            for (int i = 0; i < path.Count - 1; i++)
            {
                var currentNode = path[i];
                var nextNode = path[i + 1];

                var edge = allEdges.FirstOrDefault(e =>
                    (e.SourceNodeId == currentNode.Id && e.DestNodeNodeId == nextNode.Id) ||
                    (e.DestNodeNodeId == currentNode.Id && e.SourceNodeId == nextNode.Id));

                if (edge != null)
                    pathEdges.Add(edge);
            }

            return pathEdges;
        }

        /// <summary>
        /// Calculate travel time based on distance and average speed
        /// </summary>
        private double CalculateTime(double distance)
        {
            const double averageSpeed = 60; // km/h
            return distance / averageSpeed; // hours
        }

        /// <summary>
        /// Calculate route quality score (0-100)
        /// Based on: distance, hops, time
        /// </summary>
        private double CalculateRouteQuality(int hops, double distance, double time)
        {
            // Normalize factors
            var hopScore = Math.Max(0, 100 - (hops * 5)); // 5 points per hop
            var distanceScore = Math.Max(0, 100 - (distance / 100)); // 1 point per km
            var timeScore = Math.Max(0, 100 - (time * 10)); // 10 points per hour

            // Weighted average
            return (hopScore * 0.4) + (distanceScore * 0.3) + (timeScore * 0.3);
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// Route - represents a complete path with metadata
        /// </summary>
        public class Route
        {
            public int SourceNodeId { get; set; }
            public int DestinationNodeId { get; set; }
            public List<Node> Path { get; set; } = new();
            public List<Edge> Edges { get; set; } = new();
            public double TotalDistance { get; set; }
            public int Hops { get; set; }
            public double TravelTime { get; set; } // hours
            public double EstimatedCost { get; set; } // PKR
            public double EstimatedEmissions { get; set; } // kg CO2
            public double RouteQuality { get; set; } // 0-100
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                var pathString = string.Join(" → ", Path.Select(n => n.Name ?? $"Node_{n.Id}"));
                return $@"
╔════════════════════════════════════════════╗
║ Route: {pathString}
╠════════════════════════════════════════════╣
║ Total Distance:    {TotalDistance,25:F2} km
║ Number of Hops:    {Hops,25}
║ Travel Time:       {TravelTime,25:F2} hours
║ Estimated Cost:    {EstimatedCost,25:F0} PKR
║ Emissions:         {EstimatedEmissions,25:F2} kg CO₂
║ Route Quality:     {RouteQuality,25:F2}/100
╚════════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// Path Statistics - summary of all paths
        /// </summary>
        public class PathStatistics
        {
            public int TotalPathsFound { get; set; }
            public Route ShortestPath { get; set; }
            public Route FastestPath { get; set; }
            public Route CheapestPath { get; set; }
            public Route EcoFriendliestPath { get; set; }
            public Route SafestPath { get; set; }
            public Route MostComfortablePath { get; set; }
            public double AverageDistance { get; set; }
            public double AverageHops { get; set; }
            public double AverageTime { get; set; }
            public double MinDistance { get; set; }
            public double MaxDistance { get; set; }
            public int MinHops { get; set; }
            public int MaxHops { get; set; }
            public DateTime AnalyzedAt { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
           PATH ANALYSIS REPORT
═══════════════════════════════════════════════
Total Paths Found:     {TotalPathsFound}
Analysis Time:         {AnalyzedAt:g}

AVERAGES:
├─ Avg Distance:       {AverageDistance:F2} km
├─ Avg Hops:           {AverageHops:F1}
└─ Avg Travel Time:    {AverageTime:F2} hours

DISTANCE RANGE:
├─ Minimum:            {MinDistance:F2} km
└─ Maximum:            {MaxDistance:F2} km

HOPS RANGE:
├─ Minimum:            {MinHops}
└─ Maximum:            {MaxHops}

BEST PATHS:
├─ Shortest:           {ShortestPath?.TotalDistance:F2} km ({ShortestPath?.Hops} hops)
├─ Fastest:            {FastestPath?.TravelTime:F2} hours
├─ Cheapest:           {CheapestPath?.EstimatedCost:F0} PKR
├─ Greenest:           {EcoFriendliestPath?.EstimatedEmissions:F2} kg CO₂
├─ Safest:             Avg Rating {SafestPath?.Edges?.Average(e => e.SafetyRating):F1}/5
└─ Most Comfortable:   Avg Rating {MostComfortablePath?.Edges?.Average(e => e.ComfortRating):F1}/5

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// Path Comparison - compare two routes
        /// </summary>
        public class PathComparison
        {
            public Route Path1 { get; set; }
            public Route Path2 { get; set; }
            public double DistanceDifference { get; set; }
            public double TimeDifference { get; set; }
            public double CostDifference { get; set; }
            public double EmissionDifference { get; set; }
            public int HopDifference { get; set; }
            public Route BetterPath { get; set; }

            public override string ToString()
            {
                 return $@"
                    ╔════════════════════════════════════════════╗
                    ║         PATH COMPARISON REPORT
                    ╠════════════════════════════════════════════╣
                    ║ Distance Difference:  {DistanceDifference,20:F2} km
                    ║ Time Difference:      {TimeDifference,20:F2} hours
                    ║ Cost Difference:      {CostDifference,20:F0} PKR
                    ║ Emission Difference:  {EmissionDifference,20:F2} kg CO₂
                    ║ Hop Difference:       {HopDifference,20}
                    ╠════════════════════════════════════════════╣
                    ║ Better Path: {(BetterPath == Path1 ? "Path 1" : "Path 2")}
                    ╚════════════════════════════════════════════╝
                    ";
            }
        }
    }

    /// <summary>
    /// Node - represents a location in the network
    /// </summary>
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TransportMode { get; set; } // Road, Rail, Air
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Edge - represents a connection between two nodes
    /// </summary>
    public class Edge
    {
        public int Id { get; set; }
        public int SourceNodeId { get; set; }
        public int DestNodeNodeId { get; set; }
        public double Distance { get; set; } // km
        public double EstimatedTime { get; set; } // hours
        public double AverageSpeed { get; set; } // km/h
        public string TransportMode { get; set; } // Road, Rail, Air
        public double SafetyRating { get; set; } // 0-5
        public double ComfortRating { get; set; } // 0-5
        public int Toll { get; set; } // PKR
        public bool IsBidirectional { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
