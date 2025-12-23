using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Data;
using PathFusion.Core.Graph;

namespace PathFusion.Core.Optimization
{
    public class PathOptimizerService
    {
        private List<OptimizedPath> _allPaths;
        private List<OptimizedPath> _rankedPaths;

        public PathOptimizerService()
        {
            _allPaths = new();
            _rankedPaths = new();
        }

        // ============================================================================
        // PATH RANKING BY SINGLE CRITERIA
        // ============================================================================

        /// <summary>
        /// Rank all paths by distance - returns top 3 shortest paths
        /// </summary>
        public List<OptimizedPath> RankPathsByDistance(List<PathData> allPaths)
        {
            if (allPaths == null || allPaths.Count == 0)
                return new List<OptimizedPath>();

            _allPaths = allPaths
                .Select((p, index) => new OptimizedPath
                {
                    PathId = index,
                    NodePath = p.NodePath,
                    Distance = p.Distance,
                    TravelTime = p.TravelTime,
                    EstimatedCost = p.EstimatedCost,
                    Hops = p.Hops,
                    Rank = 0,
                    Score = p.Distance,
                    OptimizationType = "Distance"
                })
                .OrderBy(p => p.Distance)
                .ToList();

            UpdateRanks();
            return _rankedPaths = _allPaths.Take(3).ToList();
        }

        /// <summary>
        /// Rank all paths by travel time - returns top 3 fastest paths
        /// </summary>
        public List<OptimizedPath> RankPathsByTime(List<PathData> allPaths)
        {
            if (allPaths == null || allPaths.Count == 0)
                return new List<OptimizedPath>();

            _allPaths = allPaths
                .Select((p, index) => new OptimizedPath
                {
                    PathId = index,
                    NodePath = p.NodePath,
                    Distance = p.Distance,
                    TravelTime = p.TravelTime,
                    EstimatedCost = p.EstimatedCost,
                    Hops = p.Hops,
                    Rank = 0,
                    Score = p.TravelTime,
                    OptimizationType = "Time"
                })
                .OrderBy(p => p.TravelTime)
                .ToList();

            UpdateRanks();
            return _rankedPaths = _allPaths.Take(3).ToList();
        }

        /// <summary>
        /// Rank all paths by cost - returns top 3 cheapest paths
        /// </summary>
        public List<OptimizedPath> RankPathsByCost(List<PathData> allPaths)
        {
            if (allPaths == null || allPaths.Count == 0)
                return new List<OptimizedPath>();

            _allPaths = allPaths
                .Select((p, index) => new OptimizedPath
                {
                    PathId = index,
                    NodePath = p.NodePath,
                    Distance = p.Distance,
                    TravelTime = p.TravelTime,
                    EstimatedCost = p.EstimatedCost,
                    Hops = p.Hops,
                    Rank = 0,
                    Score = p.EstimatedCost,
                    OptimizationType = "Cost"
                })
                .OrderBy(p => p.EstimatedCost)
                .ToList();

            UpdateRanks();
            return _rankedPaths = _allPaths.Take(3).ToList();
        }

        /// <summary>
        /// Rank all paths by hop count - returns top 3 shortest paths by hops
        /// </summary>
        public List<OptimizedPath> RankPathsByHops(List<PathData> allPaths)
        {
            if (allPaths == null || allPaths.Count == 0)
                return new List<OptimizedPath>();

            _allPaths = allPaths
                .Select((p, index) => new OptimizedPath
                {
                    PathId = index,
                    NodePath = p.NodePath,
                    Distance = p.Distance,
                    TravelTime = p.TravelTime,
                    EstimatedCost = p.EstimatedCost,
                    Hops = p.Hops,
                    Rank = 0,
                    Score = p.Hops,
                    OptimizationType = "Hops"
                })
                .OrderBy(p => p.Hops)
                .ToList();

            UpdateRanks();
            return _rankedPaths = _allPaths.Take(3).ToList();
        }

        // ============================================================================
        // MULTI-CRITERIA PATH RANKING
        // ============================================================================

