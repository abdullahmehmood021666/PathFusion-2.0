using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Models;
public class City
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Province { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public required string Color { get; set; }
    public required string Population { get; set; }
    public double MapX { get; set; }  // SVG canvas X coordinate
    public double MapY { get; set; }  // SVG canvas Y coordinate

    // Connections
    public List<int> RoadConnections { get; set; } = new();
    public List<int> RailConnections { get; set; } = new();
    public List<int> AirConnections { get; set; } = new();
}
