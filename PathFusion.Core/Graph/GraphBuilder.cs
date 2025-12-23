using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Graph
{
    public class GraphBuilder
    {
        private int _nodeIdCounter = 1;
        private int _edgeIdCounter = 1;

        // ============================================================================
        // SECTION 1: ROAD NETWORK BUILDER
        // ============================================================================

        /// <summary>
        /// Builds complete road network (cities + streets + highways + traffic)
        /// </summary>
        public NetworkGraph BuildRoadNetwork()
        {
            var network = new NetworkGraph
            {
                NetworkType = "Road",
                BuildTimestamp = DateTime.UtcNow
            };

            // Add major Pakistani cities as nodes
            var cities = new List<(string Name, string Urdu, double Lat, double Lon, string Type)>
        {
            ("Karachi", "کراچی", 24.8607, 67.0011, "Metropolis"),
            ("Lahore", "لاہور", 31.5204, 74.3587, "Metropolis"),
            ("Islamabad", "اسلام آباد", 33.6844, 73.0479, "Capital"),
            ("Rawalpindi", "راولپنڈی", 33.5731, 73.1906, "City"),
            ("Faisalabad", "فیصل آباد", 31.4181, 72.7979, "City"),
            ("Multan", "ملتان", 30.1575, 71.4243, "City"),
            ("Peshawar", "پشاور", 34.0151, 71.5249, "City"),
            ("Quetta", "کویٹہ", 30.1798, 67.0158, "City"),
            ("Sialkot", "سیالکوٹ", 32.4945, 74.5229, "City"),
            ("Gujranwala", "گجرانوالہ", 32.1619, 74.1875, "City"),
            ("Hyderabad", "حیدر آباد", 25.3548, 68.3711, "City"),
            ("Sukkur", "سکھر", 27.7097, 68.8703, "City"),
        };

            foreach (var city in cities)
            {
                var node = new GraphNode
                {
                    Id = _nodeIdCounter++,
                    Name = city.Name,
                    Urdu = city.Urdu,
                    NodeType = city.Type,
                    Latitude = city.Lat,
                    Longitude = city.Lon,
                    TransportMode = "Road",
                    Capacity = 50000
                };
                network.Nodes.Add(node);
            }

            // Add connections between nearby cities (highways)
            var highways = new List<(int Source, int Target, int Distance, int SpeedLimit)>
        {
            (1, 11, 1256, 120), // Karachi - Hyderabad
            (2, 3, 360, 120),   // Lahore - Islamabad
            (3, 4, 15, 100),    // Islamabad - Rawalpindi
            (2, 10, 90, 120),   // Lahore - Gujranwala
            (2, 5, 130, 120),   // Lahore - Faisalabad
            (5, 6, 240, 100),   // Faisalabad - Multan
            (1, 6, 520, 100),   // Karachi - Multan
            (1, 12, 380, 120),  // Karachi - Sukkur
            (3, 7, 450, 100),   // Islamabad - Peshawar
            (2, 7, 550, 100),   // Lahore - Peshawar
            (3, 8, 1500, 80),   // Islamabad - Quetta
        };

            foreach (var (source, target, distance, speedLimit) in highways)
            {
                var edge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = source,
                    TargetId = target,
                    Distance = distance,
                    SpeedLimit = speedLimit,
                    EdgeType = "Highway",
                    TrafficLevel = "Light",
                    Frequency = 0,
                    TimeWeight = CalculateTimeWeight(distance, speedLimit),
                    CostWeight = distance * 5,
                    MetaData = new Dictionary<string, object>
                {
                    { "RoadType", "National Highway" },
                    { "Lanes", 4 },
                    { "Toll", true }
                }
                };
                network.Edges.Add(edge);

                // Add reverse edge for undirected graph
                var reverseEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = target,
                    TargetId = source,
                    Distance = distance,
                    SpeedLimit = speedLimit,
                    EdgeType = "Highway",
                    TrafficLevel = "Light",
                    Frequency = 0,
                    TimeWeight = edge.TimeWeight,
                    CostWeight = edge.CostWeight,
                    MetaData = edge.MetaData
                };
                network.Edges.Add(reverseEdge);
            }

            network.IsValid = ValidateGraph(network);
            return network;
        }

        // ============================================================================
        // SECTION 2: RAIL NETWORK BUILDER
        // ============================================================================

        /// <summary>
        /// Builds complete rail network (stations + routes + metro)
        /// </summary>
        public NetworkGraph BuildRailNetwork()
        {
            var network = new NetworkGraph
            {
                NetworkType = "Rail",
                BuildTimestamp = DateTime.UtcNow
            };

            // Add major railway stations
            var stations = new List<(string Name, string Urdu, double Lat, double Lon, string Type)>
        {
            ("Karachi Central", "کراچی سینٹرل", 24.8607, 67.0011, "Major"),
            ("Lahore Junction", "لاہور جنکشن", 31.5204, 74.3587, "Major"),
            ("Rawalpindi Station", "راولپنڈی اسٹیشن", 33.5731, 73.1906, "Major"),
            ("Peshawar Station", "پشاور اسٹیشن", 34.0151, 71.5249, "Major"),
            ("Multan Station", "ملتان اسٹیشن", 30.1575, 71.4243, "Standard"),
            ("Faisalabad Station", "فیصل آباد اسٹیشن", 31.4181, 72.7979, "Standard"),
            ("Sialkot Station", "سیالکوٹ اسٹیشن", 32.4945, 74.5229, "Standard"),
            ("Gujranwala Station", "گجرانوالہ اسٹیشن", 32.1619, 74.1875, "Standard"),
            ("Hyderabad Station", "حیدر آباد اسٹیشن", 25.3548, 68.3711, "Standard"),
            ("Sukkur Station", "سکھر اسٹیشن", 27.7097, 68.8703, "Standard"),
            ("Islamabad Metro", "اسلام آباد میٹرو", 33.6844, 73.0479, "Metro"),
            ("Lahore Metro", "لاہور میٹرو", 31.5204, 74.3587, "Metro"),
        };

            foreach (var station in stations)
            {
                var node = new GraphNode
                {
                    Id = _nodeIdCounter++,
                    Name = station.Name,
                    Urdu = station.Urdu,
                    NodeType = station.Type,
                    Latitude = station.Lat,
                    Longitude = station.Lon,
                    TransportMode = "Rail",
                    Capacity = 2000
                };
                network.Nodes.Add(node);
            }

            // Add rail routes
            var routes = new List<(int Source, int Target, int Distance, string TrainType, int Frequency)>
        {
            (1, 9, 1256, "Express", 2),   // Karachi - Hyderabad
            (2, 3, 360, "Express", 4),    // Lahore - Rawalpindi
            (3, 4, 450, "Local", 3),      // Rawalpindi - Peshawar
            (2, 5, 130, "Local", 5),      // Lahore - Faisalabad
            (5, 6, 240, "Local", 2),      // Faisalabad - Multan
            (1, 6, 520, "Express", 2),    // Karachi - Multan
            (2, 7, 200, "Local", 6),      // Lahore - Gujranwala
        };

            foreach (var (source, target, distance, trainType, frequency) in routes)
            {
                var edge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = source,
                    TargetId = target,
                    Distance = distance,
                    SpeedLimit = 100,
                    EdgeType = "Rail",
                    TrafficLevel = "Scheduled",
                    Frequency = frequency,
                    TimeWeight = CalculateTimeWeight(distance, 90),
                    CostWeight = distance * 3,
                    TransferTime = 15,
                    MetaData = new Dictionary<string, object>
                {
                    { "TrainType", trainType },
                    { "Frequency", frequency }
                }
                };
                network.Edges.Add(edge);

                var reverseEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = target,
                    TargetId = source,
                    Distance = distance,
                    SpeedLimit = 100,
                    EdgeType = "Rail",
                    TrafficLevel = "Scheduled",
                    Frequency = frequency,
                    TimeWeight = edge.TimeWeight,
                    CostWeight = edge.CostWeight,
                    TransferTime = 15,
                    MetaData = edge.MetaData
                };
                network.Edges.Add(reverseEdge);
            }

            network.IsValid = ValidateGraph(network);
            return network;
        }

        // ============================================================================
        // SECTION 3: AIR NETWORK BUILDER
        // ============================================================================

        /// <summary>
        /// Builds complete air network (airports + routes)
        /// </summary>
        public NetworkGraph BuildAirNetwork()
        {
            var network = new NetworkGraph
            {
                NetworkType = "Air",
                BuildTimestamp = DateTime.UtcNow
            };

            // Add major airports
            var airports = new List<(string Name, string Urdu, double Lat, double Lon, string Type)>
        {
            ("Jinnah Intl Airport", "جناح بین الاقوامی ہوائی اڈہ", 24.9144, 67.1598, "International"),
            ("Lahore Intl Airport", "لاہور بین الاقوامی ہوائی اڈہ", 31.5215, 74.4015, "International"),
            ("Islamabad Intl Airport", "اسلام آباد بین الاقوامی ہوائی اڈہ", 33.5668, 73.0858, "International"),
            ("Peshawar Airport", "پشاور ہوائی اڈہ", 34.0151, 71.5249, "Domestic"),
            ("Multan Airport", "ملتان ہوائی اڈہ", 30.1909, 71.4256, "Domestic"),
            ("Faisalabad Airport", "فیصل آباد ہوائی اڈہ", 31.8706, 72.6711, "Domestic"),
            ("Quetta Airport", "کویٹہ ہوائی اڈہ", 30.2516, 66.9630, "Domestic"),
            ("Sialkot Airport", "سیالکوٹ ہوائی اڈہ", 32.5108, 74.5342, "Domestic"),
        };

            foreach (var airport in airports)
            {
                var node = new GraphNode
                {
                    Id = _nodeIdCounter++,
                    Name = airport.Name,
                    Urdu = airport.Urdu,
                    NodeType = airport.Type,
                    Latitude = airport.Lat,
                    Longitude = airport.Lon,
                    TransportMode = "Air",
                    Capacity = 5000
                };
                network.Nodes.Add(node);
            }

            // Add air routes
            var airRoutes = new List<(int Source, int Target, int Distance, string AircraftType, int Frequency)>
        {
            (1, 2, 900, "Regional", 4),    // Karachi - Lahore
            (1, 3, 1400, "Regional", 3),   // Karachi - Islamabad
            (2, 3, 350, "Regional", 5),    // Lahore - Islamabad
            (3, 4, 450, "Regional", 2),    // Islamabad - Peshawar
            (1, 5, 520, "Regional", 2),    // Karachi - Multan
            (2, 6, 250, "Regional", 2),    // Lahore - Faisalabad
            (1, 7, 900, "Regional", 1),    // Karachi - Quetta
            (1, 8, 1100, "Regional", 1),   // Karachi - Sialkot
        };

            foreach (var (source, target, distance, aircraft, frequency) in airRoutes)
            {
                var edge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = source,
                    TargetId = target,
                    Distance = distance,
                    SpeedLimit = 450,
                    EdgeType = "Air",
                    TrafficLevel = "Scheduled",
                    Frequency = frequency,
                    TimeWeight = CalculateTimeWeight(distance, 450),
                    CostWeight = distance * 10,
                    TransferTime = 120,
                    MetaData = new Dictionary<string, object>
                {
                    { "AircraftType", aircraft },
                    { "Frequency", frequency }
                }
                };
                network.Edges.Add(edge);

                var reverseEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = target,
                    TargetId = source,
                    Distance = distance,
                    SpeedLimit = 450,
                    EdgeType = "Air",
                    TrafficLevel = "Scheduled",
                    Frequency = frequency,
                    TimeWeight = edge.TimeWeight,
                    CostWeight = edge.CostWeight,
                    TransferTime = 120,
                    MetaData = edge.MetaData
                };
                network.Edges.Add(reverseEdge);
            }

            network.IsValid = ValidateGraph(network);
            return network;
        }

        // ============================================================================
        // SECTION 4: MULTI-MODAL TRANSFERS
        // ============================================================================

        public void AddMultiModalTransfers(NetworkGraph road, NetworkGraph rail, NetworkGraph air)
        {
            // Road to Rail transfers (city to station)
            var roadRailTransfers = new List<(int RoadNode, int RailNode, int TransferTime)>
        {
            (1, 1, 20), (2, 2, 25), (3, 3, 15), (4, 4, 20),
            (5, 6, 30), (6, 5, 25), (7, 4, 35), (8, 7, 40),
        };

            foreach (var (roadId, railId, time) in roadRailTransfers)
            {
                var transferEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = roadId,
                    TargetId = railId,
                    Distance = 5,
                    SpeedLimit = 20,
                    EdgeType = "Transfer",
                    TrafficLevel = "Pedestrian",
                    Frequency = 0,
                    TimeWeight = time,
                    CostWeight = 50,
                    TransferTime = time,
                    MetaData = new Dictionary<string, object>
                {
                    { "TransferType", "Road-to-Rail" },
                    { "Method", "Walk/Taxi" }
                }
                };
                road.TransferEdges.Add(transferEdge);
            }

            // Road to Air transfers (city to airport)
            var roadAirTransfers = new List<(int RoadNode, int AirNode, int TransferTime)>
        {
            (1, 1, 45), (2, 2, 35), (3, 3, 40), (4, 4, 50),
            (5, 6, 60), (6, 5, 55), (7, 4, 70), (8, 8, 90),
        };

            foreach (var (roadId, airId, time) in roadAirTransfers)
            {
                var transferEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = roadId,
                    TargetId = airId,
                    Distance = 20,
                    SpeedLimit = 60,
                    EdgeType = "Transfer",
                    TrafficLevel = "Vehicle",
                    Frequency = 0,
                    TimeWeight = time,
                    CostWeight = 200,
                    TransferTime = time,
                    MetaData = new Dictionary<string, object>
                {
                    { "TransferType", "Road-to-Air" },
                    { "Method", "Taxi/Shuttle" }
                }
                };
                road.TransferEdges.Add(transferEdge);
            }

            // Rail to Air transfers (station to airport)
            var railAirTransfers = new List<(int RailNode, int AirNode, int TransferTime)>
        {
            (1, 1, 60), (2, 2, 50), (3, 3, 55), (4, 4, 75),
        };

            foreach (var (railId, airId, time) in railAirTransfers)
            {
                var transferEdge = new GraphEdge
                {
                    Id = _edgeIdCounter++,
                    SourceId = railId,
                    TargetId = airId,
                    Distance = 25,
                    SpeedLimit = 50,
                    EdgeType = "Transfer",
                    TrafficLevel = "Vehicle",
                    Frequency = 0,
                    TimeWeight = time,
                    CostWeight = 150,
                    TransferTime = time,
                    MetaData = new Dictionary<string, object>
                {
                    { "TransferType", "Rail-to-Air" },
                    { "Method", "Shuttle/Public Transport" }
                }
                };
                rail.TransferEdges.Add(transferEdge);
            }
        }

        // ============================================================================
        // SECTION 5: HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Calculates time weight based on distance and speed
        /// </summary>
        private double CalculateTimeWeight(int distance, int speedLimit)
        {
            return (distance / (double)speedLimit) * 60; // Result in minutes
        }

        /// <summary>
        /// Validates graph structure
        /// </summary>
        private bool ValidateGraph(NetworkGraph graph)
        {
            if (graph.Nodes.Count == 0 || graph.Edges.Count == 0)
                return false;

            // Check all edges reference valid nodes
            foreach (var edge in graph.Edges)
            {
                if (!graph.Nodes.Any(n => n.Id == edge.SourceId) ||
                    !graph.Nodes.Any(n => n.Id == edge.TargetId))
                    return false;
            }

            return true;
        }
    }

    // ============================================================================
    // HELPER CLASSES
    // ============================================================================

    public class NetworkGraph
    {
        public string NetworkType { get; set; }
        public List<GraphNode> Nodes { get; set; } = new();
        public List<GraphEdge> Edges { get; set; } = new();
        public List<GraphEdge> TransferEdges { get; set; } = new();
        public bool IsValid { get; set; }
        public DateTime BuildTimestamp { get; set; }
    }

    public class GraphNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Urdu { get; set; }
        public string NodeType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TransportMode { get; set; }
        public int Capacity { get; set; }
        public Dictionary<string, object> MetaData { get; set; } = new();
    }

    public class GraphEdge
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public int Distance { get; set; }
        public int SpeedLimit { get; set; }
        public string EdgeType { get; set; }
        public string TrafficLevel { get; set; }
        public int Frequency { get; set; }
        public double TimeWeight { get; set; }
        public double CostWeight { get; set; }
        public int TransferTime { get; set; }
        public Dictionary<string, object> MetaData { get; set; } = new();
    }

    public class GraphStatistics
    {
        public int NodeCount { get; set; }
        public int EdgeCount { get; set; }
        public int TransferEdgeCount { get; set; }
        public double AverageNodeDegree { get; set; }
        public int TotalDistance { get; set; }
        public double TotalAverageTime { get; set; }
        public double TotalAverageCost { get; set; }
        public string NetworkType { get; set; }
        public bool IsConnected { get; set; }
    }
}
