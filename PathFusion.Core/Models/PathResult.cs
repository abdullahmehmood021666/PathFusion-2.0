using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PathFusion.Core.Models;

public class PathResult
{
    public required string FromCity { get; set; }
    public required string ToCity { get; set; }
    public TransportMode RequestedMode { get; set; }

    // Three optimization results
    public Path? ShortestPath { get; set; }  // By distance
    public Path? FastestPath { get; set; }   // By duration
    public Path? CheapestPath { get; set; }  // By cost

    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.Now;
}
