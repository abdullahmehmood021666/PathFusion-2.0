using PathFusion.Core.Data;
using PathFusion.Core.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Network
{
    public class VehicleNodeManager
    {
        private readonly Dictionary<int, VehicleTypeDefinitions.VehicleType> _vehicleCache;
        private readonly Dictionary<int, List<int>> _nodeVehicleCompatibility;
        private readonly Dictionary<int, NodeVehicleMapping> _vehicleNodeMappings;
        private readonly Dictionary<int, VehiclePerformanceMetrics> _performanceCache;

        public VehicleNodeManager()
        {
            _vehicleCache = new();
            _nodeVehicleCompatibility = new();
            _vehicleNodeMappings = new();
            _performanceCache = new();
            InitializeVehicleCache();
        }

        // ============================================================================
        // INITIALIZATION METHODS
        // ============================================================================

        /// <summary>
        /// Initialize vehicle cache with all 13 vehicle types
        /// </summary>
        private void InitializeVehicleCache()
        {
            var allVehicles = VehicleTypeDefinitions.GetAllVehicleTypes();
            foreach (var vehicle in allVehicles)
            {
                _vehicleCache[vehicle.Id] = vehicle;
            }
        }

        // ============================================================================
        // VEHICLE RETRIEVAL METHODS
        // ============================================================================

        /// <summary>
        /// Get vehicle by ID with caching
        /// </summary>
        public VehicleTypeDefinitions.VehicleType GetVehicleById(int vehicleId)
        {
            if (_vehicleCache.TryGetValue(vehicleId, out var vehicle))
                return vehicle;

            return VehicleTypeDefinitions.GetVehicleById(vehicleId);
        }

        /// <summary>
        /// Get all vehicles of a specific category
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetVehiclesByCategory(string category)
        {
            return _vehicleCache.Values
                .Where(v => v.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Get eco-friendly vehicles (low emissions + high standard)
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetEcoFriendlyVehicles()
        {
            return _vehicleCache.Values
                .Where(v => v.Co2PerKm < 0.2 && v.EmissionStandard >= 4)
                .ToList();
        }

        /// <summary>
        /// Get budget-friendly vehicles (low cost per km)
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetBudgetFriendlyVehicles()
        {
            return _vehicleCache.Values
                .Where(v => v.CostPerKm < 10)
                .ToList();
        }

        /// <summary>
        /// Get high-speed vehicles
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetHighSpeedVehicles()
        {
            return _vehicleCache.Values
                .Where(v => v.MaxSpeed >= 160)
                .ToList();
        }

        /// <summary>
        /// Get all vehicles
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetAllVehicles()
        {
            return _vehicleCache.Values.ToList();
        }

        // ============================================================================
        // NODE COMPATIBILITY METHODS
        // ============================================================================

        /// <summary>
        /// Get all vehicles compatible with a node
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetCompatibleVehicles(int nodeId)
        {
            if (_nodeVehicleCompatibility.TryGetValue(nodeId, out var vehicleIds))
            {
                return vehicleIds
                    .Where(id => _vehicleCache.ContainsKey(id))
                    .Select(id => _vehicleCache[id])
                    .ToList();
            }

            var compatible = _vehicleCache.Values
                .Where(v => v.RouteCompatibility.Contains(nodeId))
                .ToList();

            _nodeVehicleCompatibility[nodeId] = compatible.Select(v => v.Id).ToList();
            return compatible;
        }

        /// <summary>
        /// Check if vehicle is compatible with node
        /// </summary>
        public bool IsVehicleCompatibleWithNode(int vehicleId, int nodeId)
        {
            if (!_vehicleCache.TryGetValue(vehicleId, out var vehicle))
                return false;

            return vehicle.RouteCompatibility.Contains(nodeId);
        }

        // ============================================================================
        // BEST VEHICLE SELECTION METHODS
        // ============================================================================

        /// <summary>
        /// Get best vehicle for a route based on distance
        /// Considers: Cost (30%), Time (30%), Emissions (40%)
        /// </summary>
        public VehicleTypeDefinitions.VehicleType GetBestVehicleForRoute(
            int sourceNodeId,
            int destNodeId,
            double distance)
        {
            if (distance <= 0) return null;

            var compatibleVehicles = _vehicleCache.Values
                .Where(v => v.RouteCompatibility.Contains(sourceNodeId) ||
                            v.RouteCompatibility.Contains(destNodeId))
                .ToList();

            if (!compatibleVehicles.Any()) return null;

            return compatibleVehicles
                .Select(v => new
                {
                    Vehicle = v,
                    Score = CalculateCompositeScore(v, distance)
                })
                .OrderBy(x => x.Score)
                .First()
                .Vehicle;
        }

        /// <summary>
        /// Calculate composite score for route (lower is better)
        /// </summary>
        private double CalculateCompositeScore(VehicleTypeDefinitions.VehicleType vehicle, double distance)
        {
            double costScore = (double)(vehicle.CostPerKm * (decimal)distance) * 0.30;
            double timeScore = (distance / vehicle.AverageSpeed) * 0.30;
            double emissionScore = (vehicle.Co2PerKm * distance) * 0.40;

            return costScore + timeScore + emissionScore;
        }

        /// <summary>
        /// Get best vehicles ranked by criteria
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetBestVehiclesByScore(
            List<VehicleTypeDefinitions.VehicleType> vehicles,
            double distance,
            int count = 3)
        {
            return vehicles
                .Select(v => new
                {
                    Vehicle = v,
                    Score = CalculateCompositeScore(v, distance)
                })
                .OrderBy(x => x.Score)
                .Take(count)
                .Select(x => x.Vehicle)
                .ToList();
        }

        // ============================================================================
        // PERFORMANCE METRICS METHODS
        // ============================================================================

        /// <summary>
        /// Calculate route performance metrics for a specific vehicle
        /// </summary>
        public VehiclePerformanceMetrics CalculatePerformanceMetrics(
            int vehicleId,
            double distance)
        {
            if (!_vehicleCache.TryGetValue(vehicleId, out var vehicle))
                return null;

            if (distance <= 0) return null;

            var metrics = new VehiclePerformanceMetrics
            {
                VehicleId = vehicleId,
                VehicleName = vehicle.Name,
                VehicleUrdu = vehicle.Urdu,
                VehicleCategory = vehicle.Category,
                TravelTime = distance / vehicle.AverageSpeed,                              // Hours
                TravelCost = (double)(vehicle.CostPerKm * (decimal)distance),              // PKR
                FuelConsumption = (distance / vehicle.MaxRange) * vehicle.FuelCapacity,    // Liters
                EmissionGenerated = vehicle.Co2PerKm * distance,                           // kg CO2
                ComfortScore = vehicle.ComfortRating * 20,                                 // 0-100
                SafetyScore = vehicle.SafetyRating * 20,                                   // 0-100
                EnvironmentalScore = Math.Max(0, 100 - (vehicle.Co2PerKm * 100)),          // 0-100
                OverallScore = CalculateOverallScore(vehicle),
                Distance = distance
            };

            return metrics;
        }

        /// <summary>
        /// Calculate overall performance score for vehicle
        /// Weighted: Cost (25%), Efficiency (25%), Safety (25%), Comfort (25%)
        /// Range: 0-100
        /// </summary>
        private double CalculateOverallScore(VehicleTypeDefinitions.VehicleType vehicle)
        {
            // Cost score (0-100, lower cost = higher score)
            double costScore = Math.Max(0, 100 - ((double)vehicle.CostPerKm * 2));

            // Efficiency score (0-100, based on max and average speed)
            double efficiencyScore = ((vehicle.MaxSpeed + vehicle.AverageSpeed) / 4.0);

            // Safety score (0-100, safety rating * 20)
            double safetyScore = vehicle.SafetyRating * 20.0;

            // Comfort score (0-100, comfort rating * 20)
            double comfortScore = vehicle.ComfortRating * 20.0;

            // Weighted composite
            double overall = (costScore * 0.25) +
                            (Math.Min(efficiencyScore, 100) * 0.25) +
                            (safetyScore * 0.25) +
                            (comfortScore * 0.25);

            return Math.Min(overall, 100);
        }

        /// <summary>
        /// Get detailed performance comparison for multiple vehicles
        /// </summary>
        public List<VehiclePerformanceMetrics> GetDetailedComparison(
            List<VehicleTypeDefinitions.VehicleType> vehicles,
            double distance)
        {
            return vehicles
                .Select(v => CalculatePerformanceMetrics(v.Id, distance))
                .Where(m => m != null)
                .OrderByDescending(m => m.OverallScore)
                .ToList();
        }

        // ============================================================================
        // VEHICLE NODE MAPPING METHODS
        // ============================================================================

        /// <summary>
        /// Map vehicle to specific nodes for tracking
        /// </summary>
        public void MapVehicleToNodes(int vehicleId, List<int> nodeIds)
        {
            if (!_vehicleCache.ContainsKey(vehicleId))
                return;

            var mapping = new NodeVehicleMapping
            {
                VehicleId = vehicleId,
                VehicleName = _vehicleCache[vehicleId].Name,
                NodeIds = nodeIds,
                MappedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _vehicleNodeMappings[vehicleId] = mapping;
        }

        /// <summary>
        /// Get vehicle node mapping
        /// </summary>
        public NodeVehicleMapping GetVehicleNodeMapping(int vehicleId)
        {
            if (_vehicleNodeMappings.TryGetValue(vehicleId, out var mapping))
                return mapping;

            return null;
        }

        /// <summary>
        /// Update vehicle node mapping with new nodes
        /// </summary>
        public void UpdateVehicleNodeMapping(int vehicleId, List<int> newNodeIds)
        {
            if (_vehicleNodeMappings.TryGetValue(vehicleId, out var mapping))
            {
                mapping.NodeIds = newNodeIds;
                mapping.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                MapVehicleToNodes(vehicleId, newNodeIds);
            }
        }

        /// <summary>
        /// Get all vehicle mappings
        /// </summary>
        public List<NodeVehicleMapping> GetAllVehicleMappings()
        {
            return _vehicleNodeMappings.Values.ToList();
        }

        // ============================================================================
        // FILTERING & SEARCH METHODS
        // ============================================================================

        /// <summary>
        /// Find vehicles by transport mode
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetVehiclesByTransportMode(string mode)
        {
            return _vehicleCache.Values
                .Where(v => v.TransportMode.Equals(mode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Find vehicles by emission standard
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetVehiclesByEmissionStandard(int minStandard)
        {
            return _vehicleCache.Values
                .Where(v => v.EmissionStandard >= minStandard)
                .ToList();
        }

        /// <summary>
        /// Find vehicles by capacity range
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetVehiclesByCapacity(int minCapacity, int maxCapacity = int.MaxValue)
        {
            return _vehicleCache.Values
                .Where(v => v.Capacity >= minCapacity && v.Capacity <= maxCapacity)
                .ToList();
        }

        /// <summary>
        /// Find vehicles by price range
        /// </summary>
        public List<VehicleTypeDefinitions.VehicleType> GetVehiclesByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return _vehicleCache.Values
                .Where(v => v.CostPerKm >= minPrice && v.CostPerKm <= maxPrice)
                .ToList();
        }

        // ============================================================================
        // STATISTICS & ANALYTICS METHODS
        // ============================================================================

        /// <summary>
        /// Get vehicle statistics for dashboard
        /// </summary>
        public VehicleStatistics GetVehicleStatistics()
        {
            var vehicles = _vehicleCache.Values.ToList();

            return new VehicleStatistics
            {
                TotalVehicles = vehicles.Count,
                AverageCostPerKm = vehicles.Average(v => (double)v.CostPerKm),
                AverageSpeed = vehicles.Average(v => v.AverageSpeed),
                AverageComfortRating = vehicles.Average(v => v.ComfortRating),
                AverageSafetyRating = vehicles.Average(v => v.SafetyRating),
                AverageCapacity = vehicles.Average(v => v.Capacity),
                CheapestVehicle = vehicles.OrderBy(v => v.CostPerKm).First(),
                MostExpensiveVehicle = vehicles.OrderByDescending(v => v.CostPerKm).First(),
                FastestVehicle = vehicles.OrderByDescending(v => v.MaxSpeed).First(),
                SafestVehicle = vehicles.OrderByDescending(v => v.SafetyRating).First(),
                MostComfortableVehicle = vehicles.OrderByDescending(v => v.ComfortRating).First(),
                GreenerVehicles = GetEcoFriendlyVehicles(),
                BudgetVehicles = GetBudgetFriendlyVehicles()
            };
        }

        /// <summary>
        /// Cache performance metrics
        /// </summary>
        public void CachePerformanceMetrics(int vehicleId, VehiclePerformanceMetrics metrics)
        {
            _performanceCache[vehicleId] = metrics;
        }

        /// <summary>
        /// Get cached performance metrics
        /// </summary>
        public VehiclePerformanceMetrics GetCachedPerformanceMetrics(int vehicleId)
        {
            if (_performanceCache.TryGetValue(vehicleId, out var metrics))
                return metrics;

            return null;
        }

        /// <summary>
        /// Clear performance cache
        /// </summary>
        public void ClearPerformanceCache()
        {
            _performanceCache.Clear();
        }

        // ============================================================================
        // DATA STRUCTURES - NESTED CLASSES
        // ============================================================================

        /// <summary>
        /// Vehicle Performance Metrics - Comprehensive performance analysis
        /// </summary>
        public class VehiclePerformanceMetrics
        {
            public int VehicleId { get; set; }
            public string VehicleName { get; set; }
            public string VehicleUrdu { get; set; }
            public string VehicleCategory { get; set; }

            // Travel Metrics
            public double Distance { get; set; }            // km
            public double TravelTime { get; set; }          // Hours
            public double TravelCost { get; set; }          // PKR
            public double FuelConsumption { get; set; }     // Liters

            // Environmental Metrics
            public double EmissionGenerated { get; set; }   // kg CO2

            // Quality Metrics
            public double ComfortScore { get; set; }        // 0-100
            public double SafetyScore { get; set; }         // 0-100
            public double EnvironmentalScore { get; set; }  // 0-100
            public double OverallScore { get; set; }        // Composite 0-100

            // Display formatted results
            public override string ToString()
            {
                return $@"
╔═══════════════════════════════════════════╗
║ Vehicle: {VehicleName,-35} ║
║ Category: {VehicleCategory,-32} ║
╠═══════════════════════════════════════════╣
║ Distance:      {Distance,25:F2} km
║ Travel Time:   {TravelTime,25:F2} hours
║ Travel Cost:   {TravelCost,25:F0} PKR
║ Fuel Used:     {FuelConsumption,25:F2} liters
║ Emissions:     {EmissionGenerated,25:F2} kg CO₂
╠═══════════════════════════════════════════╣
║ Comfort Score: {ComfortScore,25:F0}/100
║ Safety Score:  {SafetyScore,25:F0}/100
║ Env. Score:    {EnvironmentalScore,25:F0}/100
║ Overall Score: {OverallScore,25:F2}/100
╚═══════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// Vehicle-Node Mapping - Tracks vehicle assignments to nodes
        /// </summary>
        public class NodeVehicleMapping
        {
            public int VehicleId { get; set; }
            public string VehicleName { get; set; }
            public List<int> NodeIds { get; set; } = new();
            public DateTime MappedAt { get; set; }
            public DateTime LastUpdated { get; set; }

            public int NodeCount => NodeIds.Count;

            public override string ToString()
            {
                return $"Vehicle {VehicleName} (ID: {VehicleId}) → {NodeCount} nodes | Mapped: {MappedAt:g}";
            }
        }

        /// <summary>
        /// Vehicle Statistics for analytics
        /// </summary>
        public class VehicleStatistics
        {
            public int TotalVehicles { get; set; }
            public double AverageCostPerKm { get; set; }
            public double AverageSpeed { get; set; }
            public double AverageComfortRating { get; set; }
            public double AverageSafetyRating { get; set; }
            public double AverageCapacity { get; set; }
            public VehicleTypeDefinitions.VehicleType CheapestVehicle { get; set; }
            public VehicleTypeDefinitions.VehicleType MostExpensiveVehicle { get; set; }
            public VehicleTypeDefinitions.VehicleType FastestVehicle { get; set; }
            public VehicleTypeDefinitions.VehicleType SafestVehicle { get; set; }
            public VehicleTypeDefinitions.VehicleType MostComfortableVehicle { get; set; }
            public List<VehicleTypeDefinitions.VehicleType> GreenerVehicles { get; set; }
            public List<VehicleTypeDefinitions.VehicleType> BudgetVehicles { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════
         VEHICLE STATISTICS REPORT
═══════════════════════════════════════════
Total Vehicles:        {TotalVehicles}
Average Cost/km:       {AverageCostPerKm:F2} PKR
Average Speed:         {AverageSpeed:F0} km/h
Average Comfort:       {AverageComfortRating:F1}/5
Average Safety:        {AverageSafetyRating:F1}/5
Average Capacity:      {AverageCapacity:F0}

EXTREMES:
├─ Cheapest:           {CheapestVehicle?.Name} ({CheapestVehicle?.CostPerKm:F0} PKR/km)
├─ Most Expensive:     {MostExpensiveVehicle?.Name} ({MostExpensiveVehicle?.CostPerKm:F0} PKR/km)
├─ Fastest:            {FastestVehicle?.Name} ({FastestVehicle?.MaxSpeed} km/h)
├─ Safest:             {SafestVehicle?.Name} ({SafestVehicle?.SafetyRating}/5)
└─ Most Comfortable:   {MostComfortableVehicle?.Name} ({MostComfortableVehicle?.ComfortRating}/5)

ECO-FRIENDLY:          {GreenerVehicles?.Count ?? 0} vehicles
BUDGET-FRIENDLY:       {BudgetVehicles?.Count ?? 0} vehicles
═══════════════════════════════════════════
";
            }
        }
    }
}