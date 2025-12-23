using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;

namespace PathFusion.Core.Algorithms
{
    public class DijkstraService
    {
        private readonly Dictionary<int, DijkstraNode> _nodeMap;
        private readonly PriorityQueue<DijkstraNode, double> _priorityQueue;

        public DijkstraService()
        {
            _nodeMap = new();
            _priorityQueue = new();
        }

        // ============================================================================
        // MAIN DIJKSTRA ALGORITHM METHODS
        // ============================================================================

        /// <summary>
        /// Find shortest path by distance using Dijkstra's algorithm
        /// </summary>
        public DijkstraPath FindShortestPathByDistance(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges)
        {
            InitializeGraph(sourceNodeId, edges);

            while (_priorityQueue.Count > 0)
            {
                var current = _priorityQueue.Dequeue();

                if (current.Id == destNodeId)
                    return ReconstructPath(current, "Distance");

                if (current.IsVisited)
                    continue;

                current.IsVisited = true;

                // Get neighbors of current node
                var neighbors = GetNeighbors(current.Id, edges);

                foreach (var (neighborId, distance) in neighbors)
                {
                    if (!_nodeMap.ContainsKey(neighborId))
                        continue;

                    var neighbor = _nodeMap[neighborId];
                    if (neighbor.IsVisited)
                        continue;

                    var newDistance = current.Distance + distance;

                    if (newDistance < neighbor.Distance)
                    {
                        neighbor.Distance = newDistance;
                        neighbor.PreviousNodeId = current.Id;
                        neighbor.Hops = current.Hops + 1;
                        _priorityQueue.Enqueue(neighbor, newDistance);
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Find fastest path by travel time using Dijkstra's algorithm
        /// </summary>
        public DijkstraPath FindFastestPathByTime(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges)
        {
            InitializeGraphByTime(sourceNodeId, edges);

            while (_priorityQueue.Count > 0)
            {
                var current = _priorityQueue.Dequeue();

                if (current.Id == destNodeId)
                    return ReconstructPath(current, "Time");

                if (current.IsVisited)
                    continue;

                current.IsVisited = true;

                var neighbors = GetNeighborsWithTime(current.Id, edges);

                foreach (var (neighborId, time) in neighbors)
                {
                    if (!_nodeMap.ContainsKey(neighborId))
                        continue;

                    var neighbor = _nodeMap[neighborId];
                    if (neighbor.IsVisited)
                        continue;

                    var newTime = current.Time + time;

                    if (newTime < neighbor.Time)
                    {
                        neighbor.Time = newTime;
                        neighbor.PreviousNodeId = current.Id;
                        neighbor.Hops = current.Hops + 1;
                        _priorityQueue.Enqueue(neighbor, newTime);
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Find cheapest path by cost using Dijkstra's algorithm
        /// </summary>
        public DijkstraPath FindCheapestPathByCost(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, double> vehicleCostPerKm)
        {
            InitializeGraphByCost(sourceNodeId, edges, vehicleCostPerKm);

            while (_priorityQueue.Count > 0)
            {
                var current = _priorityQueue.Dequeue();

                if (current.Id == destNodeId)
                    return ReconstructPath(current, "Cost");

                if (current.IsVisited)
                    continue;

                current.IsVisited = true;

                var neighbors = GetNeighborsWithCost(current.Id, edges, vehicleCostPerKm);

                foreach (var (neighborId, cost) in neighbors)
                {
                    if (!_nodeMap.ContainsKey(neighborId))
                        continue;

                    var neighbor = _nodeMap[neighborId];
                    if (neighbor.IsVisited)
                        continue;

                    var newCost = current.Cost + cost;

                    if (newCost < neighbor.Cost)
                    {
                        neighbor.Cost = newCost;
                        neighbor.PreviousNodeId = current.Id;
                        neighbor.Hops = current.Hops + 1;
                        _priorityQueue.Enqueue(neighbor, newCost);
                    }
                }
            }

            return null; // No path found
        }

        // ============================================================================
        // DISTANCE OPTIMIZER - PRIMARY METRIC
        // ============================================================================

        /// <summary>
        /// Distance Optimizer: Find shortest distance paths
        /// </summary>
        public class DistanceOptimizer
        {
            private readonly DijkstraService _service;

            public DistanceOptimizer(DijkstraService service)
            {
                _service = service;
            }

            /// <summary>
            /// Find shortest distance path
            /// </summary>
            public DijkstraPath FindShortest(int source, int dest, List<Edge> edges)
            {
                return _service.FindShortestPathByDistance(source, dest, edges);
            }

            /// <summary>
            /// Find top N shortest paths
            /// </summary>
            public List<DijkstraPath> FindTopNShortest(int source, int dest, List<Edge> edges, int count = 3)
            {
                var paths = new List<DijkstraPath>();
                var shortestPath = FindShortest(source, dest, edges);

                if (shortestPath == null)
                    return paths;

                paths.Add(shortestPath);

                // Find alternative paths by blocking nodes
                for (int i = 1; i < count; i++)
                {
                    var alternativeEdges = DijkstraService.BlockNodeInPath(edges, shortestPath.Path, i);
                    var altPath = _service.FindShortestPathByDistance(source, dest, alternativeEdges);

                    if (altPath != null)
                        paths.Add(altPath);
                    else
                        break;
                }

                return paths;
            }

            /// <summary>
            /// Get distance range analysis
            /// </summary>
            public DistanceAnalysis AnalyzeDistanceOptions(int source, int dest, List<Edge> edges)
            {
                var paths = FindTopNShortest(source, dest, edges, 5);

                if (paths.Count == 0)
                    return null;

                return new DistanceAnalysis
                {
                    ShortestDistance = paths.First().TotalDistance,
                    LongestDistance = paths.Last().TotalDistance,
                    AverageDistance = paths.Average(p => p.TotalDistance),
                    TotalPathsFound = paths.Count,
                    Paths = paths
                };
            }
        }

        // ============================================================================
        // TIME OPTIMIZER - SPEED METRIC
        // ============================================================================

        /// <summary>
        /// Time Optimizer: Find fastest routes
        /// </summary>
        public class TimeOptimizer
        {
            private readonly DijkstraService _service;

            public TimeOptimizer(DijkstraService service)
            {
                _service = service;
            }

            /// <summary>
            /// Find fastest path by travel time
            /// </summary>
            public DijkstraPath FindFastest(int source, int dest, List<Edge> edges)
            {
                return _service.FindFastestPathByTime(source, dest, edges);
            }

            /// <summary>
            /// Find top N fastest paths
            /// </summary>
            public List<DijkstraPath> FindTopNFastest(int source, int dest, List<Edge> edges, int count = 3)
            {
                var paths = new List<DijkstraPath>();
                var fastestPath = FindFastest(source, dest, edges);

                if (fastestPath == null)
                    return paths;

                paths.Add(fastestPath);

                // Find alternative paths
                for (int i = 1; i < count; i++)
                {
                    var alternativeEdges = DijkstraService.BlockNodeInPath(edges, fastestPath.Path, i);
                    var altPath = _service.FindFastestPathByTime(source, dest, alternativeEdges);

                    if (altPath != null)
                        paths.Add(altPath);
                    else
                        break;
                }

                return paths;
            }

            /// <summary>
            /// Get time range analysis
            /// </summary>
            public TimeAnalysis AnalyzeTimeOptions(int source, int dest, List<Edge> edges)
            {
                var paths = FindTopNFastest(source, dest, edges, 5);

                if (paths.Count == 0)
                    return null;

                return new TimeAnalysis
                {
                    FastestTime = paths.First().TravelTime,
                    SlowestTime = paths.Last().TravelTime,
                    AverageTime = paths.Average(p => p.TravelTime),
                    TimeSaved = paths.Last().TravelTime - paths.First().TravelTime,
                    TotalPathsFound = paths.Count,
                    Paths = paths
                };
            }
        }

        // ============================================================================
        // COST OPTIMIZER - PRICE METRIC
        // ============================================================================

        /// <summary>
        /// Cost Optimizer: Find cheapest routes
        /// </summary>
        public class CostOptimizer
        {
            private readonly DijkstraService _service;

            public CostOptimizer(DijkstraService service)
            {
                _service = service;
            }

            /// <summary>
            /// Find cheapest path by cost
            /// </summary>
            public DijkstraPath FindCheapest(int source, int dest, List<Edge> edges, Dictionary<int, double> vehicleCostPerKm)
            {
                return _service.FindCheapestPathByCost(source, dest, edges, vehicleCostPerKm);
            }

            /// <summary>
            /// Find top N cheapest paths
            /// </summary>
            public List<DijkstraPath> FindTopNCheapest(int source, int dest, List<Edge> edges, Dictionary<int, double> vehicleCostPerKm, int count = 3)
            {
                var paths = new List<DijkstraPath>();
                var cheapestPath = FindCheapest(source, dest, edges, vehicleCostPerKm);

                if (cheapestPath == null)
                    return paths;

                paths.Add(cheapestPath);

                for (int i = 1; i < count; i++)
                {
                    var alternativeEdges = DijkstraService.BlockNodeInPath(edges, cheapestPath.Path, i);
                    var altPath = _service.FindCheapestPathByCost(source, dest, alternativeEdges, vehicleCostPerKm);

                    if (altPath != null)
                        paths.Add(altPath);
                    else
                        break;
                }

                return paths;
            }

            /// <summary>
            /// Get cost range analysis
            /// </summary>
            public CostAnalysis AnalyzeCostOptions(int source, int dest, List<Edge> edges, Dictionary<int, double> vehicleCostPerKm)
            {
                var paths = FindTopNCheapest(source, dest, edges, vehicleCostPerKm, 5);

                if (paths.Count == 0)
                    return null;

                return new CostAnalysis
                {
                    CheapestCost = paths.First().EstimatedCost,
                    MostExpensiveCost = paths.Last().EstimatedCost,
                    AverageCost = paths.Average(p => p.EstimatedCost),
                    MoneySaved = paths.Last().EstimatedCost - paths.First().EstimatedCost,
                    TotalPathsFound = paths.Count,
                    Paths = paths
                };
            }
        }

        // ============================================================================
        // MULTI-CRITERIA OPTIMIZATION
        // ============================================================================

        /// <summary>
        /// Find balanced path considering distance, time, and cost
        /// </summary>
        public DijkstraPath FindBalancedPath(
            int source,
            int dest,
            List<Edge> edges,
            Dictionary<int, double> vehicleCostPerKm,
            double distanceWeight = 0.3,
            double timeWeight = 0.3,
            double costWeight = 0.4)
        {
            var shortestPath = FindShortestPathByDistance(source, dest, edges);
            var fastestPath = FindFastestPathByTime(source, dest, edges);
            var cheapestPath = FindCheapestPathByCost(source, dest, edges, vehicleCostPerKm);

            if (shortestPath == null || fastestPath == null || cheapestPath == null)
                return shortestPath ?? fastestPath ?? cheapestPath;

            // Normalize metrics
            var maxDistance = Math.Max(shortestPath.TotalDistance, Math.Max(fastestPath.TotalDistance, cheapestPath.TotalDistance));
            var maxTime = Math.Max(shortestPath.TravelTime, Math.Max(fastestPath.TravelTime, cheapestPath.TravelTime));
            var maxCost = Math.Max(shortestPath.EstimatedCost, Math.Max(fastestPath.EstimatedCost, cheapestPath.EstimatedCost));

            var shortestScore = (shortestPath.TotalDistance / maxDistance * distanceWeight) +
                               (shortestPath.TravelTime / maxTime * timeWeight) +
                               (shortestPath.EstimatedCost / maxCost * costWeight);

            var fastestScore = (fastestPath.TotalDistance / maxDistance * distanceWeight) +
                              (fastestPath.TravelTime / maxTime * timeWeight) +
                              (fastestPath.EstimatedCost / maxCost * costWeight);

            var cheapestScore = (cheapestPath.TotalDistance / maxDistance * distanceWeight) +
                               (cheapestPath.TravelTime / maxTime * timeWeight) +
                               (cheapestPath.EstimatedCost / maxCost * costWeight);

            if (shortestScore <= fastestScore && shortestScore <= cheapestScore)
                return shortestPath;
            else if (fastestScore <= cheapestScore)
                return fastestPath;
            else
                return cheapestPath;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Initialize graph for distance-based Dijkstra
        /// </summary>
        private void InitializeGraph(int sourceNodeId, List<Edge> edges)
        {
            _nodeMap.Clear();

            // Get all unique node IDs
            var nodeIds = new HashSet<int>();
            foreach (var edge in edges)
            {
                nodeIds.Add(edge.SourceNodeId);
                nodeIds.Add(edge.DestNodeNodeId);
            }

            // Initialize all nodes
            foreach (var nodeId in nodeIds)
            {
                var node = new DijkstraNode
                {
                    Id = nodeId,
                    Distance = nodeId == sourceNodeId ? 0 : double.MaxValue,
                    Hops = nodeId == sourceNodeId ? 0 : int.MaxValue
                };
                _nodeMap[nodeId] = node;
            }

            // Add source to priority queue
            var source = _nodeMap[sourceNodeId];
            _priorityQueue.Enqueue(source, 0);
        }

        /// <summary>
        /// Initialize graph for time-based Dijkstra
        /// </summary>
        private void InitializeGraphByTime(int sourceNodeId, List<Edge> edges)
        {
            _nodeMap.Clear();

            var nodeIds = new HashSet<int>();
            foreach (var edge in edges)
            {
                nodeIds.Add(edge.SourceNodeId);
                nodeIds.Add(edge.DestNodeNodeId);
            }

            foreach (var nodeId in nodeIds)
            {
                var node = new DijkstraNode
                {
                    Id = nodeId,
                    Time = nodeId == sourceNodeId ? 0 : double.MaxValue,
                    Hops = nodeId == sourceNodeId ? 0 : int.MaxValue
                };
                _nodeMap[nodeId] = node;
            }

            var source = _nodeMap[sourceNodeId];
            _priorityQueue.Enqueue(source, 0);
        }

        /// <summary>
        /// Initialize graph for cost-based Dijkstra
        /// </summary>
        private void InitializeGraphByCost(int sourceNodeId, List<Edge> edges, Dictionary<int, double> vehicleCostPerKm)
        {
            _nodeMap.Clear();

            var nodeIds = new HashSet<int>();
            foreach (var edge in edges)
            {
                nodeIds.Add(edge.SourceNodeId);
                nodeIds.Add(edge.DestNodeNodeId);
            }

            foreach (var nodeId in nodeIds)
            {
                var node = new DijkstraNode
                {
                    Id = nodeId,
                    Cost = nodeId == sourceNodeId ? 0 : double.MaxValue,
                    Hops = nodeId == sourceNodeId ? 0 : int.MaxValue
                };
                _nodeMap[nodeId] = node;
            }

            var source = _nodeMap[sourceNodeId];
            _priorityQueue.Enqueue(source, 0);
        }

        /// <summary>
        /// Get neighbors with distances
        /// </summary>
        private List<(int neighborId, double distance)> GetNeighbors(int nodeId, List<Edge> edges)
        {
            var neighbors = new List<(int, double)>();

            foreach (var edge in edges)
            {
                if (edge.SourceNodeId == nodeId)
                    neighbors.Add((edge.DestNodeNodeId, edge.Distance));
                else if (edge.DestNodeNodeId == nodeId)
                    neighbors.Add((edge.SourceNodeId, edge.Distance));
            }

            return neighbors;
        }

        /// <summary>
        /// Get neighbors with travel times
        /// </summary>
        private List<(int neighborId, double time)> GetNeighborsWithTime(int nodeId, List<Edge> edges)
        {
            var neighbors = new List<(int, double)>();

            foreach (var edge in edges)
            {
                if (edge.SourceNodeId == nodeId)
                    neighbors.Add((edge.DestNodeNodeId, edge.EstimatedTime));
                else if (edge.DestNodeNodeId == nodeId)
                    neighbors.Add((edge.SourceNodeId, edge.EstimatedTime));
            }

            return neighbors;
        }

        /// <summary>
        /// Get neighbors with costs
        /// </summary>
        private List<(int neighborId, double cost)> GetNeighborsWithCost(int nodeId, List<Edge> edges, Dictionary<int, double> vehicleCostPerKm)
        {
            var neighbors = new List<(int, double)>();

            foreach (var edge in edges)
            {
                var neighborId = -1;
                if (edge.SourceNodeId == nodeId)
                    neighborId = edge.DestNodeNodeId;
                else if (edge.DestNodeNodeId == nodeId)
                    neighborId = edge.SourceNodeId;

                if (neighborId != -1 && vehicleCostPerKm.ContainsKey(neighborId))
                {
                    var cost = vehicleCostPerKm[neighborId] * edge.Distance;
                    neighbors.Add((neighborId, cost));
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Reconstruct path from Dijkstra result
        /// </summary>
        private DijkstraPath ReconstructPath(DijkstraNode endNode, string optimizedBy)
        {
            var path = new List<int>();
            var current = endNode;

            while (current != null)
            {
                path.Insert(0, current.Id);

                if (current.PreviousNodeId == -1)
                    break;

                current = _nodeMap.ContainsKey(current.PreviousNodeId) ? _nodeMap[current.PreviousNodeId] : null;
            }

            return new DijkstraPath
            {
                NodePath = path,
                TotalDistance = endNode.Distance,
                TravelTime = endNode.Time,
                EstimatedCost = endNode.Cost,
                Hops = endNode.Hops,
                OptimizedBy = optimizedBy,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Block specific node in path and return modified edges
        /// FIX: Now STATIC so it can be called from inner classes
        /// </summary>
        private static List<Edge> BlockNodeInPath(List<Edge> edges, List<int> pathNodes, int blockIndex)
        {
            if (blockIndex >= pathNodes.Count - 1)
                return edges;

            var nodeToBlock = pathNodes[blockIndex];
            return edges.Where(e => e.SourceNodeId != nodeToBlock && e.DestNodeNodeId != nodeToBlock).ToList();
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// Dijkstra Node - internal representation
        /// </summary>
        private class DijkstraNode
        {
            public int Id { get; set; }
            public double Distance { get; set; } = double.MaxValue;
            public double Time { get; set; } = double.MaxValue;
            public double Cost { get; set; } = double.MaxValue;
            public int Hops { get; set; } = int.MaxValue;
            public int PreviousNodeId { get; set; } = -1;
            public bool IsVisited { get; set; } = false;
        }

        /// <summary>
        /// Dijkstra Path - result of pathfinding
        /// </summary>
        public class DijkstraPath
        {
            public List<int> NodePath { get; set; } = new();
            public double TotalDistance { get; set; }
            public double TravelTime { get; set; }
            public double EstimatedCost { get; set; }
            public int Hops { get; set; }
            public string OptimizedBy { get; set; } // Distance, Time, Cost
            public DateTime CreatedAt { get; set; }

            public List<int> Path => NodePath;

            public override string ToString()
            {
                var pathString = string.Join(" → ", NodePath);
                return $@"
╔════════════════════════════════════════════╗
║ Dijkstra Path ({OptimizedBy}-Optimized)
╠════════════════════════════════════════════╣
║ Node Path:         {pathString}
║ Total Distance:    {TotalDistance,20:F2} km
║ Travel Time:       {TravelTime,20:F2} hours
║ Estimated Cost:    {EstimatedCost,20:F0} PKR
║ Number of Hops:    {Hops,20}
╚════════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// Distance Analysis - analysis of distance optimization
        /// </summary>
        public class DistanceAnalysis
        {
            public double ShortestDistance { get; set; }
            public double LongestDistance { get; set; }
            public double AverageDistance { get; set; }
            public int TotalPathsFound { get; set; }
            public List<DijkstraPath> Paths { get; set; } = new();

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
         DISTANCE ANALYSIS REPORT
═══════════════════════════════════════════════
Shortest Distance:     {ShortestDistance:F2} km
Longest Distance:      {LongestDistance:F2} km
Average Distance:      {AverageDistance:F2} km
Total Paths Found:     {TotalPathsFound}

DISTANCE RANGE:
├─ Min:    {ShortestDistance:F2} km
├─ Max:    {LongestDistance:F2} km
└─ Avg:    {AverageDistance:F2} km

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// Time Analysis - analysis of time optimization
        /// </summary>
        public class TimeAnalysis
        {
            public double FastestTime { get; set; }
            public double SlowestTime { get; set; }
            public double AverageTime { get; set; }
            public double TimeSaved { get; set; }
            public int TotalPathsFound { get; set; }
            public List<DijkstraPath> Paths { get; set; } = new();

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
           TIME ANALYSIS REPORT
═══════════════════════════════════════════════
Fastest Time:          {FastestTime:F2} hours
Slowest Time:          {SlowestTime:F2} hours
Average Time:          {AverageTime:F2} hours
Time Saved:            {TimeSaved:F2} hours
Total Paths Found:     {TotalPathsFound}

TIME RANGE:
├─ Min:    {FastestTime:F2} hours
├─ Max:    {SlowestTime:F2} hours
└─ Avg:    {AverageTime:F2} hours

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// Cost Analysis - analysis of cost optimization
        /// </summary>
        public class CostAnalysis
        {
            public double CheapestCost { get; set; }
            public double MostExpensiveCost { get; set; }
            public double AverageCost { get; set; }
            public double MoneySaved { get; set; }
            public int TotalPathsFound { get; set; }
            public List<DijkstraPath> Paths { get; set; } = new();

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
           COST ANALYSIS REPORT
═══════════════════════════════════════════════
Cheapest Cost:         {CheapestCost:F0} PKR
Most Expensive Cost:   {MostExpensiveCost:F0} PKR
Average Cost:          {AverageCost:F0} PKR
Money Saved:           {MoneySaved:F0} PKR
Total Paths Found:     {TotalPathsFound}

COST RANGE:
├─ Min:    {CheapestCost:F0} PKR
├─ Max:    {MostExpensiveCost:F0} PKR
└─ Avg:    {AverageCost:F0} PKR

═══════════════════════════════════════════════
";
            }
        }
    }
}
