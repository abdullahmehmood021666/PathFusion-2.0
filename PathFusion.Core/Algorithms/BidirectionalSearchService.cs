using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;

namespace PathFusion.Core.Algorithms
{
    public class BidirectionalSearchService
    {
        private Dictionary<int, BidirectionalNode> _forwardMap;
        private Dictionary<int, BidirectionalNode> _backwardMap;
        private PriorityQueue<BidirectionalNode, double> _forwardQueue;
        private PriorityQueue<BidirectionalNode, double> _backwardQueue;
        private int _meetingPoint = -1;
        private double _bestPathCost = double.MaxValue;

        public BidirectionalSearchService()
        {
            _forwardMap = new();
            _backwardMap = new();
            _forwardQueue = new();
            _backwardQueue = new();
        }

        // ============================================================================
        // BIDIRECTIONAL DIJKSTRA
        // ============================================================================

        /// <summary>
        /// Bidirectional Dijkstra's algorithm - search from both source and destination
        /// Converges faster than unidirectional Dijkstra
        /// </summary>
        public BidirectionalPath FindPathBidirectionalDijkstra(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges)
        {
            InitializeBidirectionalSearch(sourceNodeId, destNodeId);

            while (_forwardQueue.Count > 0 || _backwardQueue.Count > 0)
            {
                // Expand forward frontier
                if (_forwardQueue.Count > 0)
                {
                    var forwardCost = _forwardQueue.Count > 0 ?
                        _forwardQueue.Peek().Distance : double.MaxValue;
                    var backwardCost = _backwardQueue.Count > 0 ?
                        _backwardQueue.Peek().Distance : double.MaxValue;

                    if (forwardCost <= backwardCost)
                    {
                        if (!ExpandBidirectionalFrontier(_forwardQueue, _forwardMap, _backwardMap, edges, true))
                            break;
                    }
                    else
                    {
                        if (!ExpandBidirectionalFrontier(_backwardQueue, _backwardMap, _forwardMap, edges, false))
                            break;
                    }
                }
                else if (_backwardQueue.Count > 0)
                {
                    if (!ExpandBidirectionalFrontier(_backwardQueue, _backwardMap, _forwardMap, edges, false))
                        break;
                }

                if (_meetingPoint != -1 && HasBestPathBeenFound())
                    return ReconstructBidirectionalPath(_meetingPoint, sourceNodeId, destNodeId);
            }

            return null; // No path found
        }

