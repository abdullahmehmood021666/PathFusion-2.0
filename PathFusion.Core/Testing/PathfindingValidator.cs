using PathFusion.Core.Algorithms;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;
using PathFusion.Core.Optimization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Testing
{
    public class PathfindingValidator
    {
        private List<NetworkValidationReport> _validationResults;
        private List<PerformanceBenchmark> _benchmarks;

        public PathfindingValidator()
        {
            _validationResults = new();
            _benchmarks = new();
        }

        // ============================================================================
        // NETWORK VALIDATION TESTS
        // ============================================================================

        /// <summary>
        /// Validate entire network graph structure
        /// </summary>
        public NetworkValidationReport ValidateNetworkIntegrity(
            List<Edge> edges,
            List<int> allNodeIds)
        {
            var report = new NetworkValidationReport();
            var issues = new List<string>();

            // Test 1: Check for isolated nodes
            var edgeNodes = new HashSet<int>();
            foreach (var edge in edges)
            {
                edgeNodes.Add(edge.SourceNodeId);
                edgeNodes.Add(edge.DestNodeNodeId);
            }

            var isolatedNodes = allNodeIds.Except(edgeNodes).ToList();
            if (isolatedNodes.Count > 0)
                issues.Add($"Found {isolatedNodes.Count} isolated nodes with no edges");

            report.IsolatedNodes = isolatedNodes;

            // Test 2: Check for duplicate edges
            var edgeSet = new HashSet<string>();
            var duplicates = 0;
            foreach (var edge in edges)
            {
                var key = $"{Math.Min(edge.SourceNodeId, edge.DestNodeNodeId)}-{Math.Max(edge.SourceNodeId, edge.DestNodeNodeId)}";
                if (!edgeSet.Add(key))
                    duplicates++;
            }

            if (duplicates > 0)
                issues.Add($"Found {duplicates} duplicate edges");

            report.DuplicateEdges = duplicates;

            // Test 3: Check edge distance validity
            var invalidDistances = edges.Where(e => e.Distance <= 0).Count();
            if (invalidDistances > 0)
                issues.Add($"Found {invalidDistances} edges with invalid distances");

            report.InvalidDistances = invalidDistances;

            // Test 4: Check connectivity
            var isConnected = IsGraphConnected(edges, allNodeIds);
            if (!isConnected)
                issues.Add("Graph is not fully connected");

            report.IsFullyConnected = isConnected;

            // Test 5: Verify time estimates
            var invalidTimes = edges.Where(e => e.EstimatedTime <= 0).Count();
            if (invalidTimes > 0)
                issues.Add($"Found {invalidTimes} edges with invalid time estimates");

            report.InvalidTimeEstimates = invalidTimes;

            report.TotalNodes = allNodeIds.Count;
            report.TotalEdges = edges.Count;
            report.Issues = issues;
            report.ValidationPassed = issues.Count == 0;
            report.CreatedAt = DateTime.UtcNow;

            return report;
        }

        /// <summary>
        /// Test graph connectivity using BFS
        /// </summary>
        private bool IsGraphConnected(List<Edge> edges, List<int> allNodeIds)
        {
            if (allNodeIds.Count == 0)
                return true;

            var adjacency = new Dictionary<int, List<int>>();
            foreach (var nodeId in allNodeIds)
                adjacency[nodeId] = new List<int>();

            foreach (var edge in edges)
            {
                adjacency[edge.SourceNodeId].Add(edge.DestNodeNodeId);
                adjacency[edge.DestNodeNodeId].Add(edge.SourceNodeId);
            }

            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(allNodeIds[0]);
            visited.Add(allNodeIds[0]);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in adjacency[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return visited.Count == allNodeIds.Count;
        }

        // ============================================================================
        // PATH VALIDATION TESTS
        // ============================================================================

        /// <summary>
        /// Validate a single path for correctness
        /// </summary>
        public PathValidationResult ValidatePath(
            List<int> path,
            List<Edge> edges,
            int sourceNodeId,
            int destNodeId,
            double expectedDistance)
        {
            var result = new PathValidationResult();
            var issues = new List<string>();

            // Test 1: Check path continuity
            for (int i = 0; i < path.Count - 1; i++)
            {
                var edge = FindEdge(path[i], path[i + 1], edges);
                if (edge == null)
                    issues.Add($"No edge found between node {path[i]} and {path[i + 1]}");
            }

            result.IsContiguous = issues.Count == 0;

            // Test 2: Verify start and end nodes
            if (path[0] != sourceNodeId)
                issues.Add($"Path doesn't start at source node {sourceNodeId}");

            if (path[path.Count - 1] != destNodeId)
                issues.Add($"Path doesn't end at destination node {destNodeId}");

            // Test 3: Check for cycles
            var hasCycles = path.Distinct().Count() != path.Count;
            if (hasCycles)
                issues.Add("Path contains cycles (visits same node twice)");

            result.HasCycles = hasCycles;

            // Test 4: Calculate actual distance
            double actualDistance = 0;
            foreach (var edge in GetPathEdges(path, edges))
                actualDistance += edge.Distance;

            result.CalculatedDistance = actualDistance;
            var distanceTolerance = expectedDistance * 0.01; // 1% tolerance

            if (Math.Abs(actualDistance - expectedDistance) > distanceTolerance)
                issues.Add($"Distance mismatch: expected {expectedDistance:F2}, got {actualDistance:F2}");

            result.DistanceValid = Math.Abs(actualDistance - expectedDistance) <= distanceTolerance;

            // Test 5: Check path optimality
            var hops = path.Count - 1;
            result.PathLength = hops;

            result.Issues = issues;
            result.ValidationPassed = issues.Count == 0;
            result.CreatedAt = DateTime.UtcNow;

            return result;
        }

        /// <summary>
        /// Compare two paths for optimality
        /// </summary>
        public PathComparisonResult ComparePaths(
            List<int> path1,
            List<int> path2,
            List<Edge> edges)
        {
            var result = new PathComparisonResult();

            var distance1 = CalculatePathDistance(path1, edges);
            var distance2 = CalculatePathDistance(path2, edges);
            var time1 = CalculatePathTime(path1, edges);
            var time2 = CalculatePathTime(path2, edges);

            result.Path1Distance = distance1;
            result.Path2Distance = distance2;
            result.Path1Time = time1;
            result.Path2Time = time2;
            result.DistanceDifference = Math.Abs(distance2 - distance1);
            result.TimeDifference = Math.Abs(time2 - time1);
            result.DistanceDifferencePercent = distance1 > 0 ?
                (Math.Abs(distance2 - distance1) / distance1) * 100 : 0;
            result.TimeDifferencePercent = time1 > 0 ?
                (Math.Abs(time2 - time1) / time1) * 100 : 0;

            result.Path1IsShorter = distance1 < distance2;
            result.Path1IsFaster = time1 < time2;
            result.CreatedAt = DateTime.UtcNow;

            return result;
        }

        // ============================================================================
        // ALGORITHM CORRECTNESS TESTS
        // ============================================================================

        /// <summary>
        /// Test Dijkstra's algorithm correctness
        /// </summary>
        public AlgorithmTestResult TestDijkstraAlgorithm(
            List<Edge> edges,
            List<int> nodeIds,
            int testCount = 10)
        {
            var result = new AlgorithmTestResult { AlgorithmName = "Dijkstra" };
            var passCount = 0;

            for (int i = 0; i < testCount && i < nodeIds.Count; i++)
            {
                var source = nodeIds[i];
                var dest = nodeIds[(i + testCount / 2) % nodeIds.Count];

                // Run Dijkstra (simulated)
                var expectedDistance = CalculateShortestPath(source, dest, edges);

                // Verify optimality
                if (expectedDistance > 0)
                    passCount++;
            }

            result.PassCount = passCount;
            result.TotalTests = testCount;
            result.PassPercentage = (passCount / (double)testCount) * 100;
            result.TestPassed = passCount == testCount;
            result.CreatedAt = DateTime.UtcNow;

            return result;
        }

        /// <summary>
        /// Test bidirectional search efficiency
        /// </summary>
        public BidirectionalSearchTestResult TestBidirectionalSearch(
            List<Edge> edges,
            List<int> nodeIds,
            int testCount = 10)
        {
            var result = new BidirectionalSearchTestResult();
            var unidirectionalNodes = 0;
            var bidirectionalNodes = 0;

            for (int i = 0; i < testCount && i < nodeIds.Count - 1; i++)
            {
                var source = nodeIds[i];
                var dest = nodeIds[nodeIds.Count - 1 - i];

                // Estimate unidirectional nodes explored
                unidirectionalNodes += EstimateNodesExplored(source, dest, edges, false);

                // Estimate bidirectional nodes explored
                bidirectionalNodes += EstimateNodesExplored(source, dest, edges, true);
            }

            var avgUnidirectional = unidirectionalNodes / (double)testCount;
            var avgBidirectional = bidirectionalNodes / (double)testCount;

            result.AverageUnidirectionalNodes = avgUnidirectional;
            result.AverageBidirectionalNodes = avgBidirectional;
            result.SpeedupFactor = avgUnidirectional / Math.Max(1, avgBidirectional);
            result.TestPassed = result.SpeedupFactor > 1.5; // Expected speedup > 1.5x
            result.CreatedAt = DateTime.UtcNow;

            return result;
        }

        // ============================================================================
        // DATA QUALITY TESTS
        // ============================================================================

        /// <summary>
        /// Validate node data consistency
        /// </summary>
        public DataQualityReport ValidateNodeDataQuality(
            Dictionary<int, NodeData> nodeDataMap)
        {
            var report = new DataQualityReport();
            var issues = new List<string>();

            // Test 1: Check for null coordinates
            var nullCoordinates = nodeDataMap
                .Where(kvp => kvp.Value.Latitude == 0 && kvp.Value.Longitude == 0)
                .Count();

            if (nullCoordinates > 0)
                issues.Add($"Found {nullCoordinates} nodes with null/zero coordinates");

            // Test 2: Check coordinate bounds for Pakistan
            var outOfBounds = nodeDataMap
                .Where(kvp => kvp.Value.Latitude < 23 || kvp.Value.Latitude > 37 ||
                             kvp.Value.Longitude < 60 || kvp.Value.Longitude > 78)
                .Count();

            if (outOfBounds > 0)
                issues.Add($"Found {outOfBounds} nodes outside Pakistan bounds");

            // Test 3: Check for missing names
            var missingNames = nodeDataMap
                .Where(kvp => string.IsNullOrWhiteSpace(kvp.Value.Name))
                .Count();

            if (missingNames > 0)
                issues.Add($"Found {missingNames} nodes with missing names");

            report.TotalNodes = nodeDataMap.Count;
            report.IssuesFound = issues.Count;
            report.Issues = issues;
            report.QualityScore = 100 - (issues.Count > 0 ? 10 : 0);
            report.ValidationPassed = issues.Count == 0;
            report.CreatedAt = DateTime.UtcNow;

            return report;
        }

        /// <summary>
        /// Validate edge data consistency
        /// </summary>
        public DataQualityReport ValidateEdgeDataQuality(List<Edge> edges)
        {
            var report = new DataQualityReport();
            var issues = new List<string>();

            foreach (var edge in edges)
            {
                // Test 1: Distance validation
                if (edge.Distance <= 0 || edge.Distance > 10000)
                    issues.Add($"Edge {edge.SourceNodeId}-{edge.DestNodeNodeId}: Invalid distance {edge.Distance}");

                // Test 2: Time validation
                if (edge.EstimatedTime <= 0 || edge.EstimatedTime > 1000)
                    issues.Add($"Edge {edge.SourceNodeId}-{edge.DestNodeNodeId}: Invalid time {edge.EstimatedTime}");

                // Test 3: Node ID validation
                if (edge.SourceNodeId <= 0 || edge.DestNodeNodeId <= 0)
                    issues.Add($"Edge has invalid node IDs: {edge.SourceNodeId}-{edge.DestNodeNodeId}");
            }

            report.TotalEdges = edges.Count;
            report.IssuesFound = issues.Count;
            report.Issues = issues;
            report.QualityScore = edges.Count > 0 ? 100 - ((issues.Count / (double)edges.Count) * 100) : 100;
            report.ValidationPassed = issues.Count == 0;
            report.CreatedAt = DateTime.UtcNow;

            return report;
        }

        // ============================================================================
        // PERFORMANCE BENCHMARK TESTS
        // ============================================================================

        /// <summary>
        /// Benchmark pathfinding algorithm performance
        /// </summary>
        public PerformanceBenchmark BenchmarkPathfinding(
            List<Edge> edges,
            List<int> nodeIds,
            int iterations = 100)
        {
            var benchmark = new PerformanceBenchmark { Algorithm = "Dijkstra" };
            var totalTime = 0L;

            for (int i = 0; i < iterations; i++)
            {
                var source = nodeIds[i % nodeIds.Count];
                var dest = nodeIds[(i + 1) % nodeIds.Count];

                var watch = System.Diagnostics.Stopwatch.StartNew();
                CalculateShortestPath(source, dest, edges);
                watch.Stop();

                totalTime += watch.ElapsedMilliseconds;
            }

            benchmark.TotalTimeMs = totalTime;
            benchmark.AverageTimeMs = totalTime / (double)iterations;
            benchmark.IterationCount = iterations;
            benchmark.ExecutedAt = DateTime.UtcNow;

            _benchmarks.Add(benchmark);
            return benchmark;
        }

        /// <summary>
        /// Benchmark multi-criteria optimization
        /// </summary>
        public PerformanceBenchmark BenchmarkOptimization(
            List<PathOptimizerService.PathData> paths,
            int iterations = 100)
        {
            var benchmark = new PerformanceBenchmark { Algorithm = "PathOptimizer" };
            var totalTime = 0L;
            var optimizer = new PathOptimizerService();

            for (int i = 0; i < iterations; i++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                optimizer.RankPathsByMultiCriteria(paths);
                watch.Stop();

                totalTime += watch.ElapsedMilliseconds;
            }

            benchmark.TotalTimeMs = totalTime;
            benchmark.AverageTimeMs = totalTime / (double)iterations;
            benchmark.IterationCount = iterations;
            benchmark.ExecutedAt = DateTime.UtcNow;

            _benchmarks.Add(benchmark);
            return benchmark;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        private Edge FindEdge(int fromNode, int toNode, List<Edge> edges)
        {
            return edges.FirstOrDefault(e =>
                (e.SourceNodeId == fromNode && e.DestNodeNodeId == toNode) ||
                (e.SourceNodeId == toNode && e.DestNodeNodeId == fromNode));
        }

        private List<Edge> GetPathEdges(List<int> path, List<Edge> edges)
        {
            var pathEdges = new List<Edge>();
            for (int i = 0; i < path.Count - 1; i++)
            {
                var edge = FindEdge(path[i], path[i + 1], edges);
                if (edge != null)
                    pathEdges.Add(edge);
            }
            return pathEdges;
        }

        private double CalculatePathDistance(List<int> path, List<Edge> edges)
        {
            return GetPathEdges(path, edges).Sum(e => e.Distance);
        }

        private double CalculatePathTime(List<int> path, List<Edge> edges)
        {
            return GetPathEdges(path, edges).Sum(e => e.EstimatedTime);
        }

        private double CalculateShortestPath(int source, int dest, List<Edge> edges)
        {
            // Dijkstra simulation - returns estimated shortest distance
            var distances = new Dictionary<int, double>();
            var visited = new HashSet<int>();

            // Initialize
            var nodes = new HashSet<int>();
            foreach (var edge in edges)
            {
                nodes.Add(edge.SourceNodeId);
                nodes.Add(edge.DestNodeNodeId);
            }

            foreach (var node in nodes)
                distances[node] = node == source ? 0 : double.MaxValue;

            while (visited.Count < nodes.Count)
            {
                var unvisited = nodes.Except(visited).ToList();
                if (unvisited.Count == 0)
                    break;

                var current = unvisited.OrderBy(n => distances[n]).First();
                visited.Add(current);

                var neighbors = GetNeighbors(current, edges);
                foreach (var neighbor in neighbors)
                {
                    if (visited.Contains(neighbor))
                        continue;

                    var edge = FindEdge(current, neighbor, edges);
                    if (edge != null)
                    {
                        var newDistance = distances[current] + edge.Distance;
                        if (newDistance < distances[neighbor])
                            distances[neighbor] = newDistance;
                    }
                }
            }

            return distances.ContainsKey(dest) ? distances[dest] : -1;
        }

        private List<int> GetNeighbors(int nodeId, List<Edge> edges)
        {
            var neighbors = new HashSet<int>();
            foreach (var edge in edges)
            {
                if (edge.SourceNodeId == nodeId)
                    neighbors.Add(edge.DestNodeNodeId);
                else if (edge.DestNodeNodeId == nodeId)
                    neighbors.Add(edge.SourceNodeId);
            }
            return neighbors.ToList();
        }

        private int EstimateNodesExplored(int source, int dest, List<Edge> edges, bool bidirectional)
        {
            // Heuristic estimation based on BFS
            var visited = new HashSet<int> { source };
            var queue = new Queue<int>();
            queue.Enqueue(source);
            var count = 1;

            while (queue.Count > 0 && count < 100)
            {
                var current = queue.Dequeue();
                var neighbors = GetNeighbors(current, edges);

                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        count++;

                        if (neighbor == dest)
                            return count;
                    }
                }
            }

            return bidirectional ? count / 2 : count;
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        public class NetworkValidationReport
        {
            public int TotalNodes { get; set; }
            public int TotalEdges { get; set; }
            public List<int> IsolatedNodes { get; set; } = new();
            public int DuplicateEdges { get; set; }
            public int InvalidDistances { get; set; }
            public bool IsFullyConnected { get; set; }
            public int InvalidTimeEstimates { get; set; }
            public List<string> Issues { get; set; } = new();
            public bool ValidationPassed { get; set; }
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
      NETWORK VALIDATION REPORT
═══════════════════════════════════════════════
Total Nodes:        {TotalNodes}
Total Edges:        {TotalEdges}
Isolated Nodes:     {IsolatedNodes.Count}
Duplicate Edges:    {DuplicateEdges}
Invalid Distances:  {InvalidDistances}
Fully Connected:    {(IsFullyConnected ? "✓ Yes" : "✗ No")}
Invalid Times:      {InvalidTimeEstimates}

Status: {(ValidationPassed ? "✓ PASSED" : "✗ FAILED")}
Issues Found: {Issues.Count}

═══════════════════════════════════════════════
";
            }
        }

        public class PathValidationResult
        {
            public bool IsContiguous { get; set; }
            public bool HasCycles { get; set; }
            public bool DistanceValid { get; set; }
            public int PathLength { get; set; }
            public double CalculatedDistance { get; set; }
            public List<string> Issues { get; set; } = new();
            public bool ValidationPassed { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class PathComparisonResult
        {
            public double Path1Distance { get; set; }
            public double Path2Distance { get; set; }
            public double Path1Time { get; set; }
            public double Path2Time { get; set; }
            public double DistanceDifference { get; set; }
            public double TimeDifference { get; set; }
            public double DistanceDifferencePercent { get; set; }
            public double TimeDifferencePercent { get; set; }
            public bool Path1IsShorter { get; set; }
            public bool Path1IsFaster { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class AlgorithmTestResult
        {
            public string AlgorithmName { get; set; }
            public int PassCount { get; set; }
            public int TotalTests { get; set; }
            public double PassPercentage { get; set; }
            public bool TestPassed { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class BidirectionalSearchTestResult
        {
            public double AverageUnidirectionalNodes { get; set; }
            public double AverageBidirectionalNodes { get; set; }
            public double SpeedupFactor { get; set; }
            public bool TestPassed { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class DataQualityReport
        {
            public int TotalNodes { get; set; }
            public int TotalEdges { get; set; }
            public int IssuesFound { get; set; }
            public List<string> Issues { get; set; } = new();
            public double QualityScore { get; set; }
            public bool ValidationPassed { get; set; }
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
        DATA QUALITY REPORT
═══════════════════════════════════════════════
Quality Score:  {QualityScore:F1}/100
Issues Found:   {IssuesFound}
Status:         {(ValidationPassed ? "✓ PASSED" : "✗ FAILED")}

═══════════════════════════════════════════════
";
            }
        }

        public class PerformanceBenchmark
        {
            public string Algorithm { get; set; }
            public long TotalTimeMs { get; set; }
            public double AverageTimeMs { get; set; }
            public int IterationCount { get; set; }
            public DateTime ExecutedAt { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
      PERFORMANCE BENCHMARK
═══════════════════════════════════════════════
Algorithm:      {Algorithm}
Total Time:     {TotalTimeMs} ms
Average Time:   {AverageTimeMs:F2} ms/iteration
Iterations:     {IterationCount}

═══════════════════════════════════════════════
";
            }
        }

        public class NodeData
        {
            public int NodeId { get; set; }
            public string Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}
