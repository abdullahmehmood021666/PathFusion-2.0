using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Models;

public class Path
{
    public List<int> CitySequence { get; set; } = new();  // City IDs in order
    public int TotalDistance { get; set; }  // km
    public int TotalDuration { get; set; }  // minutes
    public decimal TotalCost { get; set; }  // PKR
    public int TotalEmissions { get; set; }  // grams
    public List<TransportMode> ModeSequence { get; set; } = new();  // Transport mode per segment

    public override string ToString()
    {
        return $"{TotalDistance}km | {TotalDuration}min | PKR {TotalCost} | {TotalEmissions}g CO2";
    }
}
