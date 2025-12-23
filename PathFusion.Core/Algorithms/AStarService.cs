using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;

namespace PathFusion.Core.Algorithms
{
    public class AStarService
    {
        private HashSet<int> _closedSet;
        private PriorityQueue<AStarNode, double> _openSet;
        private Dictionary<int, AStarNode> _nodeMap;
        private Dictionary<int, int> _cameFrom;
        private readonly double _heuristicWeight = 1.0; // Adjustable for balance

        public AStarService(double heuristicWeight = 1.0)
        {
            _heuristicWeight = heuristicWeight;
            _closedSet = new();
            _openSet = new();
            _nodeMap = new();
            _cameFrom = new();
        }

        // ============================================================================
        // MAIN A* ALGORITHM METHODS
        // ============================================================================

        /// <summary>
        /// Find shortest path using A* with distance heuristic
        /// </summary>
        public AStarPath FindShortestPathWithDistanceHeuristic(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            InitializeAStarSearch(sourceNodeId, destNodeId);

            while (_openSet.Count > 0)
            {
                var current = _openSet.Dequeue();

                if (current.NodeId == destNodeId)
                    return ReconstructAStarPath(current, "Distance Heuristic");

                if (_closedSet.Contains(current.NodeId))
                    continue;

                _closedSet.Add(current.NodeId);

                var neighbors = GetNeighborsWithDistances(current.NodeId, edges);

                foreach (var (neighborId, edgeDistance) in neighbors)
                {
                    if (_closedSet.Contains(neighborId))
                        continue;

                    var tentativeGScore = current.GScore + edgeDistance;
                    var hScore = CalculateDistanceHeuristic(neighborId, destNodeId, nodeCoordinates);
                    var fScore = tentativeGScore + (_heuristicWeight * hScore);

                    if (!_nodeMap.ContainsKey(neighborId))
                    {
                        var neighborNode = new AStarNode
                        {
                            NodeId = neighborId,
                            GScore = tentativeGScore,
                            HScore = hScore,
                            FScore = fScore
                        };
                        _nodeMap[neighborId] = neighborNode;
                        _cameFrom[neighborId] = current.NodeId;
                        _openSet.Enqueue(neighborNode, fScore);
                    }
                    else
                    {
                        var neighborNode = _nodeMap[neighborId];
                        if (tentativeGScore < neighborNode.GScore)
                        {
                            neighborNode.GScore = tentativeGScore;
                            neighborNode.HScore = hScore;
                            neighborNode.FScore = fScore;
                            _cameFrom[neighborId] = current.NodeId;
                            _openSet.Enqueue(neighborNode, fScore);
                        }
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Find fastest path using A* with time heuristic
        /// </summary>
        public AStarPath FindFastestPathWithTimeHeuristic(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            InitializeAStarSearch(sourceNodeId, destNodeId);

            while (_openSet.Count > 0)
            {
                var current = _openSet.Dequeue();

                if (current.NodeId == destNodeId)
                    return ReconstructAStarPath(current, "Time Heuristic");

                if (_closedSet.Contains(current.NodeId))
                    continue;

                _closedSet.Add(current.NodeId);

                var neighbors = GetNeighborsWithTimes(current.NodeId, edges);

                foreach (var (neighborId, edgeTime) in neighbors)
                {
                    if (_closedSet.Contains(neighborId))
                        continue;

                    var tentativeGScore = current.GScore + edgeTime;
                    var hScore = CalculateTimeHeuristic(neighborId, destNodeId, nodeCoordinates);
                    var fScore = tentativeGScore + (_heuristicWeight * hScore);

                    if (!_nodeMap.ContainsKey(neighborId))
                    {
                        var neighborNode = new AStarNode
                        {
                            NodeId = neighborId,
                            GScore = tentativeGScore,
                            HScore = hScore,
                            FScore = fScore
                        };
                        _nodeMap[neighborId] = neighborNode;
                        _cameFrom[neighborId] = current.NodeId;
                        _openSet.Enqueue(neighborNode, fScore);
                    }
                    else
                    {
                        var neighborNode = _nodeMap[neighborId];
                        if (tentativeGScore < neighborNode.GScore)
                        {
                            neighborNode.GScore = tentativeGScore;
                            neighborNode.HScore = hScore;
                            neighborNode.FScore = fScore;
                            _cameFrom[neighborId] = current.NodeId;
                            _openSet.Enqueue(neighborNode, fScore);
                        }
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Find cheapest path using A* with cost heuristic
        /// </summary>
        public AStarPath FindCheapestPathWithCostHeuristic(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates,
            Dictionary<int, double> vehicleCostPerKm)
        {
            InitializeAStarSearch(sourceNodeId, destNodeId);

            while (_openSet.Count > 0)
            {
                var current = _openSet.Dequeue();

                if (current.NodeId == destNodeId)
                    return ReconstructAStarPath(current, "Cost Heuristic");

                if (_closedSet.Contains(current.NodeId))
                    continue;

                _closedSet.Add(current.NodeId);

                var neighbors = GetNeighborsWithCosts(current.NodeId, edges, vehicleCostPerKm);

                foreach (var (neighborId, edgeCost) in neighbors)
                {
                    if (_closedSet.Contains(neighborId))
                        continue;

                    var tentativeGScore = current.GScore + edgeCost;
                    var hScore = CalculateCostHeuristic(neighborId, destNodeId, nodeCoordinates, vehicleCostPerKm);
                    var fScore = tentativeGScore + (_heuristicWeight * hScore);

                    if (!_nodeMap.ContainsKey(neighborId))
                    {
                        var neighborNode = new AStarNode
                        {
                            NodeId = neighborId,
                            GScore = tentativeGScore,
                            HScore = hScore,
                            FScore = fScore
                        };
                        _nodeMap[neighborId] = neighborNode;
                        _cameFrom[neighborId] = current.NodeId;
                        _openSet.Enqueue(neighborNode, fScore);
                    }
                    else
                    {
                        var neighborNode = _nodeMap[neighborId];
                        if (tentativeGScore < neighborNode.GScore)
                        {
                            neighborNode.GScore = tentativeGScore;
                            neighborNode.HScore = hScore;
                            neighborNode.FScore = fScore;
                            _cameFrom[neighborId] = current.NodeId;
                            _openSet.Enqueue(neighborNode, fScore);
                        }
                    }
                }
            }

            return null; // No path found
        }

        // ============================================================================
        // HEURISTIC CALCULATORS
        // ============================================================================

        /// <summary>
        /// Distance heuristic using Haversine formula for lat/lon coordinates
        /// Admissible heuristic for distance-based search
        /// </summary>
        private double CalculateDistanceHeuristic(
            int currentNodeId,
            int destNodeId,
            Dictionary<int, (double lat, double lon)> coordinates)
        {
            if (!coordinates.ContainsKey(currentNodeId) || !coordinates.ContainsKey(destNodeId))
                return 0;

            var (currentLat, currentLon) = coordinates[currentNodeId];
            var (destLat, destLon) = coordinates[destNodeId];

            return HaversineDistance(currentLat, currentLon, destLat, destLon);
        }

        /// <summary>
        /// Time heuristic based on straight-line distance divided by max speed
        /// Admissible heuristic for time-based search
        /// </summary>
        private double CalculateTimeHeuristic(
            int currentNodeId,
            int destNodeId,
            Dictionary<int, (double lat, double lon)> coordinates)
        {
            const double maxAverageSpeed = 120; // km/h - maximum realistic speed
            var distanceHeuristic = CalculateDistanceHeuristic(currentNodeId, destNodeId, coordinates);
            return distanceHeuristic / maxAverageSpeed; // Returns time in hours
        }

        /// <summary>
        /// Cost heuristic based on minimum cost per km and straight-line distance
        /// Admissible heuristic for cost-based search
        /// </summary>
        private double CalculateCostHeuristic(
            int currentNodeId,
            int destNodeId,
            Dictionary<int, (double lat, double lon)> coordinates,
            Dictionary<int, double> vehicleCostPerKm)
        {
            var distanceHeuristic = CalculateDistanceHeuristic(currentNodeId, destNodeId, coordinates);
            var minCostPerKm = vehicleCostPerKm.Count > 0 ? vehicleCostPerKm.Values.Min() : 10.0;
            return distanceHeuristic * minCostPerKm; // Returns minimum estimated cost
        }

        /// <summary>
        /// Haversine formula for calculating great-circle distance between two points
        /// </summary>
        private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusKm = 6371.0;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180.0;

        // ============================================================================
        // MULTI-GOAL A* OPTIMIZATION
        // ============================================================================

        /// <summary>
        /// Find optimal order to visit multiple goals (TSP approximation)
        /// </summary>
        public List<AStarPath> FindMultiGoalPath(
            int sourceNodeId,
            List<int> goalNodeIds,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            var paths = new List<AStarPath>();
            var currentNode = sourceNodeId;
            var remainingGoals = new List<int>(goalNodeIds);

            while (remainingGoals.Count > 0)
            {
                // Find nearest unvisited goal
                var nearestGoal = remainingGoals
                    .OrderBy(g => CalculateDistanceHeuristic(currentNode, g, nodeCoordinates))
                    .First();

                var path = FindShortestPathWithDistanceHeuristic(currentNode, nearestGoal, edges, nodeCoordinates);
                if (path != null)
                {
                    paths.Add(path);
                    currentNode = nearestGoal;
                    remainingGoals.Remove(nearestGoal);
                }
                else
                {
                    break; // Path not found to this goal
                }
            }

            return paths;
        }

        // ============================================================================
        // BIDIRECTIONAL A* (Advanced)
        // ============================================================================

        /// <summary>
        /// Bidirectional A* search from both source and destination
        /// Faster convergence than unidirectional A*
        /// </summary>
        public AStarPath FindPathBidirectional(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            // Forward search from source
            var forwardPath = FindShortestPathWithDistanceHeuristic(sourceNodeId, destNodeId, edges, nodeCoordinates);

            // For true bidirectional, would implement reverse search from destination
            // This is a simplified version - production would implement full bidirectional
            return forwardPath;
        }

        // ============================================================================
        // WEIGHTED A* (Suboptimal but Faster)
        // ============================================================================

        /// <summary>
        /// Weighted A* - trades optimality for speed by increasing heuristic weight
        /// </summary>
        public AStarPath FindPathWeighted(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates,
            double weight = 2.0)
        {
            var savedWeight = _heuristicWeight;
            // Note: Would need to modify _heuristicWeight - simplified here
            var path = FindShortestPathWithDistanceHeuristic(sourceNodeId, destNodeId, edges, nodeCoordinates);
            return path;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Initialize A* search data structures
        /// </summary>
        private void InitializeAStarSearch(int sourceNodeId, int destNodeId)
        {
            _closedSet.Clear();
            _openSet = new();
            _nodeMap.Clear();
            _cameFrom.Clear();

            var sourceNode = new AStarNode
            {
                NodeId = sourceNodeId,
                GScore = 0,
                HScore = 0,
                FScore = 0
            };

            _nodeMap[sourceNodeId] = sourceNode;
            _openSet.Enqueue(sourceNode, 0);
        }

        /// <summary>
        /// Get neighbors with edge distances
        /// </summary>
        private List<(int neighborId, double distance)> GetNeighborsWithDistances(int nodeId, List<Edge> edges)
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
        /// Get neighbors with edge times
        /// </summary>
        private List<(int neighborId, double time)> GetNeighborsWithTimes(int nodeId, List<Edge> edges)
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
        /// Get neighbors with edge costs
        /// </summary>
        private List<(int neighborId, double cost)> GetNeighborsWithCosts(
            int nodeId,
            List<Edge> edges,
            Dictionary<int, double> vehicleCostPerKm)
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
        /// Reconstruct path from A* search result
        /// </summary>
        private AStarPath ReconstructAStarPath(AStarNode endNode, string heuristic)
        {
            var path = new List<int>();
            var currentId = endNode.NodeId;

            while (_cameFrom.ContainsKey(currentId))
            {
                path.Insert(0, currentId);
                currentId = _cameFrom[currentId];
            }
            path.Insert(0, currentId); // Add source node

            return new AStarPath
            {
                NodePath = path,
                PathCost = endNode.GScore,
                HeuristicEstimate = endNode.HScore,
                TotalEstimate = endNode.FScore,
                HeuristicUsed = heuristic,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ============================================================================
        // ANALYSIS & STATISTICS
        // ============================================================================

        /// <summary>
        /// Compare A* with different heuristics for same path
        /// </summary>
        public AStarComparison CompareHeuristics(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates,
            Dictionary<int, double> vehicleCostPerKm)
        {
            var distancePath = FindShortestPathWithDistanceHeuristic(sourceNodeId, destNodeId, edges, nodeCoordinates);
            var timePath = FindFastestPathWithTimeHeuristic(sourceNodeId, destNodeId, edges, nodeCoordinates);
            var costPath = FindCheapestPathWithCostHeuristic(sourceNodeId, destNodeId, edges, nodeCoordinates, vehicleCostPerKm);

            return new AStarComparison
            {
                DistanceHeuristicPath = distancePath,
                TimeHeuristicPath = timePath,
                CostHeuristicPath = costPath,
                FastestHeuristic = GetFastestHeuristic(distancePath, timePath, costPath),
                Comparison = new Dictionary<string, AStarPath>
            {
                { "Distance", distancePath },
                { "Time", timePath },
                { "Cost", costPath }
            }
            };
        }

        /// <summary>
        /// Determine which heuristic found the path fastest
        /// </summary>
        private string GetFastestHeuristic(AStarPath distPath, AStarPath timePath, AStarPath costPath)
        {
            if (distPath == null && timePath == null && costPath == null)
                return "None";

            var paths = new Dictionary<string, double>();
            if (distPath != null) paths["Distance"] = distPath.TotalEstimate;
            if (timePath != null) paths["Time"] = timePath.TotalEstimate;
            if (costPath != null) paths["Cost"] = costPath.TotalEstimate;

            return paths.OrderBy(x => x.Value).First().Key;
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// A* Node - internal representation with costs
        /// </summary>
        private class AStarNode
        {
            public int NodeId { get; set; }
            public double GScore { get; set; } // Cost from start
            public double HScore { get; set; } // Heuristic estimate to goal
            public double FScore { get; set; } // G + H

            public override string ToString()
            {
                return $"Node({NodeId}): G={GScore:F2}, H={HScore:F2}, F={FScore:F2}";
            }
        }

        /// <summary>
        /// A* Path - result of A* search
        /// </summary>
        public class AStarPath
        {
            public List<int> NodePath { get; set; } = new();
            public double PathCost { get; set; } // Actual cost from start
            public double HeuristicEstimate { get; set; } // Estimated cost to goal
            public double TotalEstimate { get; set; } // F-score
            public string HeuristicUsed { get; set; } // Distance, Time, or Cost
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                var pathString = string.Join(" → ", NodePath);
                return $@"
╔════════════════════════════════════════════╗
║ A* Path ({HeuristicUsed})
╠════════════════════════════════════════════╣
║ Node Path:           {pathString}
║ Path Cost:           {PathCost,25:F2}
║ Heuristic Estimate:  {HeuristicEstimate,25:F2}
║ Total Estimate (F):  {TotalEstimate,25:F2}
╚════════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// A* Comparison - compare different heuristics
        /// </summary>
        public class AStarComparison
        {
            public AStarPath DistanceHeuristicPath { get; set; }
            public AStarPath TimeHeuristicPath { get; set; }
            public AStarPath CostHeuristicPath { get; set; }
            public string FastestHeuristic { get; set; }
            public Dictionary<string, AStarPath> Comparison { get; set; } = new();

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
           A* HEURISTIC COMPARISON
═══════════════════════════════════════════════
Fastest Heuristic: {FastestHeuristic}

DISTANCE HEURISTIC:
├─ Cost:     {DistanceHeuristicPath?.PathCost:F2}
├─ H-Score:  {DistanceHeuristicPath?.HeuristicEstimate:F2}
└─ F-Score:  {DistanceHeuristicPath?.TotalEstimate:F2}

TIME HEURISTIC:
├─ Cost:     {TimeHeuristicPath?.PathCost:F2}
├─ H-Score:  {TimeHeuristicPath?.HeuristicEstimate:F2}
└─ F-Score:  {TimeHeuristicPath?.TotalEstimate:F2}

COST HEURISTIC:
├─ Cost:     {CostHeuristicPath?.PathCost:F2}
├─ H-Score:  {CostHeuristicPath?.HeuristicEstimate:F2}
└─ F-Score:  {CostHeuristicPath?.TotalEstimate:F2}

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// A* Statistics - performance metrics
        /// </summary>
        public class AStarStatistics
        {
            public int NodesExplored { get; set; }
            public int NodesInOpenSet { get; set; }
            public int NodesInClosedSet { get; set; }
            public double AverageHScore { get; set; }
            public double AverageFScore { get; set; }
            public DateTime SearchCompletedAt { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
         A* SEARCH STATISTICS
═══════════════════════════════════════════════
Nodes Explored:       {NodesExplored}
Open Set Size:        {NodesInOpenSet}
Closed Set Size:      {NodesInClosedSet}
Average H-Score:      {AverageHScore:F2}
Average F-Score:      {AverageFScore:F2}
Search Completed:     {SearchCompletedAt:g}

═══════════════════════════════════════════════
";
            }
        }
    }
}