        /// <summary>
        /// Rank paths using weighted multi-criteria scoring
        /// </summary>
        public List<OptimizedPath> RankPathsByMultiCriteria(
            List<PathData> allPaths,
            double distanceWeight = 0.3,
            double timeWeight = 0.3,
            double costWeight = 0.4)
        {
            if (allPaths == null || allPaths.Count == 0)
                return new List<OptimizedPath>();

            // Normalize metrics
            var maxDistance = allPaths.Max(p => p.Distance);
            var maxTime = allPaths.Max(p => p.TravelTime);
            var maxCost = allPaths.Max(p => p.EstimatedCost);

            _allPaths = allPaths
                .Select((p, index) => new OptimizedPath
                {
                    PathId = index,
                    NodePath = p.NodePath,
                    Distance = p.Distance,
                    TravelTime = p.TravelTime,
                    EstimatedCost = p.EstimatedCost,
                    Hops = p.Hops,
                    Rank = 0,
                    Score = CalculateWeightedScore(
                        p.Distance, p.TravelTime, p.EstimatedCost,
                        maxDistance, maxTime, maxCost,
                        distanceWeight, timeWeight, costWeight),
                    OptimizationType = "Multi-Criteria",
                    DistanceWeight = distanceWeight,
                    TimeWeight = timeWeight,
                    CostWeight = costWeight
                })
                .OrderBy(p => p.Score)
                .ToList();

            UpdateRanks();
            return _rankedPaths = _allPaths.Take(3).ToList();
        }

        /// <summary>
        /// Rank paths with dynamic weighting based on user preferences
        /// </summary>
        public List<OptimizedPath> RankPathsByPreference(
            List<PathData> allPaths,
            PathPreference preference)
        {
            double distanceWeight = 0.3, timeWeight = 0.3, costWeight = 0.4;

            return preference switch
            {
                PathPreference.ShortestDistance => RankPathsByDistance(allPaths),
                PathPreference.FastestTime => RankPathsByTime(allPaths),
                PathPreference.CheapestCost => RankPathsByCost(allPaths),
                PathPreference.FewestHops => RankPathsByHops(allPaths),
                PathPreference.BalancedEconomy => RankPathsByMultiCriteria(allPaths, 0.3, 0.3, 0.4),
                PathPreference.BalancedSpeed => RankPathsByMultiCriteria(allPaths, 0.2, 0.5, 0.3),
                PathPreference.DistancePriority => RankPathsByMultiCriteria(allPaths, 0.6, 0.2, 0.2),
                PathPreference.TimePriority => RankPathsByMultiCriteria(allPaths, 0.2, 0.6, 0.2),
                PathPreference.CostPriority => RankPathsByMultiCriteria(allPaths, 0.2, 0.2, 0.6),
                _ => RankPathsByMultiCriteria(allPaths)
            };
        }

        // ============================================================================
        // FILTERED & CONSTRAINED RANKING
        // ============================================================================

        /// <summary>
        /// Rank paths with constraints (e.g., max distance, max cost)
        /// </summary>
        public List<OptimizedPath> RankPathsWithConstraints(
            List<PathData> allPaths,
            double? maxDistance = null,
            double? maxTime = null,
            double? maxCost = null,
            int? maxHops = null)
        {
            var filtered = allPaths
                .Where(p => !maxDistance.HasValue || p.Distance <= maxDistance.Value)
                .Where(p => !maxTime.HasValue || p.TravelTime <= maxTime.Value)
                .Where(p => !maxCost.HasValue || p.EstimatedCost <= maxCost.Value)
                .Where(p => !maxHops.HasValue || p.Hops <= maxHops.Value)
                .ToList();

            return RankPathsByDistance(filtered);
        }

        /// <summary>
        /// Rank paths excluding certain nodes (avoid toll roads, etc.)
        /// </summary>
        public List<OptimizedPath> RankPathsWithExclusions(
            List<PathData> allPaths,
            List<int> excludedNodeIds,
            string optimizeBy = "distance")
        {
            var filtered = allPaths
                .Where(p => !p.NodePath.Intersect(excludedNodeIds).Any())
                .ToList();

            return optimizeBy.ToLower() switch
            {
                "distance" => RankPathsByDistance(filtered),
                "time" => RankPathsByTime(filtered),
                "cost" => RankPathsByCost(filtered),
                "hops" => RankPathsByHops(filtered),
                _ => RankPathsByDistance(filtered)
            };
        }

        // ============================================================================
        // PERCENTILE & STATISTICAL ANALYSIS
        // ============================================================================

        /// <summary>
        /// Get percentile analysis of paths
        /// </summary>
        public PathPercentileAnalysis AnalyzePathPercentiles(List<PathData> allPaths)
        {
            if (allPaths == null || allPaths.Count == 0)
                return null;

            var distances = allPaths.Select(p => p.Distance).OrderBy(d => d).ToList();
            var times = allPaths.Select(p => p.TravelTime).OrderBy(t => t).ToList();
            var costs = allPaths.Select(p => p.EstimatedCost).OrderBy(c => c).ToList();

            return new PathPercentileAnalysis
            {
                TotalPaths = allPaths.Count,
                DistancePercentiles = new Dictionary<string, double>
            {
                { "Min", distances.First() },
                { "25th", GetPercentile(distances, 0.25) },
                { "50th (Median)", GetPercentile(distances, 0.5) },
                { "75th", GetPercentile(distances, 0.75) },
                { "Max", distances.Last() }
            },
                TimePercentiles = new Dictionary<string, double>
            {
                { "Min", times.First() },
                { "25th", GetPercentile(times, 0.25) },
                { "50th (Median)", GetPercentile(times, 0.5) },
                { "75th", GetPercentile(times, 0.75) },
                { "Max", times.Last() }
            },
                CostPercentiles = new Dictionary<string, double>
            {
                { "Min", costs.First() },
                { "25th", GetPercentile(costs, 0.25) },
                { "50th (Median)", GetPercentile(costs, 0.5) },
                { "75th", GetPercentile(costs, 0.75) },
                { "Max", costs.Last() }
            },
                DistanceStdDev = CalculateStdDev(distances),
                TimeStdDev = CalculateStdDev(times),
                CostStdDev = CalculateStdDev(costs)
            };
        }

