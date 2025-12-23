using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Graph;
using PathFusion.Core.Data;
public class NetworkManager
{
    private readonly GraphBuilder _graphBuilder;
    private NetworkGraph _roadNetwork;
    private NetworkGraph _railNetwork;
    private NetworkGraph _airNetwork;
    private Dictionary<string, NetworkGraph> _networkCache;
    private Dictionary<string, DateTime> _networkTimestamps;
    private readonly object _lockObject = new object();

    public bool IsInitialized { get; private set; }
    public DateTime LastUpdateTime { get; private set; }

    public NetworkManager()
    {
        _graphBuilder = new GraphBuilder();
        _networkCache = new Dictionary<string, NetworkGraph>();
        _networkTimestamps = new Dictionary<string, DateTime>();
        IsInitialized = false;
    }

    // ============================================================================
    // SECTION 1: NETWORK INITIALIZATION
    // ============================================================================

    public async Task<bool> InitializeAllNetworksAsync()
    {
        try
        {
            lock (_lockObject)
            {
                if (IsInitialized) return true;
            }

            _roadNetwork = await Task.Run(() => _graphBuilder.BuildRoadNetwork());
            _railNetwork = await Task.Run(() => _graphBuilder.BuildRailNetwork());
            _airNetwork = await Task.Run(() => _graphBuilder.BuildAirNetwork());

            if (!ValidateNetworks())
                throw new NetworkManagerException("Network validation failed");

            _graphBuilder.AddMultiModalTransfers(_roadNetwork, _railNetwork, _airNetwork);
            CacheNetworks();

            lock (_lockObject)
            {
                IsInitialized = true;
                LastUpdateTime = DateTime.UtcNow;
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new NetworkManagerException($"Failed to initialize networks: {ex.Message}", ex);
        }
    }

    public async Task<NetworkGraph> InitializeNetworkAsync(string networkType)
    {
        try
        {
            return networkType.ToLower() switch
            {
                "road" => await Task.Run(() =>
                {
                    _roadNetwork = _graphBuilder.BuildRoadNetwork();
                    CacheNetwork(_roadNetwork);
                    return _roadNetwork;
                }),

                "rail" => await Task.Run(() =>
                {
                    _railNetwork = _graphBuilder.BuildRailNetwork();
                    CacheNetwork(_railNetwork);
                    return _railNetwork;
                }),

                "air" => await Task.Run(() =>
                {
                    _airNetwork = _graphBuilder.BuildAirNetwork();
                    CacheNetwork(_airNetwork);
                    return _airNetwork;
                }),

                _ => throw new NetworkManagerException($"Unknown network type: {networkType}")
            };
        }
        catch (Exception ex)
        {
            throw new NetworkManagerException($"Failed to initialize {networkType} network: {ex.Message}", ex);
        }
    }

    // ============================================================================
    // SECTION 2: NETWORK RETRIEVAL
    // ============================================================================

    public NetworkGraph GetRoadNetwork()
    {
        if (_roadNetwork == null)
            throw new NetworkManagerException("Road network not initialized");
        return _roadNetwork;
    }

    public NetworkGraph GetRailNetwork()
    {
        if (_railNetwork == null)
            throw new NetworkManagerException("Rail network not initialized");
        return _railNetwork;
    }

    public NetworkGraph GetAirNetwork()
    {
        if (_airNetwork == null)
            throw new NetworkManagerException("Air network not initialized");
        return _airNetwork;
    }

    public NetworkGraph GetNetwork(string networkType)
    {
        return networkType.ToLower() switch
        {
            "road" => GetRoadNetwork(),
            "rail" => GetRailNetwork(),
            "air" => GetAirNetwork(),
            _ => throw new NetworkManagerException($"Unknown network type: {networkType}")
        };
    }

    public Dictionary<string, NetworkGraph> GetAllNetworks()
    {
        return new Dictionary<string, NetworkGraph>
        {
            { "Road", GetRoadNetwork() },
            { "Rail", GetRailNetwork() },
            { "Air", GetAirNetwork() }
        };
    }

    // ============================================================================
    // SECTION 3: NETWORK CACHING
    // ============================================================================

    private void CacheNetworks()
    {
        CacheNetwork(_roadNetwork);
        CacheNetwork(_railNetwork);
        CacheNetwork(_airNetwork);
    }

    private void CacheNetwork(NetworkGraph network)
    {
        if (network == null) return;
        lock (_lockObject)
        {
            _networkCache[network.NetworkType] = network;
            _networkTimestamps[network.NetworkType] = DateTime.UtcNow;
        }
    }

    public NetworkGraph GetCachedNetwork(string networkType)
    {
        lock (_lockObject)
        {
            if (_networkCache.TryGetValue(networkType, out var network))
                return network;
        }
        return null;
    }

    public void ClearCache()
    {
        lock (_lockObject)
        {
            _networkCache.Clear();
            _networkTimestamps.Clear();
        }
    }

    public Dictionary<string, object> GetCacheStats()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, object>
            {
                { "CachedNetworks", _networkCache.Count },
                { "RoadNetworkCached", _networkCache.ContainsKey("Road") },
                { "RailNetworkCached", _networkCache.ContainsKey("Rail") },
                { "AirNetworkCached", _networkCache.ContainsKey("Air") },
                { "LastUpdateTime", LastUpdateTime }
            };
        }
    }

    // ============================================================================
    // SECTION 4: NETWORK VALIDATION
    // ============================================================================

    private bool ValidateNetworks()
    {
        return ValidateNetwork(_roadNetwork, "Road") &&
               ValidateNetwork(_railNetwork, "Rail") &&
               ValidateNetwork(_airNetwork, "Air");
    }

    private bool ValidateNetwork(NetworkGraph network, string name)
    {
        try
        {
            if (network == null || network.Nodes.Count == 0 || network.Edges.Count == 0)
                return false;

            foreach (var edge in network.Edges)
            {
                if (!network.Nodes.Any(n => n.Id == edge.SourceId) ||
                    !network.Nodes.Any(n => n.Id == edge.TargetId))
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ValidateConnectivity(string networkType, int minConnectivityPercent = 80)
    {
        var network = GetNetwork(networkType);
        if (network.Nodes.Count == 0) return false;

        var startNode = network.Nodes.First().Id;
        var reachable = new HashSet<int>();
        var queue = new Queue<int>();

        queue.Enqueue(startNode);
        reachable.Add(startNode);

        while (queue.Count > 0)
        {
            var nodeId = queue.Dequeue();
            var adjacentEdges = network.Edges.Where(e => e.SourceId == nodeId).ToList();

            foreach (var edge in adjacentEdges)
            {
                if (!reachable.Contains(edge.TargetId))
                {
                    reachable.Add(edge.TargetId);
                    queue.Enqueue(edge.TargetId);
                }
            }
        }

        var connectivity = (reachable.Count * 100.0) / network.Nodes.Count;
        return connectivity >= minConnectivityPercent;
    }

    // ============================================================================
    // SECTION 5: NETWORK STATISTICS
    // ============================================================================

    public Dictionary<string, GraphStatistics> GetAllNetworkStats()
    {
        return new Dictionary<string, GraphStatistics>
        {
            { "Road", GetNetworkStats("Road") },
            { "Rail", GetNetworkStats("Rail") },
            { "Air", GetNetworkStats("Air") }
        };
    }

    public GraphStatistics GetNetworkStats(string networkType)
    {
        var network = GetNetwork(networkType);

        return new GraphStatistics
        {
            NodeCount = network.Nodes.Count,
            EdgeCount = network.Edges.Count,
            TransferEdgeCount = network.TransferEdges.Count,
            AverageNodeDegree = network.Edges.Count > 0 ?
                (network.Edges.Count * 2.0) / network.Nodes.Count : 0,
            TotalDistance = network.Edges.Sum(e => e.Distance),
            TotalAverageTime = network.Edges.Sum(e => e.TimeWeight),
            TotalAverageCost = network.Edges.Sum(e => e.CostWeight),
            NetworkType = networkType,
            IsConnected = network.IsValid
        };
    }

    public NetworkSummary GetNetworkSummary(string networkType)
    {
        var network = GetNetwork(networkType);
        var stats = GetNetworkStats(networkType);

        return new NetworkSummary
        {
            NetworkType = networkType,
            NodeCount = stats.NodeCount,
            EdgeCount = stats.EdgeCount,
            TransferCount = stats.TransferEdgeCount,
            TotalDistance = stats.TotalDistance,
            AverageEdgeDistance = stats.EdgeCount > 0 ?
                (double)stats.TotalDistance / stats.EdgeCount : 0,
            AverageEdgeCost = stats.EdgeCount > 0 ?
                stats.TotalAverageCost / stats.EdgeCount : 0,
            Connectivity = GetConnectivityPercentage(networkType),
            IsValid = network.IsValid,
            BuildTime = network.BuildTimestamp,
            NodeTypes = network.Nodes.GroupBy(n => n.NodeType)
                .ToDictionary(g => g.Key, g => g.Count()),
            EdgeTypes = network.Edges.GroupBy(e => e.EdgeType)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public double GetConnectivityPercentage(string networkType)
    {
        var network = GetNetwork(networkType);
        if (network.Nodes.Count == 0) return 0;

        var startNode = network.Nodes.First().Id;
        var reachable = CountReachableNodes(network, startNode);
        return (reachable * 100.0) / network.Nodes.Count;
    }

    private int CountReachableNodes(NetworkGraph network, int startNodeId)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        queue.Enqueue(startNodeId);
        visited.Add(startNodeId);

        while (queue.Count > 0)
        {
            var nodeId = queue.Dequeue();
            var adjacentEdges = network.Edges.Where(e => e.SourceId == nodeId).ToList();

            foreach (var edge in adjacentEdges)
            {
                if (!visited.Contains(edge.TargetId))
                {
                    visited.Add(edge.TargetId);
                    queue.Enqueue(edge.TargetId);
                }
            }
        }

        return visited.Count;
    }

    // ============================================================================
    // SECTION 6: NODE OPERATIONS
    // ============================================================================

    public GraphNode FindNode(int nodeId, string networkType = null)
    {
        if (networkType != null)
        {
            var network = GetNetwork(networkType);
            return network.Nodes.FirstOrDefault(n => n.Id == nodeId);
        }

        var allNetworks = GetAllNetworks();
        foreach (var network in allNetworks.Values)
        {
            var node = network.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null) return node;
        }

        return null;
    }

    public List<GraphNode> SearchNodes(string query, string networkType = null)
    {
        var lowerQuery = query.ToLower();

        if (networkType != null)
        {
            var network = GetNetwork(networkType);
            return network.Nodes.Where(n =>
                n.Name.ToLower().Contains(lowerQuery) ||
                (n.Urdu != null && n.Urdu.Contains(query))
            ).ToList();
        }

        var allNetworks = GetAllNetworks();
        var results = new List<GraphNode>();
        foreach (var network in allNetworks.Values)
        {
            results.AddRange(network.Nodes.Where(n =>
                n.Name.ToLower().Contains(lowerQuery) ||
                (n.Urdu != null && n.Urdu.Contains(query))
            ));
        }

        return results.DistinctBy(n => n.Id).ToList();
    }

    public List<GraphNode> GetNodesByType(string nodeType, string networkType = null)
    {
        if (networkType != null)
        {
            var network = GetNetwork(networkType);
            return network.Nodes.Where(n => n.NodeType == nodeType).ToList();
        }

        var allNetworks = GetAllNetworks();
        var results = new List<GraphNode>();
        foreach (var network in allNetworks.Values)
        {
            results.AddRange(network.Nodes.Where(n => n.NodeType == nodeType));
        }

        return results;
    }

    // ============================================================================
    // SECTION 7: EDGE OPERATIONS
    // ============================================================================

    public List<GraphEdge> GetAdjacentEdges(int nodeId, string networkType)
    {
        var network = GetNetwork(networkType);
        return network.Edges.Where(e => e.SourceId == nodeId || e.TargetId == nodeId).ToList();
    }

    public List<GraphEdge> GetOutgoingEdges(int nodeId, string networkType)
    {
        var network = GetNetwork(networkType);
        return network.Edges.Where(e => e.SourceId == nodeId).ToList();
    }

    public List<GraphEdge> GetIncomingEdges(int nodeId, string networkType)
    {
        var network = GetNetwork(networkType);
        return network.Edges.Where(e => e.TargetId == nodeId).ToList();
    }

    public GraphEdge FindEdge(int sourceId, int targetId, string networkType)
    {
        var network = GetNetwork(networkType);
        return network.Edges.FirstOrDefault(e =>
            e.SourceId == sourceId && e.TargetId == targetId);
    }

    public List<GraphEdge> GetEdgesByType(string edgeType, string networkType = null)
    {
        if (networkType != null)
        {
            var network = GetNetwork(networkType);
            return network.Edges.Where(e => e.EdgeType == edgeType).ToList();
        }

        var allNetworks = GetAllNetworks();
        var results = new List<GraphEdge>();
        foreach (var network in allNetworks.Values)
        {
            results.AddRange(network.Edges.Where(e => e.EdgeType == edgeType));
        }

        return results;
    }

    // ============================================================================
    // SECTION 8: TRANSFER OPERATIONS
    // ============================================================================

    public List<GraphEdge> GetTransferEdges(string networkType)
    {
        var network = GetNetwork(networkType);
        return network.TransferEdges;
    }

    public List<GraphEdge> GetNodeTransfers(int nodeId, string fromNetwork, string toNetwork)
    {
        var fromNet = GetNetwork(fromNetwork);
        return fromNet.TransferEdges.Where(e => e.SourceId == nodeId).ToList();
    }

    // ============================================================================
    // SECTION 9: NETWORK ANALYSIS
    // ============================================================================

    public double GetNetworkDensity(string networkType)
    {
        var network = GetNetwork(networkType);
        var n = network.Nodes.Count;
        var maxEdges = n * (n - 1) / 2.0;
        return maxEdges > 0 ? network.Edges.Count / maxEdges : 0;
    }

    public List<(GraphNode Node, int Degree)> GetNodesByDegree(string networkType, int topN = 10)
    {
        var network = GetNetwork(networkType);

        var nodeDegrees = network.Nodes.Select(node =>
        {
            var degree = network.Edges.Count(e => e.SourceId == node.Id || e.TargetId == node.Id);
            return (Node: node, Degree: degree);
        })
        .OrderByDescending(x => x.Degree)
        .Take(topN)
        .ToList();

        return nodeDegrees;
    }

    public List<int> GetShortestPath(int sourceId, int targetId, string networkType)
    {
        var network = GetNetwork(networkType);
        return DijkstraShortestPath(network, sourceId, targetId, edge => edge.Distance);
    }

    private List<int> DijkstraShortestPath(NetworkGraph network, int source, int target,
        Func<GraphEdge, double> getWeight)
    {
        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int>();
        var unvisited = new HashSet<int>();

        foreach (var node in network.Nodes)
        {
            distances[node.Id] = double.MaxValue;
            unvisited.Add(node.Id);
        }

        distances[source] = 0;

        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(n => distances[n]).First();

            if (current == target || distances[current] == double.MaxValue) break;

            unvisited.Remove(current);

            foreach (var edge in network.Edges.Where(e => e.SourceId == current))
            {
                var neighbor = edge.TargetId;
                if (!unvisited.Contains(neighbor)) continue;

                var alt = distances[current] + getWeight(edge);
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }

        var path = new List<int>();
        var current_node = target;

        if (!previous.ContainsKey(current_node) && current_node != source)
            return null;

        while (previous.ContainsKey(current_node))
        {
            path.Insert(0, current_node);
            current_node = previous[current_node];
        }

        path.Insert(0, source);
        return path;
    }

    // ============================================================================
    // SECTION 10: DIAGNOSTICS
    // ============================================================================

    public NetworkDiagnosticReport GetDiagnosticReport()
    {
        return new NetworkDiagnosticReport
        {
            IsInitialized = IsInitialized,
            LastUpdateTime = LastUpdateTime,
            RoadNetworkStats = IsInitialized ? GetNetworkStats("Road") : null,
            RailNetworkStats = IsInitialized ? GetNetworkStats("Rail") : null,
            AirNetworkStats = IsInitialized ? GetNetworkStats("Air") : null,
            CacheStats = GetCacheStats()
        };
    }
}

// ============================================================================
// HELPER CLASSES
// ============================================================================

public class NetworkSummary
{
    public string NetworkType { get; set; }
    public int NodeCount { get; set; }
    public int EdgeCount { get; set; }
    public int TransferCount { get; set; }
    public int TotalDistance { get; set; }
    public double AverageEdgeDistance { get; set; }
    public double AverageEdgeCost { get; set; }
    public double Connectivity { get; set; }
    public bool IsValid { get; set; }
    public DateTime BuildTime { get; set; }
    public Dictionary<string, int> NodeTypes { get; set; }
    public Dictionary<string, int> EdgeTypes { get; set; }
}

public class NetworkDiagnosticReport
{
    public bool IsInitialized { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public GraphStatistics RoadNetworkStats { get; set; }
    public GraphStatistics RailNetworkStats { get; set; }
    public GraphStatistics AirNetworkStats { get; set; }
    public Dictionary<string, object> CacheStats { get; set; }
}

public class NetworkManagerException : Exception
{
    public NetworkManagerException(string message) : base(message) { }
    public NetworkManagerException(string message, Exception innerException)
        : base(message, innerException) { }
}