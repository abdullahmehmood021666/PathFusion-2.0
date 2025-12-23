using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Models;

public class Route
{
    public int Id { get; set; }
    public int FromCityId { get; set; }
    public int ToCityId { get; set; }
    public TransportMode Mode { get; set; }

    // Metrics
    public int Distance { get; set; }  // km
    public int Duration { get; set; }  // minutes
    public decimal Cost { get; set; }  // PKR
    public int Co2Emissions { get; set; }  // grams

    // Flags
    public bool IsOperational { get; set; } = true;
    public int Frequency { get; set; }  // trips per day (for Rail/Air)
}
