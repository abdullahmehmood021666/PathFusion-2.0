using PathFusion.Core.Models;

namespace PathFusion.Core.Data;

public static class PakistanCitiesData
{
    public static List<City> GetAllCities()
    {
        return new List<City>
        {
            // Northern Region
            new()
            {
                Id = 1, Name = "Islamabad", Province = "Federal",
                Latitude = 33.7298, Longitude = 73.1772, Color = "#3b82f6", Population = "2.1M",
                MapX = 420, MapY = 220,
                RoadConnections = new() { 2, 3, 4 },
                RailConnections = new() { 2 },
                AirConnections = new() { 5, 6, 7 }
            },
            new()
            {
                Id = 2, Name = "Peshawar", Province = "KPK",
                Latitude = 34.0151, Longitude = 71.5249, Color = "#10b981", Population = "2.3M",
                MapX = 350, MapY = 180,
                RoadConnections = new() { 1, 3 },
                RailConnections = new() { 1 },
                AirConnections = new() { 5 }
            },
            new()
            {
                Id = 3, Name = "Lahore", Province = "Punjab",
                Latitude = 31.5497, Longitude = 74.3436, Color = "#f59e0b", Population = "11M",
                MapX = 450, MapY = 300,
                RoadConnections = new() { 1, 2, 4, 5, 6 },
                RailConnections = new() { 1, 4, 5 },
                AirConnections = new() { 5, 6, 7 }
            },

            // Central Region
            new()
            {
                Id = 4, Name = "Multan", Province = "Punjab",
                Latitude = 30.1575, Longitude = 71.4454, Color = "#ec4899", Population = "1.9M",
                MapX = 420, MapY = 420,
                RoadConnections = new() { 3, 5, 6, 7 },
                RailConnections = new() { 3, 5 },
                AirConnections = new() { 6 }
            },
            new()
            {
                Id = 5, Name = "Faisalabad", Province = "Punjab",
                Latitude = 30.1884, Longitude = 72.6360, Color = "#14b8a6", Population = "3.2M",
                MapX = 460, MapY = 350,
                RoadConnections = new() { 3, 4, 6 },
                RailConnections = new() { 3, 4 },
                AirConnections = new() { 6 }
            },
            new()
            {
                Id = 6, Name = "Rawalpindi", Province = "Punjab",
                Latitude = 33.5731, Longitude = 73.1898, Color = "#8b5cf6", Population = "2.2M",
                MapX = 430, MapY = 260,
                RoadConnections = new() { 1, 3, 4, 5, 8 },
                RailConnections = new() { 1, 3 },
                AirConnections = new() { 5 }
            },

            // Southern Region
            new()
            {
                Id = 7, Name = "Karachi", Province = "Sindh",
                Latitude = 24.8607, Longitude = 67.0011, Color = "#ef4444", Population = "16M",
                MapX = 380, MapY = 600,
                RoadConnections = new() { 4, 8, 9 },
                RailConnections = new() { 4, 8 },
                AirConnections = new() { 5, 6 }
            },
            new()
            {
                Id = 8, Name = "Hyderabad", Province = "Sindh",
                Latitude = 25.3960, Longitude = 68.3578, Color = "#f97316", Population = "1.8M",
                MapX = 420, MapY = 550,
                RoadConnections = new() { 6, 7, 9 },
                RailConnections = new() { 4, 7 },
                AirConnections = new() { 6 }
            },
            new()
            {
                Id = 9, Name = "Quetta", Province = "Balochistan",
                Latitude = 30.1798, Longitude = 67.0158, Color = "#8b5cf6", Population = "1.1M",
                MapX = 250, MapY = 400,
                RoadConnections = new() { 4, 7, 8 },
                RailConnections = new() { 4 },
                AirConnections = new() { 6 }
            }
        };
    }

    public static List<Route> GetAllRoutes()
    {
        return new List<Route>
        {
            // Islamabad connections
            new() { Id = 1, FromCityId = 1, ToCityId = 2, Mode = TransportMode.Road, Distance = 160, Duration = 180, Cost = 3000, Co2Emissions = 48 },
            new() { Id = 2, FromCityId = 1, ToCityId = 3, Mode = TransportMode.Road, Distance = 330, Duration = 360, Cost = 5500, Co2Emissions = 99 },
            new() { Id = 3, FromCityId = 1, ToCityId = 3, Mode = TransportMode.Rail, Distance = 330, Duration = 540, Cost = 2500, Co2Emissions = 50 },
            
            // Peshawar connections
            new() { Id = 4, FromCityId = 2, ToCityId = 3, Mode = TransportMode.Road, Distance = 500, Duration = 600, Cost = 8000, Co2Emissions = 150 },
            
            // Lahore connections
            new() { Id = 5, FromCityId = 3, ToCityId = 4, Mode = TransportMode.Road, Distance = 280, Duration = 320, Cost = 4500, Co2Emissions = 84 },
            new() { Id = 6, FromCityId = 3, ToCityId = 5, Mode = TransportMode.Road, Distance = 170, Duration = 200, Cost = 2800, Co2Emissions = 51 },
            new() { Id = 7, FromCityId = 3, ToCityId = 6, Mode = TransportMode.Road, Distance = 50, Duration = 60, Cost = 1000, Co2Emissions = 15 },
            
            // Multan connections
            new() { Id = 8, FromCityId = 4, ToCityId = 5, Mode = TransportMode.Road, Distance = 200, Duration = 240, Cost = 3200, Co2Emissions = 60 },
            new() { Id = 9, FromCityId = 4, ToCityId = 8, Mode = TransportMode.Road, Distance = 300, Duration = 360, Cost = 5000, Co2Emissions = 90 },
            
            // Karachi connections
            new() { Id = 10, FromCityId = 7, ToCityId = 8, Mode = TransportMode.Road, Distance = 160, Duration = 180, Cost = 2500, Co2Emissions = 48 },
            new() { Id = 11, FromCityId = 7, ToCityId = 4, Mode = TransportMode.Road, Distance = 750, Duration = 900, Cost = 12000, Co2Emissions = 225 },
            new() { Id = 12, FromCityId = 7, ToCityId = 4, Mode = TransportMode.Rail, Distance = 750, Duration = 1440, Cost = 5000, Co2Emissions = 100 },
            
            // Air routes
            new() { Id = 13, FromCityId = 1, ToCityId = 7, Mode = TransportMode.Air, Distance = 1400, Duration = 100, Cost = 15000, Co2Emissions = 280 },
            new() { Id = 14, FromCityId = 3, ToCityId = 7, Mode = TransportMode.Air, Distance = 1050, Duration = 75, Cost = 12000, Co2Emissions = 210 }
        };
    }
}
