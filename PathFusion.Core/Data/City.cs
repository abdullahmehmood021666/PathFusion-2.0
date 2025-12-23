using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Data
{
    public  class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Elevation { get; set; }
        public long Population { get; set; }
        public string EconomyType { get; set; }
        public string Color { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public bool IsCapital { get; set; }
        public bool IsPort { get; set; }
        public bool HasAirport { get; set; }
        public bool HasRailway { get; set; }
        public bool IsMetroCity { get; set; }
        public string TimeZone { get; set; }
        public string LanguageSpoken { get; set; }
        public string District { get; set; }
        public List<int> RoadConnections { get; set; } = new();
        public List<int> RailConnections { get; set; } = new();
        public List<int> AirConnections { get; set; } = new();
    }
}