        /// <summary>
        /// Bidirectional BFS - for unweighted graphs
        /// </summary>
        public BidirectionalPath FindPathBidirectionalBFS(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges)
        {
            InitializeBidirectionalSearch(sourceNodeId, destNodeId);

            var forwardQueue = new Queue<int>();
            var backwardQueue = new Queue<int>();
            var forwardVisited = new HashSet<int> { sourceNodeId };
            var backwardVisited = new HashSet<int> { destNodeId };
            var forwardParent = new Dictionary<int, int> { { sourceNodeId, -1 } };
            var backwardParent = new Dictionary<int, int> { { destNodeId, -1 } };

            forwardQueue.Enqueue(sourceNodeId);
            backwardQueue.Enqueue(destNodeId);

            while (forwardQueue.Count > 0 || backwardQueue.Count > 0)
            {
                // Expand forward
                if (forwardQueue.Count > 0)
                {
                    var current = forwardQueue.Dequeue();
                    var neighbors = GetNeighbors(current, edges);

                    foreach (var neighborId in neighbors)
                    {
                        if (backwardVisited.Contains(neighborId))
                        {
                            _meetingPoint = neighborId;
                            forwardParent[neighborId] = current;
                            return ReconstructBidirectionalPath(neighborId, sourceNodeId, destNodeId, forwardParent, backwardParent);
                        }

                        if (!forwardVisited.Contains(neighborId))
                        {
                            forwardVisited.Add(neighborId);
                            forwardParent[neighborId] = current;
                            forwardQueue.Enqueue(neighborId);
                        }
                    }
                }

                // Expand backward
                if (backwardQueue.Count > 0)
                {
                    var current = backwardQueue.Dequeue();
                    var neighbors = GetNeighbors(current, edges);

                    foreach (var neighborId in neighbors)
                    {
                        if (forwardVisited.Contains(neighborId))
                        {
                            _meetingPoint = neighborId;
                            backwardParent[neighborId] = current;
                            return ReconstructBidirectionalPath(neighborId, sourceNodeId, destNodeId, forwardParent, backwardParent);
                        }

                        if (!backwardVisited.Contains(neighborId))
                        {
                            backwardVisited.Add(neighborId);
                            backwardParent[neighborId] = current;
                            backwardQueue.Enqueue(neighborId);
                        }
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Bidirectional A* with heuristic
        /// </summary>
        public BidirectionalPath FindPathBidirectionalAStar(
            int sourceNodeId,
            int destNodeId,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            InitializeBidirectionalSearch(sourceNodeId, destNodeId);

            while (_forwardQueue.Count > 0 || _backwardQueue.Count > 0)
            {
                // Alternate expansion with heuristic awareness
                if (_forwardQueue.Count > 0)
                {
                    if (!ExpandBidirectionalAStarFrontier(
                        _forwardQueue, _forwardMap, _backwardMap, edges, nodeCoordinates,
                        sourceNodeId, destNodeId, true))
                        break;
                }

                if (_backwardQueue.Count > 0)
                {
                    if (!ExpandBidirectionalAStarFrontier(
                        _backwardQueue, _backwardMap, _forwardMap, edges, nodeCoordinates,
                        sourceNodeId, destNodeId, false))
                        break;
                }

                if (_meetingPoint != -1 && HasBestPathBeenFound())
                    return ReconstructBidirectionalPath(_meetingPoint, sourceNodeId, destNodeId);
            }

            return null; // No path found
        }

        // ============================================================================
        // MULTI-DESTINATION BIDIRECTIONAL SEARCH
        // ============================================================================

        /// <summary>
        /// Find shortest paths from source to multiple destinations simultaneously
        /// </summary>
        public List<BidirectionalPath> FindPathsToMultipleDestinations(
            int sourceNodeId,
            List<int> destNodeIds,
            List<Edge> edges)
        {
            var results = new List<BidirectionalPath>();

            foreach (var destNodeId in destNodeIds)
            {
                var path = FindPathBidirectionalDijkstra(sourceNodeId, destNodeId, edges);
                if (path != null)
                    results.Add(path);
            }

            return results;
        }

        /// <summary>
        /// Find shortest paths from multiple sources to destination
        /// </summary>
        public List<BidirectionalPath> FindPathsFromMultipleSources(
            List<int> sourceNodeIds,
            int destNodeId,
            List<Edge> edges)
        {
            var results = new List<BidirectionalPath>();

            foreach (var sourceNodeId in sourceNodeIds)
            {
                var path = FindPathBidirectionalDijkstra(sourceNodeId, destNodeId, edges);
                if (path != null)
                    results.Add(path);
            }

            return results;
        }

        // ============================================================================
        // BIDIRECTIONAL FRONTIER EXPANSION
        // ============================================================================

        /// <summary>
        /// Expand one step of bidirectional Dijkstra frontier
        /// </summary>
        private bool ExpandBidirectionalFrontier(
            PriorityQueue<BidirectionalNode, double> activeQueue,
            Dictionary<int, BidirectionalNode> activeMap,
            Dictionary<int, BidirectionalNode> oppositeMap,
            List<Edge> edges,
            bool isForward)
        {
            if (activeQueue.Count == 0)
                return false;

            var current = activeQueue.Dequeue();

            if (activeMap[current.NodeId].IsVisited)
                return true;

            activeMap[current.NodeId].IsVisited = true;

            var neighbors = GetNeighborsWithDistances(current.NodeId, edges);

            foreach (var (neighborId, edgeDistance) in neighbors)
            {
                var newDistance = current.Distance + edgeDistance;

                if (!activeMap.ContainsKey(neighborId))
                {
                    var neighborNode = new BidirectionalNode
                    {
                        NodeId = neighborId,
                        Distance = newDistance,
                        ParentNodeId = current.NodeId,
                        IsForward = isForward
                    };
                    activeMap[neighborId] = neighborNode;
                    activeQueue.Enqueue(neighborNode, newDistance);
                }
                else
                {
                    var neighborNode = activeMap[neighborId];
                    if (newDistance < neighborNode.Distance)
                    {
                        neighborNode.Distance = newDistance;
                        neighborNode.ParentNodeId = current.NodeId;
                        activeQueue.Enqueue(neighborNode, newDistance);
                    }
                }

                // Check if opposite frontier has this node
                if (oppositeMap.ContainsKey(neighborId))
                {
                    var totalDistance = newDistance + oppositeMap[neighborId].Distance;
                    if (totalDistance < _bestPathCost)
                    {
                        _bestPathCost = totalDistance;
                        _meetingPoint = neighborId;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Expand bidirectional A* frontier with heuristic
        /// </summary>
        private bool ExpandBidirectionalAStarFrontier(
            PriorityQueue<BidirectionalNode, double> activeQueue,
            Dictionary<int, BidirectionalNode> activeMap,
            Dictionary<int, BidirectionalNode> oppositeMap,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates,
            int sourceNodeId,
            int destNodeId,
            bool isForward)
        {
            if (activeQueue.Count == 0)
                return false;

            var current = activeQueue.Dequeue();

            if (activeMap[current.NodeId].IsVisited)
                return true;

            activeMap[current.NodeId].IsVisited = true;

            var neighbors = GetNeighborsWithDistances(current.NodeId, edges);
            var targetNode = isForward ? destNodeId : sourceNodeId;

            foreach (var (neighborId, edgeDistance) in neighbors)
            {
                var newDistance = current.Distance + edgeDistance;
                var heuristic = CalculateHeuristic(neighborId, targetNode, nodeCoordinates);
                var fScore = newDistance + heuristic;

                if (!activeMap.ContainsKey(neighborId))
                {
                    var neighborNode = new BidirectionalNode
                    {
                        NodeId = neighborId,
                        Distance = newDistance,
                        FScore = fScore,
                        ParentNodeId = current.NodeId,
                        IsForward = isForward
                    };
                    activeMap[neighborId] = neighborNode;
                    activeQueue.Enqueue(neighborNode, fScore);
                }
                else
                {
                    var neighborNode = activeMap[neighborId];
                    if (newDistance < neighborNode.Distance)
                    {
                        neighborNode.Distance = newDistance;
                        neighborNode.FScore = fScore;
                        neighborNode.ParentNodeId = current.NodeId;
                        activeQueue.Enqueue(neighborNode, fScore);
                    }
                }

                // Check opposite frontier
                if (oppositeMap.ContainsKey(neighborId))
                {
                    var totalDistance = newDistance + oppositeMap[neighborId].Distance;
                    if (totalDistance < _bestPathCost)
                    {
                        _bestPathCost = totalDistance;
                        _meetingPoint = neighborId;
                    }
                }
            }

            return true;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Initialize bidirectional search structures
        /// </summary>
        private void InitializeBidirectionalSearch(int sourceNodeId, int destNodeId)
        {
            _forwardMap.Clear();
            _backwardMap.Clear();
            _forwardQueue = new();
            _backwardQueue = new();
            _meetingPoint = -1;
            _bestPathCost = double.MaxValue;

            var sourceNode = new BidirectionalNode
            {
                NodeId = sourceNodeId,
                Distance = 0,
                ParentNodeId = -1,
                IsForward = true
            };

            var destNode = new BidirectionalNode
            {
                NodeId = destNodeId,
                Distance = 0,
                ParentNodeId = -1,
                IsForward = false
            };

            _forwardMap[sourceNodeId] = sourceNode;
            _backwardMap[destNodeId] = destNode;
            _forwardQueue.Enqueue(sourceNode, 0);
            _backwardQueue.Enqueue(destNode, 0);
        }

        /// <summary>
        /// Get neighbors of node
        /// </summary>
        private List<int> GetNeighbors(int nodeId, List<Edge> edges)
        {
            var neighbors = new List<int>();

            foreach (var edge in edges)
            {
                if (edge.SourceNodeId == nodeId)
                    neighbors.Add(edge.DestNodeNodeId);
                else if (edge.DestNodeNodeId == nodeId)
                    neighbors.Add(edge.SourceNodeId);
            }

            return neighbors;
        }

        /// <summary>
        /// Get neighbors with distances
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
        /// Calculate heuristic distance for A*
        /// </summary>
        private double CalculateHeuristic(
            int currentNodeId,
            int targetNodeId,
            Dictionary<int, (double lat, double lon)> coordinates)
        {
            if (!coordinates.ContainsKey(currentNodeId) || !coordinates.ContainsKey(targetNodeId))
                return 0;

            var (currentLat, currentLon) = coordinates[currentNodeId];
            var (targetLat, targetLon) = coordinates[targetNodeId];

            return HaversineDistance(currentLat, currentLon, targetLat, targetLon);
        }

        /// <summary>
        /// Haversine distance calculation
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

        /// <summary>
        /// Check if best path has been found
        /// </summary>
        private bool HasBestPathBeenFound()
        {
            if (_meetingPoint == -1)
                return false;

            var forwardMin = _forwardQueue.Count > 0 ? _forwardQueue.Peek().Distance : double.MaxValue;
            var backwardMin = _backwardQueue.Count > 0 ? _backwardQueue.Peek().Distance : double.MaxValue;

            return forwardMin + backwardMin >= _bestPathCost;
        }

        /// <summary>
        /// Reconstruct path from bidirectional search (Dijkstra/BFS)
        /// </summary>
        private BidirectionalPath ReconstructBidirectionalPath(
            int meetingPoint,
            int sourceNodeId,
            int destNodeId)
        {
            var forwardPath = new List<int>();
            var backwardPath = new List<int>();

            // Reconstruct forward path
            var currentForward = meetingPoint;
            while (currentForward != -1)
            {
                forwardPath.Insert(0, currentForward);
                if (!_forwardMap.ContainsKey(currentForward))
                    break;
                currentForward = _forwardMap[currentForward].ParentNodeId;
            }

            // Reconstruct backward path
            var currentBackward = meetingPoint;
            while (currentBackward != -1)
            {
                if (currentBackward != meetingPoint)
                    backwardPath.Add(currentBackward);
                if (!_backwardMap.ContainsKey(currentBackward))
                    break;
                currentBackward = _backwardMap[currentBackward].ParentNodeId;
            }

            // Combine paths
            var completePath = new List<int>(forwardPath);
            completePath.AddRange(backwardPath);

            var totalCost = (_forwardMap.ContainsKey(meetingPoint) ? _forwardMap[meetingPoint].Distance : 0) +
                           (_backwardMap.ContainsKey(meetingPoint) ? _backwardMap[meetingPoint].Distance : 0);

            return new BidirectionalPath
            {
                NodePath = completePath,
                MeetingPoint = meetingPoint,
                PathCost = totalCost,
                SearchType = "Dijkstra",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Reconstruct path from BFS (with parent maps)
        /// </summary>
        private BidirectionalPath ReconstructBidirectionalPath(
            int meetingPoint,
            int sourceNodeId,
            int destNodeId,
            Dictionary<int, int> forwardParent,
            Dictionary<int, int> backwardParent)
        {
            var forwardPath = new List<int>();
            var backwardPath = new List<int>();

            // Forward path from source to meeting point
            var current = meetingPoint;
            while (current != -1)
            {
                forwardPath.Insert(0, current);
                if (!forwardParent.ContainsKey(current))
                    break;
                current = forwardParent[current];
                if (current == -1)
                    break;
            }

            // Backward path from meeting point to destination
            current = meetingPoint;
            while (current != -1)
            {
                if (current != meetingPoint)
                    backwardPath.Add(current);
                if (!backwardParent.ContainsKey(current))
                    break;
                current = backwardParent[current];
                if (current == -1)
                    break;
            }

            var completePath = new List<int>(forwardPath);
            completePath.AddRange(backwardPath);

            return new BidirectionalPath
            {
                NodePath = completePath,
                MeetingPoint = meetingPoint,
                PathCost = completePath.Count - 1, // Hop count for BFS
                SearchType = "BFS",
                CreatedAt = DateTime.UtcNow
            };
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// Bidirectional Node - tracks both forward and backward search state
        /// </summary>
        private class BidirectionalNode
        {
            public int NodeId { get; set; }
            public double Distance { get; set; }
            public double FScore { get; set; } // For A*
            public int ParentNodeId { get; set; } = -1;
            public bool IsVisited { get; set; } = false;
            public bool IsForward { get; set; } // Forward or backward search

            public override string ToString()
            {
                var direction = IsForward ? "Forward" : "Backward";
                return $"Node({NodeId}): Distance={Distance:F2}, Direction={direction}";
            }
        }

        /// <summary>
        /// Bidirectional Path - result of bidirectional search
        /// </summary>
        public class BidirectionalPath
        {
            public List<int> NodePath { get; set; } = new();
            public int MeetingPoint { get; set; }
            public double PathCost { get; set; }
            public string SearchType { get; set; } // Dijkstra, BFS, A*
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                var pathString = string.Join(" → ", NodePath);
                return $@"
╔════════════════════════════════════════════╗
║ Bidirectional Path ({SearchType})
╠════════════════════════════════════════════╣
║ Node Path:      {pathString}
║ Meeting Point:  {MeetingPoint}
║ Path Cost:      {PathCost,26:F2}
║ Hops:           {NodePath.Count - 1,26}
╚════════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// Bidirectional Comparison - compare with unidirectional
        /// </summary>
        public class BidirectionalComparison
        {
            public BidirectionalPath BidirectionalResult { get; set; }
            public double NodesExploredBidirectional { get; set; }
            public double NodesExploredUnidirectional { get; set; }
            public double SpeedupFactor { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
     BIDIRECTIONAL VS UNIDIRECTIONAL
═══════════════════════════════════════════════
Nodes Explored (Bidirectional):  {NodesExploredBidirectional:F0}
Nodes Explored (Unidirectional): {NodesExploredUnidirectional:F0}
Speedup Factor:                  {SpeedupFactor:F2}x

Path Cost:      {BidirectionalResult?.PathCost:F2}
Path Length:    {BidirectionalResult?.NodePath?.Count}

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// Search Statistics - detailed performance metrics
        /// </summary>
        public class SearchStatistics
        {
            public int TotalNodesExplored { get; set; }
            public int ForwardNodesExplored { get; set; }
            public int BackwardNodesExplored { get; set; }
            public double AverageBranchingFactor { get; set; }
            public int MeetingPoint { get; set; }
            public double SearchTime { get; set; }
            public string SearchAlgorithm { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
        SEARCH STATISTICS
═══════════════════════════════════════════════
Algorithm:              {SearchAlgorithm}
Total Nodes Explored:   {TotalNodesExplored}
Forward Nodes:          {ForwardNodesExplored}
Backward Nodes:         {BackwardNodesExplored}
Meeting Point:          {MeetingPoint}
Avg Branching Factor:   {AverageBranchingFactor:F2}
Search Time (ms):       {SearchTime:F2}

═══════════════════════════════════════════════
";
            }
        }
    }
}
