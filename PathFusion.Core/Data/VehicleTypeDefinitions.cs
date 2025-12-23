using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFusion.Core.Data
{
    public static class VehicleTypeDefinitions
    {
        // ============================================================================
        // SECTION 1: ROAD VEHICLES (6 Types)
        // ============================================================================

        public static List<VehicleType> GetAllVehicleTypes()
        {
            return new List<VehicleType>
        {
            // ========== ROAD VEHICLES ==========

            new()
            {
                Id = 1,
                Name = "Motorcycle",
                Urdu = "موٹرسائیکل",
                Category = "Road",
                TransportMode = "Road",
                Color = "#ef4444",
                Capacity = 2,
                MaxSpeed = 120,
                AverageSpeed = 80,
                CostPerKm = 2.5M,
                Co2PerKm = 0.05,
                FuelType = "Petrol",
                ComfortRating = 2,
                Amenities = new() { "Basic" },
                Cost = 200000,
                MaintenanceCostPerYear = 5000,
                InsuranceCostPerYear = 3000,
                LifeSpan = 10,
                MaxRange = 400,
                FuelCapacity = 12,
                Emission = "Euro 2",
                EmissionStandard = 2,
                SafetyRating = 2,
                Acceleration = 0.12,
                BrakingDistance = 40,
                TurningRadius = 3.5,
                WheelDrive = "2WD",
                Transmission = "Manual",
                GearRatio = 4.2,
                CurbWeight = 160,
                MaxGVWR = 320,
                AxleCount = 2,
                TireSize = "17 Inch",
                SuspensionType = "Telescopic",
                GroundClearance = 0.16,
                FuelTankType = "Steel",
                CoolingSystems = new() { "Air Cooled" },
                ParkingSpace = 0.8,
                BestForRoute = "Short distance, traffic congestion",
                OperatingCost = "Low",
                EnvironmentalImpact = "Moderate",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110 }
            },

            new()
            {
                Id = 2,
                Name = "Auto-Rickshaw",
                Urdu = "آٹو رکشہ",
                Category = "Road",
                TransportMode = "Road",
                Color = "#f97316",
                Capacity = 4,
                MaxSpeed = 100,
                AverageSpeed = 60,
                CostPerKm = 3.5M,
                Co2PerKm = 0.12,
                FuelType = "CNG/Petrol",
                ComfortRating = 3,
                Amenities = new() { "Seating", "Basic AC" },
                Cost = 350000,
                MaintenanceCostPerYear = 8000,
                InsuranceCostPerYear = 5000,
                LifeSpan = 8,
                MaxRange = 350,
                FuelCapacity = 35,
                Emission = "Euro 3",
                EmissionStandard = 3,
                SafetyRating = 3,
                Acceleration = 0.09,
                BrakingDistance = 50,
                TurningRadius = 4.2,
                WheelDrive = "FWD",
                Transmission = "Manual",
                GearRatio = 3.8,
                CurbWeight = 650,
                MaxGVWR = 1100,
                AxleCount = 2,
                TireSize = "13 Inch",
                SuspensionType = "Leaf Spring",
                GroundClearance = 0.18,
                FuelTankType = "Steel",
                CoolingSystems = new() { "Water Cooled" },
                ParkingSpace = 1.2,
                BestForRoute = "City transport, short routes",
                OperatingCost = "Low",
                EnvironmentalImpact = "Moderate",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 }
            },

            new()
            {
                Id = 3,
                Name = "Car",
                Urdu = "کار",
                Category = "Road",
                TransportMode = "Road",
                Color = "#8b5cf6",
                Capacity = 5,
                MaxSpeed = 180,
                AverageSpeed = 100,
                CostPerKm = 6.0M,
                Co2PerKm = 0.18,
                FuelType = "Petrol/Diesel",
                ComfortRating = 4,
                Amenities = new() { "AC", "Power Steering", "Airbags", "Music System" },
                Cost = 1500000,
                MaintenanceCostPerYear = 20000,
                InsuranceCostPerYear = 12000,
                LifeSpan = 12,
                MaxRange = 600,
                FuelCapacity = 50,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 4,
                Acceleration = 0.10,
                BrakingDistance = 35,
                TurningRadius = 5.5,
                WheelDrive = "FWD",
                Transmission = "Automatic",
                GearRatio = 3.5,
                CurbWeight = 1200,
                MaxGVWR = 1600,
                AxleCount = 2,
                TireSize = "15 Inch",
                SuspensionType = "Independent Double Wishbone",
                GroundClearance = 0.15,
                FuelTankType = "Plastic",
                CoolingSystems = new() { "Water Cooled" },
                ParkingSpace = 2.0,
                BestForRoute = "Highway, long distance",
                OperatingCost = "Medium",
                EnvironmentalImpact = "Moderate",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119 }
            },

            new()
            {
                Id = 4,
                Name = "Bus",
                Urdu = "بس",
                Category = "Road",
                TransportMode = "Road",
                Color = "#14b8a6",
                Capacity = 50,
                MaxSpeed = 120,
                AverageSpeed = 70,
                CostPerKm = 15.0M,
                Co2PerKm = 0.45,
                FuelType = "Diesel",
                ComfortRating = 3,
                Amenities = new() { "Seating", "AC", "Suspension System", "Conductor Service" },
                Cost = 3500000,
                MaintenanceCostPerYear = 50000,
                InsuranceCostPerYear = 30000,
                LifeSpan = 15,
                MaxRange = 500,
                FuelCapacity = 150,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 4,
                Acceleration = 0.07,
                BrakingDistance = 60,
                TurningRadius = 8.5,
                WheelDrive = "RWD",
                Transmission = "Manual",
                GearRatio = 4.5,
                CurbWeight = 6000,
                MaxGVWR = 11000,
                AxleCount = 2,
                TireSize = "22.5 Inch",
                SuspensionType = "Air Suspension",
                GroundClearance = 0.30,
                FuelTankType = "Steel",
                CoolingSystems = new() { "Water Cooled" },
                ParkingSpace = 3.5,
                BestForRoute = "City transport, mass transit",
                OperatingCost = "Low per passenger",
                EnvironmentalImpact = "Moderate",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120 }
            },

            new()
            {
                Id = 5,
                Name = "Truck",
                Urdu = "ٹرک",
                Category = "Road",
                TransportMode = "Road",
                Color = "#06b6d4",
                Capacity = 20000,
                MaxSpeed = 100,
                AverageSpeed = 60,
                CostPerKm = 25.0M,
                Co2PerKm = 0.85,
                FuelType = "Diesel",
                ComfortRating = 2,
                Amenities = new() { "Basic Cabin", "AC" },
                Cost = 2500000,
                MaintenanceCostPerYear = 60000,
                InsuranceCostPerYear = 40000,
                LifeSpan = 10,
                MaxRange = 800,
                FuelCapacity = 200,
                Emission = "Euro 3",
                EmissionStandard = 3,
                SafetyRating = 3,
                Acceleration = 0.05,
                BrakingDistance = 80,
                TurningRadius = 10.0,
                WheelDrive = "RWD",
                Transmission = "Manual",
                GearRatio = 5.2,
                CurbWeight = 8000,
                MaxGVWR = 28000,
                AxleCount = 3,
                TireSize = "22.5 Inch",
                SuspensionType = "Leaf Spring",
                GroundClearance = 0.35,
                FuelTankType = "Steel",
                CoolingSystems = new() { "Water Cooled" },
                ParkingSpace = 4.0,
                BestForRoute = "Freight, long distance",
                OperatingCost = "Medium",
                EnvironmentalImpact = "High",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120 }
            },

            new()
            {
                Id = 6,
                Name = "Taxi/Sedan",
                Urdu = "ٹیکسی",
                Category = "Road",
                TransportMode = "Road",
                Color = "#ec4899",
                Capacity = 4,
                MaxSpeed = 180,
                AverageSpeed = 90,
                CostPerKm = 8.0M,
                Co2PerKm = 0.22,
                FuelType = "Petrol",
                ComfortRating = 4,
                Amenities = new() { "AC", "Comfortable Seating", "Music System", "Power Windows" },
                Cost = 1800000,
                MaintenanceCostPerYear = 18000,
                InsuranceCostPerYear = 10000,
                LifeSpan = 12,
                MaxRange = 550,
                FuelCapacity = 45,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 4,
                Acceleration = 0.095,
                BrakingDistance = 38,
                TurningRadius = 5.2,
                WheelDrive = "FWD",
                Transmission = "Automatic",
                GearRatio = 3.6,
                CurbWeight = 1250,
                MaxGVWR = 1700,
                AxleCount = 2,
                TireSize = "16 Inch",
                SuspensionType = "Independent Double Wishbone",
                GroundClearance = 0.16,
                FuelTankType = "Plastic",
                CoolingSystems = new() { "Water Cooled" },
                ParkingSpace = 2.1,
                BestForRoute = "City routes, on-demand transport",
                OperatingCost = "Medium",
                EnvironmentalImpact = "Moderate",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120 }
            },

            // ========== RAIL VEHICLES (4 Types) ==========

            new()
            {
                Id = 7,
                Name = "Local Train",
                Urdu = "مقامی ٹرین",
                Category = "Rail",
                TransportMode = "Rail",
                Color = "#f59e0b",
                Capacity = 800,
                MaxSpeed = 120,
                AverageSpeed = 70,
                CostPerKm = 8.0M,
                Co2PerKm = 0.15,
                FuelType = "Diesel/Electric",
                ComfortRating = 3,
                Amenities = new() { "Seating", "Fans", "Capacity for standing" },
                Cost = 50000000,
                MaintenanceCostPerYear = 500000,
                InsuranceCostPerYear = 200000,
                LifeSpan = 30,
                MaxRange = 500,
                FuelCapacity = 3000,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 5,
                Acceleration = 0.25,
                BrakingDistance = 200,
                TurningRadius = 300,
                WheelDrive = "All Wheels",
                Transmission = "Automatic",
                GearRatio = 2.5,
                CurbWeight = 400000,
                MaxGVWR = 600000,
                AxleCount = 8,
                TireSize = "Rail Wheels",
                SuspensionType = "Railway Suspension",
                GroundClearance = 0.50,
                FuelTankType = "Storage Tank",
                CoolingSystems = new() { "Ventilation" },
                ParkingSpace = 50,
                BestForRoute = "Mass transit, regular schedules",
                OperatingCost = "Low per passenger",
                EnvironmentalImpact = "Low",
                RouteCompatibility = new() { 1, 2, 3, 4, 5, 20, 23, 30, 35, 40, 50 }
            },

            new()
            {
                Id = 8,
                Name = "Express Train",
                Urdu = "ایکسپریس ٹرین",
                Category = "Rail",
                TransportMode = "Rail",
                Color = "#3b82f6",
                Capacity = 600,
                MaxSpeed = 160,
                AverageSpeed = 110,
                CostPerKm = 12.0M,
                Co2PerKm = 0.12,
                FuelType = "Diesel",
                ComfortRating = 4,
                Amenities = new() { "Comfortable Seating", "AC", "Dining", "Bedding" },
                Cost = 70000000,
                MaintenanceCostPerYear = 600000,
                InsuranceCostPerYear = 250000,
                LifeSpan = 30,
                MaxRange = 800,
                FuelCapacity = 5000,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 5,
                Acceleration = 0.30,
                BrakingDistance = 250,
                TurningRadius = 350,
                WheelDrive = "All Wheels",
                Transmission = "Automatic",
                GearRatio = 2.8,
                CurbWeight = 450000,
                MaxGVWR = 700000,
                AxleCount = 8,
                TireSize = "Rail Wheels",
                SuspensionType = "Railway Suspension",
                GroundClearance = 0.50,
                FuelTankType = "Storage Tank",
                CoolingSystems = new() { "AC System" },
                ParkingSpace = 55,
                BestForRoute = "Long distance, inter-city",
                OperatingCost = "Medium",
                EnvironmentalImpact = "Very Low",
                RouteCompatibility = new() { 1, 2, 3, 4, 5, 6, 20, 23, 30, 35, 40, 50, 60 }
            },

            new()
            {
                Id = 9,
                Name = "Freight Train",
                Urdu = "سامان لادنے والی ٹرین",
                Category = "Rail",
                TransportMode = "Rail",
                Color = "#22c55e",
                Capacity = 2000000,
                MaxSpeed = 100,
                AverageSpeed = 60,
                CostPerKm = 5.0M,
                Co2PerKm = 0.10,
                FuelType = "Diesel",
                ComfortRating = 1,
                Amenities = new() { },
                Cost = 80000000,
                MaintenanceCostPerYear = 700000,
                InsuranceCostPerYear = 300000,
                LifeSpan = 35,
                MaxRange = 1000,
                FuelCapacity = 8000,
                Emission = "Euro 3",
                EmissionStandard = 3,
                SafetyRating = 4,
                Acceleration = 0.20,
                BrakingDistance = 300,
                TurningRadius = 400,
                WheelDrive = "All Wheels",
                Transmission = "Manual",
                GearRatio = 3.5,
                CurbWeight = 600000,
                MaxGVWR = 2600000,
                AxleCount = 10,
                TireSize = "Rail Wheels",
                SuspensionType = "Railway Suspension",
                GroundClearance = 0.50,
                FuelTankType = "Large Storage Tank",
                CoolingSystems = new() { "Natural" },
                ParkingSpace = 100,
                BestForRoute = "Freight, cargo transport",
                OperatingCost = "Very Low per ton",
                EnvironmentalImpact = "Low",
                RouteCompatibility = new() { 1, 2, 3, 4, 5, 6, 20, 23, 30, 35, 40, 50 }
            },

            new()
            {
                Id = 10,
                Name = "Metro Train",
                Urdu = "میٹرو ٹرین",
                Category = "Rail",
                TransportMode = "Rail",
                Color = "#a855f7",
                Capacity = 1000,
                MaxSpeed = 80,
                AverageSpeed = 50,
                CostPerKm = 6.0M,
                Co2PerKm = 0.0,
                FuelType = "Electric",
                ComfortRating = 4,
                Amenities = new() { "Comfortable Seating", "Capacity for standing", "Digital Displays" },
                Cost = 100000000,
                MaintenanceCostPerYear = 800000,
                InsuranceCostPerYear = 350000,
                LifeSpan = 35,
                MaxRange = 300,
                FuelCapacity = 0,
                Emission = "Zero",
                EmissionStandard = 5,
                SafetyRating = 5,
                Acceleration = 0.40,
                BrakingDistance = 150,
                TurningRadius = 250,
                WheelDrive = "All Wheels",
                Transmission = "Automatic",
                GearRatio = 2.0,
                CurbWeight = 500000,
                MaxGVWR = 800000,
                AxleCount = 8,
                TireSize = "Rail Wheels",
                SuspensionType = "Advanced Railway Suspension",
                GroundClearance = 0.50,
                FuelTankType = "N/A",
                CoolingSystems = new() { "AC System" },
                ParkingSpace = 60,
                BestForRoute = "Urban rapid transit",
                OperatingCost = "Low",
                EnvironmentalImpact = "Zero",
                RouteCompatibility = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 }
            },

            // ========== AIR VEHICLES (3 Types) ==========

            new()
            {
                Id = 11,
                Name = "Regional Aircraft",
                Urdu = "علاقائی طیارہ",
                Category = "Air",
                TransportMode = "Air",
                Color = "#0ea5e9",
                Capacity = 100,
                MaxSpeed = 500,
                AverageSpeed = 450,
                CostPerKm = 50.0M,
                Co2PerKm = 0.25,
                FuelType = "Jet Fuel",
                ComfortRating = 3,
                Amenities = new() { "Seating", "Basic Service", "Beverage Service" },
                Cost = 200000000,
                MaintenanceCostPerYear = 2000000,
                InsuranceCostPerYear = 500000,
                LifeSpan = 30,
                MaxRange = 2000,
                FuelCapacity = 10000,
                Emission = "Euro 3",
                EmissionStandard = 3,
                SafetyRating = 5,
                Acceleration = 0.50,
                BrakingDistance = 2000,
                TurningRadius = 5000,
                WheelDrive = "4WD Landing Gear",
                Transmission = "Automatic",
                GearRatio = 1.0,
                CurbWeight = 35000,
                MaxGVWR = 60000,
                AxleCount = 3,
                TireSize = "26 Inch",
                SuspensionType = "Hydraulic",
                GroundClearance = 3.0,
                FuelTankType = "Integrated Wings",
                CoolingSystems = new() { "Air Circulation" },
                ParkingSpace = 50,
                BestForRoute = "Regional routes, 500-2000 km",
                OperatingCost = "High",
                EnvironmentalImpact = "Moderate-High",
                RouteCompatibility = new() { 1, 20, 25, 30, 35, 40, 50, 55, 60 }
            },

            new()
            {
                Id = 12,
                Name = "Commercial Jet",
                Urdu = "تجارتی جیٹ",
                Category = "Air",
                TransportMode = "Air",
                Color = "#84cc16",
                Capacity = 300,
                MaxSpeed = 900,
                AverageSpeed = 800,
                CostPerKm = 80.0M,
                Co2PerKm = 0.40,
                FuelType = "Jet Fuel",
                ComfortRating = 4,
                Amenities = new() { "Comfortable Seating", "Meals", "Entertainment", "Lavatories" },
                Cost = 400000000,
                MaintenanceCostPerYear = 4000000,
                InsuranceCostPerYear = 1000000,
                LifeSpan = 30,
                MaxRange = 6000,
                FuelCapacity = 25000,
                Emission = "Euro 4",
                EmissionStandard = 4,
                SafetyRating = 5,
                Acceleration = 0.55,
                BrakingDistance = 2500,
                TurningRadius = 6000,
                WheelDrive = "4WD Landing Gear",
                Transmission = "Automatic",
                GearRatio = 1.0,
                CurbWeight = 75000,
                MaxGVWR = 130000,
                AxleCount = 3,
                TireSize = "28 Inch",
                SuspensionType = "Hydraulic",
                GroundClearance = 3.0,
                FuelTankType = "Integrated Wings",
                CoolingSystems = new() { "Pressurized AC" },
                ParkingSpace = 80,
                BestForRoute = "Long haul, international",
                OperatingCost = "Very High",
                EnvironmentalImpact = "High",
                RouteCompatibility = new() { 1, 20, 25, 30, 35, 40, 45, 50, 55, 60 }
            },

            new()
            {
                Id = 13,
                Name = "Helicopter",
                Urdu = "ہیلی کاپٹر",
                Category = "Air",
                TransportMode = "Air",
                Color = "#f43f5e",
                Capacity = 20,
                MaxSpeed = 300,
                AverageSpeed = 200,
                CostPerKm = 100.0M,
                Co2PerKm = 0.35,
                FuelType = "Jet Fuel",
                ComfortRating = 3,
                Amenities = new() { "Seating", "Minimal Service" },
                Cost = 50000000,
                MaintenanceCostPerYear = 1000000,
                InsuranceCostPerYear = 200000,
                LifeSpan = 25,
                MaxRange = 1000,
                FuelCapacity = 2000,
                Emission = "Euro 2",
                EmissionStandard = 2,
                SafetyRating = 4,
                Acceleration = 0.45,
                BrakingDistance = 500,
                TurningRadius = 2000,
                WheelDrive = "Vertical Takeoff",
                Transmission = "Automatic",
                GearRatio = 1.5,
                CurbWeight = 5000,
                MaxGVWR = 10000,
                AxleCount = 4,
                TireSize = "14 Inch",
                SuspensionType = "Shock Absorber",
                GroundClearance = 1.0,
                FuelTankType = "Internal Tank",
                CoolingSystems = new() { "Air Circulation" },
                ParkingSpace = 30,
                BestForRoute = "Emergency, short distance, remote",
                OperatingCost = "Extremely High",
                EnvironmentalImpact = "High",
                RouteCompatibility = new() { 1, 20, 25, 30, 35, 40, 50, 51, 52, 55, 60 }
            }
        };
        }

        // ============================================================================
        // HELPER CLASS: VEHICLE TYPE MODEL
        // ============================================================================

        public class VehicleType
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Urdu { get; set; }
            public string Category { get; set; } // Road, Rail, Air
            public string TransportMode { get; set; }
            public string Color { get; set; }

            // Capacity & Performance
            public int Capacity { get; set; } // Persons or kg
            public int MaxSpeed { get; set; } // km/h
            public int AverageSpeed { get; set; } // km/h

            // Cost & Emissions
            public decimal CostPerKm { get; set; }
            public double Co2PerKm { get; set; } // kg
            public string FuelType { get; set; }

            // Comfort & Amenities
            public int ComfortRating { get; set; } // 1-5
            public List<string> Amenities { get; set; } = new();

            // Financial
            public long Cost { get; set; } // Purchase cost in PKR
            public long MaintenanceCostPerYear { get; set; }
            public long InsuranceCostPerYear { get; set; }
            public int LifeSpan { get; set; } // Years

            // Fuel & Range
            public int MaxRange { get; set; } // km
            public int FuelCapacity { get; set; } // Liters

            // Environmental
            public string Emission { get; set; } // Euro standard
            public int EmissionStandard { get; set; } // 1-5

            // Safety
            public int SafetyRating { get; set; } // 1-5

            // Technical Specifications
            public double Acceleration { get; set; } // 0-100 km/h in seconds (for vehicles)
            public int BrakingDistance { get; set; } // meters
            public double TurningRadius { get; set; } // meters
            public string WheelDrive { get; set; }
            public string Transmission { get; set; }
            public double GearRatio { get; set; }

            // Weight & Dimensions
            public int CurbWeight { get; set; } // kg
            public int MaxGVWR { get; set; } // Gross Vehicle Weight Rating (kg)
            public int AxleCount { get; set; }
            public string TireSize { get; set; }

            // Suspension & Comfort
            public string SuspensionType { get; set; }
            public double GroundClearance { get; set; } // meters

            // Fuel System
            public string FuelTankType { get; set; }

            // Cooling Systems
            public List<string> CoolingSystems { get; set; } = new();

            // Parking
            public double ParkingSpace { get; set; } // meters (length)

            // Characteristics
            public string BestForRoute { get; set; }
            public string OperatingCost { get; set; }
            public string EnvironmentalImpact { get; set; }

            // Route Compatibility
            public List<int> RouteCompatibility { get; set; } = new();
        }

        // ============================================================================
        // HELPER METHODS
        // ============================================================================

        public static VehicleType GetVehicleById(int id)
        {
            return GetAllVehicleTypes().Find(v => v.Id == id);
        }

        public static List<VehicleType> GetVehiclesByCategory(string category)
        {
            return GetAllVehicleTypes().FindAll(v => v.Category == category);
        }

        public static List<VehicleType> GetVehiclesByTransportMode(string mode)
        {
            return GetAllVehicleTypes().FindAll(v => v.TransportMode == mode);
        }

        public static List<VehicleType> GetVehiclesByCapacity(int minCapacity)
        {
            return GetAllVehicleTypes().FindAll(v => v.Capacity >= minCapacity);
        }

        public static List<VehicleType> GetVehiclesByEmissionStandard(int standard)
        {
            return GetAllVehicleTypes().FindAll(v => v.EmissionStandard >= standard);
        }

        public static List<VehicleType> GetEcoFriendlyVehicles()
        {
            return GetAllVehicleTypes().FindAll(v => v.Co2PerKm < 0.2 && v.EmissionStandard >= 4);
        }

        public static List<VehicleType> GetBudgetFriendlyVehicles()
        {
            return GetAllVehicleTypes().FindAll(v => v.CostPerKm < 10);
        }

        public static List<VehicleType> GetHighSpeedVehicles()
        {
            return GetAllVehicleTypes().FindAll(v => v.MaxSpeed >= 160);
        }

        public static decimal GetTotalCostPerKm(int vehicleId)
        {
            var vehicle = GetVehicleById(vehicleId);
            if (vehicle == null) return 0;

            decimal maintenancePerKm = (decimal)vehicle.MaintenanceCostPerYear / 15000; // Assuming 15k km/year
            decimal insurancePerKm = (decimal)vehicle.InsuranceCostPerYear / 15000;

            return vehicle.CostPerKm + maintenancePerKm + insurancePerKm;
        }

        public static double GetCO2EmissionsForDistance(int vehicleId, int distance)
        {
            var vehicle = GetVehicleById(vehicleId);
            if (vehicle == null) return 0;

            return vehicle.Co2PerKm * distance;
        }

        public static int GetEstimatedTimeHours(int vehicleId, int distanceKm)
        {
            var vehicle = GetVehicleById(vehicleId);
            if (vehicle == null) return 0;

            return (int)System.Math.Ceiling((double)distanceKm / vehicle.AverageSpeed);
        }
    }
}
