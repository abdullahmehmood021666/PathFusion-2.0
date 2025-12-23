using PathFusion.Core.Algorithms;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Optimization
{
    public class PathDetailEnricherService
    {
        private Dictionary<int, NodeInformation> _nodeInformation;
        private Dictionary<int, EdgeInformation> _edgeInformation;
        private List<PathSegment> _pathSegments;

        public PathDetailEnricherService()
        {
            _nodeInformation = new();
            _edgeInformation = new();
            _pathSegments = new();
        }

        // ============================================================================
        // TURN-BY-TURN DIRECTIONS
        // ============================================================================

        /// <summary>
        /// Generate turn-by-turn directions for a path
        /// </summary>
        public List<TurnInstruction> GenerateTurnByTurnDirections(
            List<int> nodePath,
            List<Edge> edges,
            Dictionary<int, (double latitude, double longitude)> nodeCoordinates)
        {
            var instructions = new List<TurnInstruction>();

            if (nodePath == null || nodePath.Count < 2)
                return instructions;

            // Starting instruction
            instructions.Add(new TurnInstruction
            {
                StepNumber = 1,
                Instruction = $"Start at node {nodePath[0]}",
                Direction = "START",
                Distance = 0,
                CumulativeDistance = 0
            });

            double cumulativeDistance = 0;

            // Generate turn instructions for each segment
            for (int i = 1; i < nodePath.Count; i++)
            {
                var fromNode = nodePath[i - 1];
                var toNode = nodePath[i];
                var nextNode = i + 1 < nodePath.Count ? nodePath[i + 1] : -1;

                var edge = FindEdge(fromNode, toNode, edges);
                if (edge == null)
                    continue;

                cumulativeDistance += edge.Distance;

                var direction = nextNode != -1 ?
                    CalculateTurnDirection(fromNode, toNode, nextNode, nodeCoordinates) :
                    "END";

                var instruction = GenerateTurnInstruction(
                    i + 1, direction, toNode, edge.Distance, cumulativeDistance, nodeCoordinates);

                instructions.Add(instruction);
            }

            // Add final instruction
            instructions.Add(new TurnInstruction
            {
                StepNumber = instructions.Count + 1,
                Instruction = $"Arrive at destination (node {nodePath.Last()})",
                Direction = "DESTINATION",
                Distance = 0,
                CumulativeDistance = cumulativeDistance
            });

            return instructions;
        }

        /// <summary>
        /// Generate detailed segment information for path
        /// </summary>
        public List<PathSegment> GeneratePathSegments(
            List<int> nodePath,
            List<Edge> edges,
            Dictionary<int, NodeType> nodeTypes)
        {
            _pathSegments.Clear();

            if (nodePath == null || nodePath.Count < 2)
                return _pathSegments;

            for (int i = 0; i < nodePath.Count - 1; i++)
            {
                var fromNode = nodePath[i];
                var toNode = nodePath[i + 1];
                var edge = FindEdge(fromNode, toNode, edges);

                if (edge == null)
                    continue;

                var segment = new PathSegment
                {
                    SegmentIndex = i,
                    FromNodeId = fromNode,
                    ToNodeId = toNode,
                    Distance = edge.Distance,
                    EstimatedTime = edge.EstimatedTime,
                    RoadName = GetRoadName(edge),
                    RoadType = GetRoadType(edge),
                    TransportMode = DetermineTransportMode(edge),
                    TrafficCondition = SimulateTrafficCondition(edge),
                    SafetyScore = CalculateSafetyScore(edge),
                    FromNodeType = nodeTypes.ContainsKey(fromNode) ? nodeTypes[fromNode] : NodeType.Intersection,
                    ToNodeType = nodeTypes.ContainsKey(toNode) ? nodeTypes[toNode] : NodeType.Intersection,
                    CreatedAt = DateTime.UtcNow
                };

                _pathSegments.Add(segment);
            }

            return _pathSegments;
        }

        // ============================================================================
        // TRANSFER POINTS & INTERCHANGE
        // ============================================================================

        /// <summary>
        /// Identify transfer points in multi-modal journey
        /// </summary>
        public List<TransferPoint> IdentifyTransferPoints(
            List<int> nodePath,
            List<Edge> edges,
            Dictionary<int, NodeType> nodeTypes)
        {
            var transfers = new List<TransferPoint>();
            var transportMode = TransportMode.Unknown;

            for (int i = 0; i < nodePath.Count - 1; i++)
            {
                var edge = FindEdge(nodePath[i], nodePath[i + 1], edges);
                if (edge == null)
                    continue;

                var currentMode = DetermineTransportMode(edge);

                // Mode change detected - transfer point
                if (transportMode != TransportMode.Unknown && currentMode != transportMode)
                {
                    var transferNode = nodePath[i];
                    var nodeType = nodeTypes.ContainsKey(transferNode) ? nodeTypes[transferNode] : NodeType.Intersection;

                    transfers.Add(new TransferPoint
                    {
                        NodeId = transferNode,
                        FromTransportMode = transportMode,
                        ToTransportMode = currentMode,
                        WaitTime = CalculateWaitTime(currentMode),
                        TransferType = DetermineTransferType(nodeType),
                        Instructions = GenerateTransferInstructions(transportMode, currentMode, nodeType),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                transportMode = currentMode;
            }

            return transfers;
        }

        /// <summary>
        /// Get metro/bus stop information for a node
        /// </summary>
        public StopInformation GetStopInformation(
            int nodeId,
            Dictionary<int, NodeType> nodeTypes,
            Dictionary<int, string> stopNames)
        {
            if (!nodeTypes.ContainsKey(nodeId))
                return null;

            var nodeType = nodeTypes[nodeId];
            var stopName = stopNames.ContainsKey(nodeId) ? stopNames[nodeId] : $"Stop {nodeId}";

            return new StopInformation
            {
                NodeId = nodeId,
                StopName = stopName,
                StopType = nodeType switch
                {
                    NodeType.MetroStation => "Metro Station",
                    NodeType.BusStop => "Bus Stop",
                    NodeType.RailStation => "Railway Station",
                    _ => "Interchange"
                },
                Amenities = GetAmenities(nodeType),
                AccessibilityFeatures = GetAccessibilityFeatures(nodeType)
            };
        }

        // ============================================================================
        // TRAFFIC & ROUTING SIMULATION
        // ============================================================================

        /// <summary>
        /// Simulate traffic condition on segment
        /// </summary>
        public TrafficCondition SimulateTrafficCondition(Edge edge)
        {
            // Simulate based on edge properties
            var random = new Random(edge.SourceNodeId + edge.DestNodeNodeId);
            var congestionLevel = random.NextDouble();

            return congestionLevel switch
            {
                < 0.3 => TrafficCondition.Clear,
                < 0.6 => TrafficCondition.Moderate,
                < 0.8 => TrafficCondition.Congested,
                _ => TrafficCondition.VeryHeavy
            };
        }

        /// <summary>
        /// Calculate safety score for segment (0-100)
        /// </summary>
        public int CalculateSafetyScore(Edge edge)
        {
            // Base safety score
            var score = 75;

            // Adjust based on road type
            score += DetermineTransportMode(edge) switch
            {
                TransportMode.Metro => 20,
                TransportMode.Rail => 15,
                TransportMode.Bus => 10,
                TransportMode.Road => 5,
                _ => 0
            };

            // Traffic impacts safety
            var traffic = SimulateTrafficCondition(edge);
            score -= traffic switch
            {
                TrafficCondition.VeryHeavy => 15,
                TrafficCondition.Congested => 10,
                TrafficCondition.Moderate => 5,
                _ => 0
            };

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Estimate travel time with traffic
        /// </summary>
        public double EstimateTravelTimeWithTraffic(
            List<PathSegment> segments,
            DateTime departureTime)
        {
            double totalTime = 0;

            foreach (var segment in segments)
            {
                var trafficMultiplier = segment.TrafficCondition switch
                {
                    TrafficCondition.VeryHeavy => 3.0,
                    TrafficCondition.Congested => 2.0,
                    TrafficCondition.Moderate => 1.3,
                    TrafficCondition.Clear => 1.0,
                    _ => 1.0
                };

                totalTime += segment.EstimatedTime * trafficMultiplier;
            }

            return totalTime;
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Find edge between two nodes
        /// </summary>
        private Edge FindEdge(int fromNode, int toNode, List<Edge> edges)
        {
            return edges.FirstOrDefault(e =>
                (e.SourceNodeId == fromNode && e.DestNodeNodeId == toNode) ||
                (e.SourceNodeId == toNode && e.DestNodeNodeId == fromNode));
        }

        /// <summary>
        /// Determine transport mode from edge
        /// </summary>
        private TransportMode DetermineTransportMode(Edge edge)
        {
            // Heuristic based on edge distance and speed
            if (edge.EstimatedTime > 0)
            {
                var speed = edge.Distance / edge.EstimatedTime;
                return speed switch
                {
                    > 100 => TransportMode.Metro,
                    > 80 => TransportMode.Rail,
                    > 30 => TransportMode.Bus,
                    _ => TransportMode.Road
                };
            }

            return TransportMode.Road;
        }

        /// <summary>
        /// Calculate turn direction
        /// </summary>
        private string CalculateTurnDirection(
            int fromNode, int toNode, int nextNode,
            Dictionary<int, (double lat, double lon)> coordinates)
        {
            // Simplified turn calculation
            if (!coordinates.ContainsKey(fromNode) ||
                !coordinates.ContainsKey(toNode) ||
                !coordinates.ContainsKey(nextNode))
                return "STRAIGHT";

            var (lat1, lon1) = coordinates[fromNode];
            var (lat2, lon2) = coordinates[toNode];
            var (lat3, lon3) = coordinates[nextNode];

            var bearing1 = CalculateBearing(lat1, lon1, lat2, lon2);
            var bearing2 = CalculateBearing(lat2, lon2, lat3, lon3);
            var turn = bearing2 - bearing1;

            // Normalize to 0-360
            while (turn < 0) turn += 360;
            while (turn >= 360) turn -= 360;

            return turn switch
            {
                < 30 or >= 330 => "STRAIGHT",
                < 120 => "TURN_RIGHT",
                < 150 => "TURN_SHARP_RIGHT",
                < 210 => "TURN_AROUND",
                < 240 => "TURN_SHARP_LEFT",
                _ => "TURN_LEFT"
            };
        }

        /// <summary>
        /// Calculate bearing between two coordinates
        /// </summary>
        private double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            var dLon = ToRadians(lon2 - lon1);
            var y = Math.Sin(dLon) * Math.Cos(ToRadians(lat2));
            var x = Math.Cos(ToRadians(lat1)) * Math.Sin(ToRadians(lat2)) -
                    Math.Sin(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Cos(dLon);
            var bearing = Math.Atan2(y, x);
            return (ToDegrees(bearing) + 360) % 360;
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        private double ToDegrees(double radians) => radians * 180.0 / Math.PI;

        /// <summary>
        /// Generate turn instruction text
        /// </summary>
        private TurnInstruction GenerateTurnInstruction(
            int step, string direction, int toNode, double distance, double cumulative,
            Dictionary<int, (double lat, double lon)> coordinates)
        {
            var instruction = direction switch
            {
                "STRAIGHT" => $"Continue straight for {distance:F1} km",
                "TURN_RIGHT" => $"Turn right for {distance:F1} km",
                "TURN_SHARP_RIGHT" => $"Turn sharp right for {distance:F1} km",
                "TURN_LEFT" => $"Turn left for {distance:F1} km",
                "TURN_SHARP_LEFT" => $"Turn sharp left for {distance:F1} km",
                "TURN_AROUND" => $"Turn around for {distance:F1} km",
                _ => $"Head towards node {toNode} for {distance:F1} km"
            };

            return new TurnInstruction
            {
                StepNumber = step,
                Instruction = instruction,
                Direction = direction,
                Distance = distance,
                CumulativeDistance = cumulative,
                TargetNode = toNode
            };
        }

        /// <summary>
        /// Calculate wait time for transport mode
        /// </summary>
        private double CalculateWaitTime(TransportMode mode)
        {
            return mode switch
            {
                TransportMode.Metro => 3.0,
                TransportMode.Bus => 5.0,
                TransportMode.Rail => 7.0,
                _ => 0
            };
        }

        /// <summary>
        /// Determine transfer type
        /// </summary>
        private string DetermineTransferType(NodeType nodeType)
        {
            return nodeType switch
            {
                NodeType.MetroStation => "Metro Transfer",
                NodeType.BusStop => "Bus Stop",
                NodeType.RailStation => "Rail Transfer",
                _ => "Interchange"
            };
        }

        /// <summary>
        /// Generate transfer instructions
        /// </summary>
        private string GenerateTransferInstructions(
            TransportMode from, TransportMode to, NodeType nodeType)
        {
            return $"Transfer from {from} to {to} at {nodeType}";
        }

        /// <summary>
        /// Get road name for edge
        /// </summary>
        private string GetRoadName(Edge edge)
        {
            return $"Route {edge.SourceNodeId}-{edge.DestNodeNodeId}";
        }

        /// <summary>
        /// Get road type
        /// </summary>
        private string GetRoadType(Edge edge)
        {
            return "Primary";
        }

        /// <summary>
        /// Get amenities for node type
        /// </summary>
        private List<string> GetAmenities(NodeType nodeType)
        {
            return nodeType switch
            {
                NodeType.MetroStation => new List<string> { "Ticket Counter", "Restrooms", "Food Court", "ATM" },
                NodeType.BusStop => new List<string> { "Shelter", "Bench", "Ticket Counter" },
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Get accessibility features
        /// </summary>
        private List<string> GetAccessibilityFeatures(NodeType nodeType)
        {
            return new List<string> { "Elevator", "Wheelchair Accessible", "Braille Signs" };
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// Turn Instruction
        /// </summary>
        public class TurnInstruction
        {
            public int StepNumber { get; set; }
            public string Instruction { get; set; }
            public string Direction { get; set; }
            public double Distance { get; set; }
            public double CumulativeDistance { get; set; }
            public int TargetNode { get; set; }

            public override string ToString()
            {
                return $"Step {StepNumber}: {Instruction} (Cumulative: {CumulativeDistance:F1} km)";
            }
        }

        /// <summary>
        /// Path Segment
        /// </summary>
        public class PathSegment
        {
            public int SegmentIndex { get; set; }
            public int FromNodeId { get; set; }
            public int ToNodeId { get; set; }
            public double Distance { get; set; }
            public double EstimatedTime { get; set; }
            public string RoadName { get; set; }
            public string RoadType { get; set; }
            public TransportMode TransportMode { get; set; }
            public TrafficCondition TrafficCondition { get; set; }
            public int SafetyScore { get; set; }
            public NodeType FromNodeType { get; set; }
            public NodeType ToNodeType { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        /// <summary>
        /// Transfer Point
        /// </summary>
        public class TransferPoint
        {
            public int NodeId { get; set; }
            public TransportMode FromTransportMode { get; set; }
            public TransportMode ToTransportMode { get; set; }
            public double WaitTime { get; set; }
            public string TransferType { get; set; }
            public string Instructions { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        /// <summary>
        /// Stop Information
        /// </summary>
        public class StopInformation
        {
            public int NodeId { get; set; }
            public string StopName { get; set; }
            public string StopType { get; set; }
            public List<string> Amenities { get; set; } = new();
            public List<string> AccessibilityFeatures { get; set; } = new();
        }

        /// <summary>
        /// Node Information
        /// </summary>
        public class NodeInformation
        {
            public int NodeId { get; set; }
            public string Name { get; set; }
            public NodeType Type { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        /// <summary>
        /// Edge Information
        /// </summary>
        public class EdgeInformation
        {
            public int FromNode { get; set; }
            public int ToNode { get; set; }
            public double Distance { get; set; }
            public double Speed { get; set; }
        }

        /// <summary>
        /// Transport Mode
        /// </summary>
        public enum TransportMode
        {
            Road,
            Bus,
            Metro,
            Rail,
            Unknown
        }

        /// <summary>
        /// Traffic Condition
        /// </summary>
        public enum TrafficCondition
        {
            Clear,
            Moderate,
            Congested,
            VeryHeavy
        }

        /// <summary>
        /// Node Type
        /// </summary>
        public enum NodeType
        {
            Intersection,
            MetroStation,
            BusStop,
            RailStation,
            Destination
        }
    }
}