        /// <summary>
        /// Compare top 3 paths with statistics
        /// </summary>
        public PathComparisonAnalysis CompareTopThreePaths(List<OptimizedPath> topPaths)
        {
            if (topPaths == null || topPaths.Count == 0)
                return null;

            var distanceDiff = topPaths.Count > 1 ? topPaths[1].Distance - topPaths[0].Distance : 0;
            var timeDiff = topPaths.Count > 1 ? topPaths[1].TravelTime - topPaths[0].TravelTime : 0;
            var costDiff = topPaths.Count > 1 ? topPaths[1].EstimatedCost - topPaths[0].EstimatedCost : 0;

            var distanceDiffPercent = topPaths[0].Distance > 0 ? (distanceDiff / topPaths[0].Distance) * 100 : 0;
            var timeDiffPercent = topPaths[0].TravelTime > 0 ? (timeDiff / topPaths[0].TravelTime) * 100 : 0;
            var costDiffPercent = topPaths[0].EstimatedCost > 0 ? (costDiff / topPaths[0].EstimatedCost) * 100 : 0;

            return new PathComparisonAnalysis
            {
                TopPath = topPaths[0],
                SecondPath = topPaths.Count > 1 ? topPaths[1] : null,
                ThirdPath = topPaths.Count > 2 ? topPaths[2] : null,
                DistanceDifference = distanceDiff,
                DistanceDifferencePercent = distanceDiffPercent,
                TimeDifference = timeDiff,
                TimeDifferencePercent = timeDiffPercent,
                CostDifference = costDiff,
                CostDifferencePercent = costDiffPercent,
                BestOverallPath = topPaths[0]
            };
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        /// <summary>
        /// Calculate weighted score for multi-criteria ranking
        /// </summary>
        private double CalculateWeightedScore(
            double distance, double time, double cost,
            double maxDistance, double maxTime, double maxCost,
            double distanceWeight, double timeWeight, double costWeight)
        {
            var normalizedDistance = maxDistance > 0 ? distance / maxDistance : 0;
            var normalizedTime = maxTime > 0 ? time / maxTime : 0;
            var normalizedCost = maxCost > 0 ? cost / maxCost : 0;

            return (normalizedDistance * distanceWeight) +
                   (normalizedTime * timeWeight) +
                   (normalizedCost * costWeight);
        }

        /// <summary>
        /// Update rank numbers after sorting
        /// </summary>
        private void UpdateRanks()
        {
            for (int i = 0; i < _allPaths.Count; i++)
            {
                _allPaths[i].Rank = i + 1;
            }
        }

        /// <summary>
        /// Get percentile value from sorted list
        /// </summary>
        private double GetPercentile(List<double> sortedList, double percentile)
        {
            if (sortedList == null || sortedList.Count == 0)
                return 0;

            int index = (int)((percentile * sortedList.Count) - 1);
            index = Math.Max(0, Math.Min(index, sortedList.Count - 1));
            return sortedList[index];
        }

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        private double CalculateStdDev(List<double> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            var mean = values.Average();
            var variance = values.Average(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(variance);
        }

        // ============================================================================
        // NESTED DATA STRUCTURES
        // ============================================================================

        /// <summary>
        /// Optimized Path - ranked path with score
        /// </summary>
        public class OptimizedPath
        {
            public int PathId { get; set; }
            public List<int> NodePath { get; set; } = new();
            public double Distance { get; set; }
            public double TravelTime { get; set; }
            public double EstimatedCost { get; set; }
            public int Hops { get; set; }
            public int Rank { get; set; }
            public double Score { get; set; }
            public string OptimizationType { get; set; }
            public double DistanceWeight { get; set; }
            public double TimeWeight { get; set; }
            public double CostWeight { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public override string ToString()
            {
                var pathString = string.Join(" → ", NodePath);
                return $@"
╔═══════════════════════════════════════════╗
║ Path Rank #{Rank} ({OptimizationType})
╠═══════════════════════════════════════════╣
║ Node Path:       {pathString}
║ Distance:        {Distance,24:F2} km
║ Travel Time:     {TravelTime,24:F2} hours
║ Estimated Cost:  {EstimatedCost,24:F0} PKR
║ Hops:            {Hops,26}
║ Score:           {Score,25:F4}
╚═══════════════════════════════════════════╝
";
            }
        }

        /// <summary>
        /// Path Preference - user preference enum
        /// </summary>
        public enum PathPreference
        {
            ShortestDistance,
            FastestTime,
            CheapestCost,
            FewestHops,
            BalancedEconomy,
            BalancedSpeed,
            DistancePriority,
            TimePriority,
            CostPriority
        }

        /// <summary>
        /// Path Data - input path data
        /// </summary>
        public class PathData
        {
            public List<int> NodePath { get; set; } = new();
            public double Distance { get; set; }
            public double TravelTime { get; set; }
            public double EstimatedCost { get; set; }
            public int Hops { get; set; }
        }

        /// <summary>
        /// Path Percentile Analysis
        /// </summary>
        public class PathPercentileAnalysis
        {
            public int TotalPaths { get; set; }
            public Dictionary<string, double> DistancePercentiles { get; set; } = new();
            public Dictionary<string, double> TimePercentiles { get; set; } = new();
            public Dictionary<string, double> CostPercentiles { get; set; } = new();
            public double DistanceStdDev { get; set; }
            public double TimeStdDev { get; set; }
            public double CostStdDev { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
        PATH PERCENTILE ANALYSIS
═══════════════════════════════════════════════
Total Paths Analyzed: {TotalPaths}

DISTANCE PERCENTILES:
├─ Min:    {DistancePercentiles["Min"]:F2} km
├─ 25th:   {DistancePercentiles["25th"]:F2} km
├─ Median: {DistancePercentiles["50th (Median)"]:F2} km
├─ 75th:   {DistancePercentiles["75th"]:F2} km
├─ Max:    {DistancePercentiles["Max"]:F2} km
└─ StdDev: {DistanceStdDev:F2} km

TIME PERCENTILES:
├─ Min:    {TimePercentiles["Min"]:F2} hours
├─ 25th:   {TimePercentiles["25th"]:F2} hours
├─ Median: {TimePercentiles["50th (Median)"]:F2} hours
├─ 75th:   {TimePercentiles["75th"]:F2} hours
├─ Max:    {TimePercentiles["Max"]:F2} hours
└─ StdDev: {TimeStdDev:F2} hours

COST PERCENTILES:
├─ Min:    {CostPercentiles["Min"]:F0} PKR
├─ 25th:   {CostPercentiles["25th"]:F0} PKR
├─ Median: {CostPercentiles["50th (Median)"]:F0} PKR
├─ 75th:   {CostPercentiles["75th"]:F0} PKR
├─ Max:    {CostPercentiles["Max"]:F0} PKR
└─ StdDev: {CostStdDev:F0} PKR

═══════════════════════════════════════════════
";
            }
        }

        /// <summary>
        /// Path Comparison Analysis
        /// </summary>
        public class PathComparisonAnalysis
        {
            public OptimizedPath TopPath { get; set; }
            public OptimizedPath SecondPath { get; set; }
            public OptimizedPath ThirdPath { get; set; }
            public double DistanceDifference { get; set; }
            public double DistanceDifferencePercent { get; set; }
            public double TimeDifference { get; set; }
            public double TimeDifferencePercent { get; set; }
            public double CostDifference { get; set; }
            public double CostDifferencePercent { get; set; }
            public OptimizedPath BestOverallPath { get; set; }

            public override string ToString()
            {
                return $@"
═══════════════════════════════════════════════
        TOP 3 PATHS COMPARISON
═══════════════════════════════════════════════
🥇 BEST PATH (Rank #{TopPath?.Rank})
   Distance: {TopPath?.Distance:F2} km
   Time:     {TopPath?.TravelTime:F2} hours
   Cost:     {TopPath?.EstimatedCost:F0} PKR

🥈 SECOND PATH
   Distance: {SecondPath?.Distance:F2} km (+{DistanceDifference:F2} km / +{DistanceDifferencePercent:F1}%)
   Time:     {SecondPath?.TravelTime:F2} hours (+{TimeDifference:F2} hours / +{TimeDifferencePercent:F1}%)
   Cost:     {SecondPath?.EstimatedCost:F0} PKR (+{CostDifference:F0} PKR / +{CostDifferencePercent:F1}%)

🥉 THIRD PATH
   Distance: {ThirdPath?.Distance:F2} km
   Time:     {ThirdPath?.TravelTime:F2} hours
   Cost:     {ThirdPath?.EstimatedCost:F0} PKR

═══════════════════════════════════════════════
";
            }
        }
    }
}
