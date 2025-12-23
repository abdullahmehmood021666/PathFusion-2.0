using PathFusion.Core.Data;
using PathFusion.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Path = PathFusion.Core.Models.Path;

namespace PathFusion.Core.Services;

public class MultiModalRouter
{
    private readonly List<City> _cities;
    private readonly List<Route> _routes;

    public MultiModalRouter()
    {
        _cities = PakistanCitiesData.GetAllCities();
        _routes = PakistanCitiesData.GetAllRoutes();
    }

    public PathResult FindPath(string fromCity, string toCity, TransportMode mode)
    {
        var result = new PathResult
        {
            FromCity = fromCity,
            ToCity = toCity,
            RequestedMode = mode
        };

        var from = _cities.FirstOrDefault(c => c.Name == fromCity);
        var to = _cities.FirstOrDefault(c => c.Name == toCity);

        if (from == null || to == null)
        {
            result.Success = false;
            result.ErrorMessage = "One or both cities not found";
            return result;
        }

        // Dijkstra for each optimization criterion
        result.ShortestPath = DijkstraByDistance(from.Id, to.Id, mode);
        result.FastestPath = DijkstraByDuration(from.Id, to.Id, mode);
        result.CheapestPath = DijkstraByPrice(from.Id, to.Id, mode);

        result.Success = result.ShortestPath != null;

        if (!result.Success)
        {
            result.ErrorMessage = "No path found between cities";
        }

        return result;
    }

    private Path? DijkstraByDistance(int start, int end, TransportMode mode)
    {
        var distances = new Dictionary<int, int>();
        var previous = new Dictionary<int, int?>();
        var unvisited = new HashSet<int>();

        foreach (var city in _cities)
        {
            distances[city.Id] = int.MaxValue;
            previous[city.Id] = null;
            unvisited.Add(city.Id);
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(u => distances[u]).First();

            if (current == end)
                return ReconstructPath(start, end, previous, mode);

            if (distances[current] == int.MaxValue)
                break;

            unvisited.Remove(current);

            var neighbors = GetNeighbors(current, mode);
            foreach (var neighbor in neighbors)
            {
                if (!unvisited.Contains(neighbor))
                    continue;

                var route = GetRoute(current, neighbor, mode);
                if (route == null)
                    continue;

                var newDist = distances[current] + route.Distance;
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                }
            }
        }

        return null;
    }

    private Path? DijkstraByDuration(int start, int end, TransportMode mode)
    {
        var durations = new Dictionary<int, int>();
        var previous = new Dictionary<int, int?>();
        var unvisited = new HashSet<int>();

        foreach (var city in _cities)
        {
            durations[city.Id] = int.MaxValue;
            previous[city.Id] = null;
            unvisited.Add(city.Id);
        }

        durations[start] = 0;

        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(u => durations[u]).First();

            if (current == end)
                return ReconstructPath(start, end, previous, mode);

            if (durations[current] == int.MaxValue)
                break;

            unvisited.Remove(current);

            var neighbors = GetNeighbors(current, mode);
            foreach (var neighbor in neighbors)
            {
                if (!unvisited.Contains(neighbor))
                    continue;

                var route = GetRoute(current, neighbor, mode);
                if (route == null)
                    continue;

                var newDur = durations[current] + route.Duration;
                if (newDur < durations[neighbor])
                {
                    durations[neighbor] = newDur;
                    previous[neighbor] = current;
                }
            }
        }

        return null;
    }

    private Path? DijkstraByPrice(int start, int end, TransportMode mode)
    {
        var costs = new Dictionary<int, decimal>();
        var previous = new Dictionary<int, int?>();
        var unvisited = new HashSet<int>();

        foreach (var city in _cities)
        {
            costs[city.Id] = decimal.MaxValue;
            previous[city.Id] = null;
            unvisited.Add(city.Id);
        }

        costs[start] = 0;

        while (unvisited.Count > 0)
        {
            var current = unvisited.OrderBy(u => costs[u]).First();

            if (current == end)
                return ReconstructPath(start, end, previous, mode);

            if (costs[current] == decimal.MaxValue)
                break;

            unvisited.Remove(current);

            var neighbors = GetNeighbors(current, mode);
            foreach (var neighbor in neighbors)
            {
                if (!unvisited.Contains(neighbor))
                    continue;

                var route = GetRoute(current, neighbor, mode);
                if (route == null)
                    continue;

                var newCost = costs[current] + route.Cost;
                if (newCost < costs[neighbor])
                {
                    costs[neighbor] = newCost;
                    previous[neighbor] = current;
                }
            }
        }

        return null;
    }

    private Path? ReconstructPath(int start, int end, Dictionary<int, int?> previous, TransportMode mode)
    {
        var sequence = new List<int>();
        int? current = end;

        while (current.HasValue)
        {
            sequence.Insert(0, current.Value);
            current = previous[current.Value];
        }


        if (sequence[0] != start)
            return null;

        var path = new Path { CitySequence = sequence };

        for (int i = 0; i < sequence.Count - 1; i++)
        {
            var route = GetRoute(sequence[i], sequence[i + 1], mode);
            if (route == null)
                return null;

            path.TotalDistance += route.Distance;
            path.TotalDuration += route.Duration;
            path.TotalCost += route.Cost;
            path.TotalEmissions += route.Co2Emissions;
            path.ModeSequence.Add(route.Mode);
        }

        return path;
    }

    private List<int> GetNeighbors(int cityId, TransportMode mode)
    {
        var city = _cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return new();

        return mode switch
        {
            TransportMode.Road => city.RoadConnections,
            TransportMode.Rail => city.RailConnections,
            TransportMode.Air => city.AirConnections,
            TransportMode.Multi => city.RoadConnections
                .Concat(city.RailConnections)
                .Concat(city.AirConnections)
                .Distinct()
                .ToList(),
            _ => new()
        };
    }

    private Route? GetRoute(int fromId, int toId, TransportMode mode)
    {
        return _routes.FirstOrDefault(r =>
            (r.FromCityId == fromId && r.ToCityId == toId && r.Mode == mode) ||
            (r.FromCityId == toId && r.ToCityId == fromId && r.Mode == mode)
        );
    }
}
