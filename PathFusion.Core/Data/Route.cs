using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Data
{
    public  class Route
    {
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public TransportMode TransportMode { get; set; }
        public double Distance { get; set; } // in km
        public int Duration { get; set; } // in minutes
        public decimal Cost { get; set; } // in PKR
        public double Co2Emissions { get; set; } // in kg CO2
        public bool IsOperational { get; set; }
        public int Frequency { get; set; } // services per day
    }
    public enum TransportMode
    {
        Road = 1,
        Rail = 2,
        Air = 3
    }
}
