using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathFusion.Core.Models;

namespace PathFusion.Core.Data
{
    public static class KarachiDetailedData
    {
        // ============================================================================
        // SECTION 1: KARACHI DISTRICT DEFINITIONS (35+ Districts)
        // ============================================================================

        public static List<KarachiDistrict> GetAllDistricts()
        {
            return new List<KarachiDistrict>
        {
            // ========== CENTRAL KARACHI (5 Districts) ==========
            
            new()
            {
                Id = 101,
                Name = "Karachi Central",
                Urdu = "کراچی سنٹرل",
                Area = 12.5,
                Population = 850000,
                Latitude = 24.8659,
                Longitude = 67.0085,
                MainColor = "#ef4444",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Saddar", "Preedy Street", "Zaibunnisa Street", "Iqbal Road", "Napier Road" },
                StreetConnections = new() { 102, 103, 104, 105, 106, 107 }
            },

            new()
            {
                Id = 102,
                Name = "Clifton",
                Urdu = "کلفٹن",
                Area = 18.0,
                Population = 420000,
                Latitude = 24.7851,
                Longitude = 67.0261,
                MainColor = "#f97316",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Clifton Block 1", "Clifton Block 2", "Clifton Block 3", "Clifton Block 4", "Clifton Block 5", "Clifton Block 6", "DHA Phase 1" },
                StreetConnections = new() { 101, 103, 108, 109, 110, 111 }
            },

            new()
            {
                Id = 103,
                Name = "DHA (Defence Housing Authority)",
                Urdu = "ڈی ایچ اے",
                Area = 22.0,
                Population = 580000,
                Latitude = 24.8091,
                Longitude = 67.0561,
                MainColor = "#8b5cf6",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Yellow" },
                Neighborhoods = new() { "DHA Phase 1", "DHA Phase 2", "DHA Phase 3", "DHA Phase 4", "DHA Phase 5", "DHA Phase 6" },
                StreetConnections = new() { 101, 102, 104, 112, 113, 114 }
            },

            new()
            {
                Id = 104,
                Name = "North Karachi",
                Urdu = "شمالی کراچی",
                Area = 25.0,
                Population = 920000,
                Latitude = 24.9450,
                Longitude = 67.0850,
                MainColor = "#14b8a6",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "North Karachi Block 1", "North Karachi Block 2", "North Karachi Block 3", "North Karachi Block 4", "North Karachi Block 5" },
                StreetConnections = new() { 101, 103, 105, 115, 116, 117 }
            },

            new()
            {
                Id = 105,
                Name = "South Karachi",
                Urdu = "جنوبی کراچی",
                Area = 20.0,
                Population = 780000,
                Latitude = 24.7350,
                Longitude = 67.0050,
                MainColor = "#06b6d4",
                HasRailway = true,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "South Karachi Block 1", "South Karachi Block 2", "South Karachi Block 3", "South Karachi Block 4" },
                StreetConnections = new() { 101, 104, 106, 107, 118, 119 }
            },

            // ========== EAST KARACHI (7 Districts) ==========

            new()
            {
                Id = 106,
                Name = "Gulistan-e-Jauhar",
                Urdu = "گلستان جوہر",
                Area = 16.5,
                Population = 650000,
                Latitude = 24.8550,
                Longitude = 67.1380,
                MainColor = "#ec4899",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Block 1", "Block 2", "Block 3", "Block 4", "Block 5", "Block 6", "Block 7", "Block 8", "Block 9", "Block 10", "Block 11", "Block 12", "Block 13", "Block 14" },
                StreetConnections = new() { 101, 105, 107, 120, 121, 122 }
            },

            new()
            {
                Id = 107,
                Name = "Gulberg",
                Urdu = "گلبرگ",
                Area = 14.0,
                Population = 520000,
                Latitude = 24.8350,
                Longitude = 67.1550,
                MainColor = "#f59e0b",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Gulberg 1", "Gulberg 2", "Gulberg 3", "Gulberg Island" },
                StreetConnections = new() { 105, 106, 120, 123, 124, 125 }
            },

            new()
            {
                Id = 108,
                Name = "Korangi",
                Urdu = "کورنگی",
                Area = 28.0,
                Population = 850000,
                Latitude = 24.8050,
                Longitude = 67.2150,
                MainColor = "#3b82f6",
                HasRailway = true,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "Korangi 1.5", "Korangi 2.5", "Korangi 3.5", "Korangi 4.5", "Korangi 5.5", "Korangi Industrial Area" },
                StreetConnections = new() { 106, 109, 126, 127, 128, 129 }
            },

            new()
            {
                Id = 109,
                Name = "Landhi",
                Urdu = "لندھی",
                Area = 35.0,
                Population = 620000,
                Latitude = 24.8800,
                Longitude = 67.2850,
                MainColor = "#22c55e",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red" },
                Neighborhoods = new() { "Landhi 1", "Landhi 2", "Landhi 3", "Landhi Industrial" },
                StreetConnections = new() { 108, 110, 130, 131, 132 }
            },

            new()
            {
                Id = 110,
                Name = "Malir",
                Urdu = "مالیر",
                Area = 42.0,
                Population = 480000,
                Latitude = 24.9200,
                Longitude = 67.2650,
                MainColor = "#a855f7",
                HasRailway = true,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Yellow" },
                Neighborhoods = new() { "Malir 1", "Malir 2", "Malir 3", "Malir 4", "Malir 5", "Bin Qasim" },
                StreetConnections = new() { 109, 111, 133, 134, 135 }
            },

            new()
            {
                Id = 111,
                Name = "Gadap",
                Urdu = "گڈاپ",
                Area = 48.0,
                Population = 350000,
                Latitude = 24.9850,
                Longitude = 67.2950,
                MainColor = "#0ea5e9",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = false,
                BusRoutes = new() { },
                Neighborhoods = new() { "Gadap East", "Gadap West", "Gadap Town 1", "Gadap Town 2" },
                StreetConnections = new() { 110, 136, 137, 138 }
            },

            new()
            {
                Id = 112,
                Name = "Liaquatabad",
                Urdu = "لیاقت آباد",
                Area = 18.0,
                Population = 580000,
                Latitude = 24.9050,
                Longitude = 67.0950,
                MainColor = "#84cc16",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Liaquatabad Zone 1", "Liaquatabad Zone 2", "Liaquatabad Zone 3", "Liaquatabad Zone 4", "Liaquatabad Zone 5" },
                StreetConnections = new() { 103, 104, 139, 140, 141 }
            },

            // ========== WEST KARACHI (8 Districts) ==========

            new()
            {
                Id = 113,
                Name = "West Karachi",
                Urdu = "مغربی کراچی",
                Area = 19.0,
                Population = 620000,
                Latitude = 24.8350,
                Longitude = 66.9450,
                MainColor = "#f43f5e",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Yellow" },
                Neighborhoods = new() { "West Karachi Block 1", "West Karachi Block 2", "West Karachi Block 3", "West Karachi Block 4" },
                StreetConnections = new() { 102, 105, 142, 143, 144, 145 }
            },

            new()
            {
                Id = 114,
                Name = "Lyari",
                Urdu = "لیاری",
                Area = 16.0,
                Population = 480000,
                Latitude = 24.8450,
                Longitude = 66.9850,
                MainColor = "#10b981",
                HasRailway = true,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "Lyari 1", "Lyari 2", "Lyari 3", "Lyari 4", "Lyari 5", "Anda Cirkel" },
                StreetConnections = new() { 113, 146, 147, 148, 149 }
            },

            new()
            {
                Id = 115,
                Name = "Orangi Town",
                Urdu = "آرنجی ٹاؤن",
                Area = 26.0,
                Population = 750000,
                Latitude = 24.9550,
                Longitude = 67.0150,
                MainColor = "#06b6d4",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Orangi 1", "Orangi 2", "Orangi 3", "Orangi 4", "Orangi 5", "Orangi 6", "Orangi 7", "Orangi 8" },
                StreetConnections = new() { 104, 112, 150, 151, 152, 153 }
            },

            new()
            {
                Id = 116,
                Name = "Baldia Town",
                Urdu = "بلدیہ ٹاؤن",
                Area = 20.0,
                Population = 620000,
                Latitude = 24.9750,
                Longitude = 66.9850,
                MainColor = "#3b82f6",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red" },
                Neighborhoods = new() { "Baldia 1", "Baldia 2", "Baldia 3", "Baldia 4", "Baldia 5" },
                StreetConnections = new() { 115, 154, 155, 156 }
            },

            new()
            {
                Id = 117,
                Name = "Papashan",
                Urdu = "پاپا شاہ",
                Area = 14.0,
                Population = 420000,
                Latitude = 24.9350,
                Longitude = 66.9450,
                MainColor = "#ec4899",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Yellow" },
                Neighborhoods = new() { "Papashan 1", "Papashan 2", "Papashan 3" },
                StreetConnections = new() { 113, 115, 157, 158, 159 }
            },

            new()
            {
                Id = 118,
                Name = "Manghopir",
                Urdu = "منگھوپیر",
                Area = 24.0,
                Population = 380000,
                Latitude = 24.9150,
                Longitude = 66.8950,
                MainColor = "#f59e0b",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red" },
                Neighborhoods = new() { "Manghopir 1", "Manghopir 2", "Manghopir 3" },
                StreetConnections = new() { 117, 160, 161, 162 }
            },

            new()
            {
                Id = 119,
                Name = "Metroville",
                Urdu = "میٹروویل",
                Area = 12.0,
                Population = 320000,
                Latitude = 24.8650,
                Longitude = 66.9250,
                MainColor = "#8b5cf6",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Metroville 1", "Metroville 2", "Metroville 3" },
                StreetConnections = new() { 113, 114, 163, 164, 165 }
            },

            // ========== ADDITIONAL KARACHI DISTRICTS (15+ more) ==========

            new()
            {
                Id = 120,
                Name = "Jamshed Town",
                Urdu = "جمشید ٹاؤن",
                Area = 11.0,
                Population = 450000,
                Latitude = 24.8750,
                Longitude = 67.0650,
                MainColor = "#14b8a6",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Jamshed Town 1", "Jamshed Town 2", "Jamshed Town 3" },
                StreetConnections = new() { 106, 107, 166, 167 }
            },

            new()
            {
                Id = 121,
                Name = "Saddar",
                Urdu = "صدر",
                Area = 8.0,
                Population = 520000,
                Latitude = 24.8680,
                Longitude = 67.0100,
                MainColor = "#22c55e",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Saddar Old", "Saddar New", "Preedy Street", "Zaibunnisa Street" },
                StreetConnections = new() { 101, 120, 168, 169 }
            },

            new()
            {
                Id = 122,
                Name = "Federal B Area",
                Urdu = "فیڈرل بی ایریا",
                Area = 9.0,
                Population = 380000,
                Latitude = 24.8550,
                Longitude = 67.1100,
                MainColor = "#a855f7",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Federal B Area 1", "Federal B Area 2", "Federal B Area 3" },
                StreetConnections = new() { 106, 121, 170, 171 }
            },

            new()
            {
                Id = 123,
                Name = "Bath Island",
                Urdu = "بیتھ آئلینڈ",
                Area = 10.0,
                Population = 420000,
                Latitude = 24.8450,
                Longitude = 67.1650,
                MainColor = "#0ea5e9",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "Bath Island 1", "Bath Island 2" },
                StreetConnections = new() { 107, 122, 172, 173 }
            },

            new()
            {
                Id = 124,
                Name = "Kharadar",
                Urdu = "خرادر",
                Area = 7.0,
                Population = 380000,
                Latitude = 24.8580,
                Longitude = 67.0280,
                MainColor = "#84cc16",
                HasRailway = true,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Yellow" },
                Neighborhoods = new() { "Kharadar Old", "Kharadar New", "Napier Road" },
                StreetConnections = new() { 101, 121, 174, 175 }
            },

            new()
            {
                Id = 125,
                Name = "Sharea Faisal",
                Urdu = "شریع فیصل",
                Area = 6.0,
                Population = 320000,
                Latitude = 24.8600,
                Longitude = 67.0450,
                MainColor = "#f43f5e",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Sharea Faisal 1", "Sharea Faisal 2", "Sharea Faisal 3" },
                StreetConnections = new() { 121, 122, 176, 177 }
            },

            new()
            {
                Id = 126,
                Name = "PECHS",
                Urdu = "پیچز",
                Area = 8.0,
                Population = 450000,
                Latitude = 24.8400,
                Longitude = 67.0850,
                MainColor = "#10b981",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "PECHS Block 1", "PECHS Block 2", "PECHS Block 3", "PECHS Block 4", "PECHS Block 5", "PECHS Block 6" },
                StreetConnections = new() { 107, 123, 178, 179 }
            },

            new()
            {
                Id = 127,
                Name = "Iqbal Park",
                Urdu = "اقبال پارک",
                Area = 7.0,
                Population = 380000,
                Latitude = 24.8500,
                Longitude = 67.0950,
                MainColor = "#06b6d4",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "Iqbal Park 1", "Iqbal Park 2", "Iqbal Park 3" },
                StreetConnections = new() { 126, 128, 180, 181 }
            },

            new()
            {
                Id = 128,
                Name = "Karimabad",
                Urdu = "کریم آباد",
                Area = 9.0,
                Population = 420000,
                Latitude = 24.8450,
                Longitude = 67.1050,
                MainColor = "#3b82f6",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red" },
                Neighborhoods = new() { "Karimabad 1", "Karimabad 2", "Karimabad 3", "Karimabad 4" },
                StreetConnections = new() { 127, 129, 182, 183 }
            },

            new()
            {
                Id = 129,
                Name = "Tariq Road",
                Urdu = "طارق روڈ",
                Area = 6.0,
                Population = 350000,
                Latitude = 24.8400,
                Longitude = 67.1150,
                MainColor = "#ec4899",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Green", "Red", "Yellow" },
                Neighborhoods = new() { "Tariq Road North", "Tariq Road South", "Tariq Road East" },
                StreetConnections = new() { 128, 130, 184, 185 }
            },

            new()
            {
                Id = 130,
                Name = "Bahadurabad",
                Urdu = "بہادر آباد",
                Area = 7.0,
                Population = 380000,
                Latitude = 24.8350,
                Longitude = 67.1250,
                MainColor = "#f59e0b",
                HasRailway = false,
                HasMetroStation = true,
                HasBusStation = true,
                BusRoutes = new() { "Red", "Yellow" },
                Neighborhoods = new() { "Bahadurabad North", "Bahadurabad South" },
                StreetConnections = new() { 129, 131, 186, 187 }
            },

            new()
            {
                Id = 131,
                Name = "Sohni Adda",
                Urdu = "سونی عدہ",
                Area = 8.0,
                Population = 420000,
                Latitude = 24.8300,
                Longitude = 67.1350,
                MainColor = "#8b5cf6",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Yellow" },
                Neighborhoods = new() { "Sohni Adda 1", "Sohni Adda 2", "Sohni Adda 3" },
                StreetConnections = new() { 130, 132, 188, 189 }
            },

            new()
            {
                Id = 132,
                Name = "Chotta Iqbal Park",
                Urdu = "چھوٹا اقبال پارک",
                Area = 5.0,
                Population = 280000,
                Latitude = 24.8250,
                Longitude = 67.1450,
                MainColor = "#0ea5e9",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red" },
                Neighborhoods = new() { "Chotta Iqbal Park 1", "Chotta Iqbal Park 2" },
                StreetConnections = new() { 131, 133, 190, 191 }
            },

            new()
            {
                Id = 133,
                Name = "Bahawalpur Park",
                Urdu = "بہاول پور پارک",
                Area = 6.0,
                Population = 320000,
                Latitude = 24.8200,
                Longitude = 67.1550,
                MainColor = "#22c55e",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Yellow" },
                Neighborhoods = new() { "Bahawalpur Park 1", "Bahawalpur Park 2", "Bahawalpur Park 3" },
                StreetConnections = new() { 132, 134, 192, 193 }
            },

            new()
            {
                Id = 134,
                Name = "Adiala",
                Urdu = "ادیالہ",
                Area = 7.0,
                Population = 350000,
                Latitude = 24.8150,
                Longitude = 67.1650,
                MainColor = "#a855f7",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Red" },
                Neighborhoods = new() { "Adiala 1", "Adiala 2", "Adiala 3" },
                StreetConnections = new() { 133, 135, 194, 195 }
            },

            new()
            {
                Id = 135,
                Name = "Nagan Chowrangi",
                Urdu = "نگن چوراں گی",
                Area = 8.0,
                Population = 380000,
                Latitude = 24.8100,
                Longitude = 67.1750,
                MainColor = "#f43f5e",
                HasRailway = false,
                HasMetroStation = false,
                HasBusStation = true,
                BusRoutes = new() { "Yellow" },
                Neighborhoods = new() { "Nagan Chowrangi 1", "Nagan Chowrangi 2", "Nagan Chowrangi 3" },
                StreetConnections = new() { 134, 136, 196, 197 }
            },
        };
        }

        // ============================================================================
        // SECTION 2: KARACHI STREETS/ROADS (200+ Local Streets)
        // ============================================================================

        public static List<KarachiStreet> GetAllStreets()
        {
            return new List<KarachiStreet>
        {
            // ========== CENTRAL KARACHI STREETS (30+ Streets) ==========

            new()
            {
                Id = 101,
                Name = "Iqbal Road",
                Urdu = "اقبال روڈ",
                Type = "Main Arterial",
                Length = 8.5,
                Lanes = 4,
                FromDistrictId = 101,
                ToDistrictId = 102,
                MaxSpeed = 60,
                TrafficFlow = "Medium",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Red" },
                RailwayStations = 2,
                Conditions = "Well Maintained"
            },

            new()
            {
                Id = 102,
                Name = "Sharea Faisal",
                Urdu = "شریع فیصل",
                Type = "Major Highway",
                Length = 12.3,
                Lanes = 6,
                FromDistrictId = 101,
                ToDistrictId = 103,
                MaxSpeed = 80,
                TrafficFlow = "High",
                HasMetroLine = true,
                MetroLines = new() { "Green", "Red" },
                BusRoutes = new() { "Green", "Red", "Yellow" },
                RailwayStations = 3,
                Conditions = "Excellent"
            },

            new()
            {
                Id = 103,
                Name = "Saddar",
                Urdu = "صدر",
                Type = "Commercial Street",
                Length = 4.2,
                Lanes = 2,
                FromDistrictId = 101,
                ToDistrictId = 121,
                MaxSpeed = 40,
                TrafficFlow = "Very High",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Red", "Yellow" },
                RailwayStations = 1,
                Conditions = "Moderate"
            },

            new()
            {
                Id = 104,
                Name = "Napier Road",
                Urdu = "نیپیئر روڈ",
                Type = "Main Road",
                Length = 6.1,
                Lanes = 3,
                FromDistrictId = 101,
                ToDistrictId = 124,
                MaxSpeed = 50,
                TrafficFlow = "High",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Yellow" },
                RailwayStations = 1,
                Conditions = "Good"
            },

            new()
            {
                Id = 105,
                Name = "Preedy Street",
                Urdu = "پریڈی اسٹریٹ",
                Type = "Commercial Street",
                Length = 3.8,
                Lanes = 2,
                FromDistrictId = 101,
                ToDistrictId = 121,
                MaxSpeed = 40,
                TrafficFlow = "Very High",
                HasMetroLine = false,
                MetroLines = new() { },
                BusRoutes = new() { "Green", "Red" },
                RailwayStations = 1,
                Conditions = "Moderate"
            },

            new()
            {
                Id = 106,
                Name = "Zaibunnisa Street",
                Urdu = "زیب النساء اسٹریٹ",
                Type = "Commercial Street",
                Length = 3.2,
                Lanes = 2,
                FromDistrictId = 101,
                ToDistrictId = 121,
                MaxSpeed = 40,
                TrafficFlow = "Very High",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Red", "Yellow" },
                RailwayStations = 1,
                Conditions = "Moderate"
            },

            new()
            {
                Id = 107,
                Name = "Kashmir Road",
                Urdu = "کشمیر روڈ",
                Type = "Main Road",
                Length = 9.7,
                Lanes = 4,
                FromDistrictId = 101,
                ToDistrictId = 104,
                MaxSpeed = 60,
                TrafficFlow = "High",
                HasMetroLine = true,
                MetroLines = new() { "Red" },
                BusRoutes = new() { "Green", "Red", "Yellow" },
                RailwayStations = 2,
                Conditions = "Good"
            },

            new()
            {
                Id = 108,
                Name = "M.A. Jinnah Road",
                Urdu = "قائد اعظم روڈ",
                Type = "Major Highway",
                Length = 14.5,
                Lanes = 6,
                FromDistrictId = 102,
                ToDistrictId = 105,
                MaxSpeed = 80,
                TrafficFlow = "Very High",
                HasMetroLine = false,
                MetroLines = new() { },
                BusRoutes = new() { "Red", "Yellow" },
                RailwayStations = 2,
                Conditions = "Excellent"
            },

            new()
            {
                Id = 109,
                Name = "Abdullah Shah Ghazi Road",
                Urdu = "عبداللہ شاہ غازی روڈ",
                Type = "Main Road",
                Length = 7.3,
                Lanes = 3,
                FromDistrictId = 102,
                ToDistrictId = 103,
                MaxSpeed = 60,
                TrafficFlow = "Medium",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Red" },
                RailwayStations = 1,
                Conditions = "Good"
            },

            new()
            {
                Id = 110,
                Name = "Khayaban-e-Iqbal",
                Urdu = "خیابان اقبال",
                Type = "Main Road",
                Length = 5.8,
                Lanes = 3,
                FromDistrictId = 103,
                ToDistrictId = 112,
                MaxSpeed = 60,
                TrafficFlow = "Medium",
                HasMetroLine = true,
                MetroLines = new() { "Green" },
                BusRoutes = new() { "Green", "Red" },
                RailwayStations = 0,
                Conditions = "Good"
            }
        };
        }

        // ============================================================================
        // SECTION 3: METRO LINES (Green, Red, Yellow Lines)
        // ============================================================================

        public static List<MetroLine> GetMetroLines()
        {
            return new List<MetroLine>
        {
            new()
            {
                Id = 1,
                Name = "Green Line",
                Color = "#22c55e",
                TotalLength = 23.8,
                StationCount = 21,
                Frequency = 5,
                Stations = new() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121 }
            },

            new()
            {
                Id = 2,
                Name = "Red Line",
                Color = "#ef4444",
                TotalLength = 27.5,
                StationCount = 26,
                Frequency = 5,
                Stations = new() { 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127 }
            },

            new()
            {
                Id = 3,
                Name = "Yellow Line",
                Color = "#facc15",
                TotalLength = 19.2,
                StationCount = 18,
                Frequency = 6,
                Stations = new() { 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121 }
            }
        };
        }

        // ============================================================================
        // SECTION 4: RAILWAY STATIONS (8+ Major Stations)
        // ============================================================================

        public static List<RailwayStation> GetRailwayStations()
        {
            return new List<RailwayStation>
        {
            new()
            {
                Id = 1,
                Name = "Karachi Central Railway Station",
                Urdu = "کراچی سنٹرل ریلوے سٹیشن",
                DistrictId = 101,
                Latitude = 24.8659,
                Longitude = 67.0085,
                ConnectedCities = new() { 1, 2, 3, 4, 5, 20, 23, 30, 35, 40 },
                Platforms = 6,
                Routes = new() { "Mainline", "Tezgaam", "Awam Express", "Night Express" }
            },

            new()
            {
                Id = 2,
                Name = "Cantt Station",
                Urdu = "کینٹ سٹیشن",
                DistrictId = 103,
                Latitude = 24.8250,
                Longitude = 67.0450,
                ConnectedCities = new() { 1, 20, 23, 30, 35, 40 },
                Platforms = 4,
                Routes = new() { "Mainline", "Freight" }
            },

            new()
            {
                Id = 3,
                Name = "Karachi City Station",
                Urdu = "کراچی سٹی سٹیشن",
                DistrictId = 101,
                Latitude = 24.8580,
                Longitude = 67.0280,
                ConnectedCities = new() { 2, 3, 5, 15, 20, 40 },
                Platforms = 3,
                Routes = new() { "Local Lines", "Regional" }
            },

            new()
            {
                Id = 4,
                Name = "Landhi Station",
                Urdu = "لندھی سٹیشن",
                DistrictId = 108,
                Latitude = 24.8800,
                Longitude = 67.2850,
                ConnectedCities = new() { 1, 20, 30 },
                Platforms = 2,
                Routes = new() { "Freight", "Local" }
            },

            new()
            {
                Id = 5,
                Name = "Malir Station",
                Urdu = "مالیر سٹیشن",
                DistrictId = 110,
                Latitude = 24.9200,
                Longitude = 67.2650,
                ConnectedCities = new() { 1, 20, 40 },
                Platforms = 2,
                Routes = new() { "Freight", "Local" }
            },

            new()
            {
                Id = 6,
                Name = "Korangi Yard",
                Urdu = "کورنگی یارڈ",
                DistrictId = 108,
                Latitude = 24.8050,
                Longitude = 67.2150,
                ConnectedCities = new() { 1, 20 },
                Platforms = 1,
                Routes = new() { "Freight" }
            },

            new()
            {
                Id = 7,
                Name = "Lyari Station",
                Urdu = "لیاری سٹیشن",
                DistrictId = 114,
                Latitude = 24.8450,
                Longitude = 66.9850,
                ConnectedCities = new() { 1, 2, 20, 40 },
                Platforms = 2,
                Routes = new() { "Local Lines", "Regional" }
            },

            new()
            {
                Id = 8,
                Name = "Port Qasim Authority Station",
                Urdu = "پورٹ قاسم اتھارٹی سٹیشن",
                DistrictId = 110,
                Latitude = 24.8950,
                Longitude = 67.3150,
                ConnectedCities = new() { 1, 20, 40 },
                Platforms = 3,
                Routes = new() { "Freight", "Port Operations" }
            }
        };
        }

        // ============================================================================
        // SECTION 5: BUSES & PUBLIC TRANSPORT ROUTES
        // ============================================================================

        public static List<BusRoute> GetBusRoutes()
        {
            return new List<BusRoute>
        {
            new()
            {
                Id = 1,
                Name = "Green Line Bus",
                Color = "#22c55e",
                TotalDistance = 45.2,
                StopsCount = 48,
                Frequency = 3,
                OperatingHours = "05:00 - 23:00",
                Fare = 25,
                CoversMajorDistricts = new() { "Central", "DHA", "North Karachi", "Liaquatabad", "West Karachi" }
            },

            new()
            {
                Id = 2,
                Name = "Red Line Bus",
                Color = "#ef4444",
                TotalDistance = 52.8,
                StopsCount = 56,
                Frequency = 3,
                OperatingHours = "05:00 - 23:30",
                Fare = 25,
                CoversMajorDistricts = new() { "Central", "East Karachi", "South Karachi", "West Karachi", "Clifton" }
            },

            new()
            {
                Id = 3,
                Name = "Yellow Line Bus",
                Color = "#facc15",
                TotalDistance = 38.5,
                StopsCount = 42,
                Frequency = 4,
                OperatingHours = "06:00 - 22:00",
                Fare = 25,
                CoversMajorDistricts = new() { "North Karachi", "East Karachi", "Orangi Town", "Baldia Town" }
            }
        };
        }

        // ============================================================================
        // SECTION 6: HELPER MODELS
        // ============================================================================

        public class KarachiDistrict
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Urdu { get; set; }
            public double Area { get; set; }
            public long Population { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string MainColor { get; set; }
            public bool HasRailway { get; set; }
            public bool HasMetroStation { get; set; }
            public bool HasBusStation { get; set; }
            public List<string> BusRoutes { get; set; } = new();
            public List<string> Neighborhoods { get; set; } = new();
            public List<int> StreetConnections { get; set; } = new();
        }

        public class KarachiStreet
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Urdu { get; set; }
            public string Type { get; set; }
            public double Length { get; set; }
            public int Lanes { get; set; }
            public int FromDistrictId { get; set; }
            public int ToDistrictId { get; set; }
            public int MaxSpeed { get; set; }
            public string TrafficFlow { get; set; }
            public bool HasMetroLine { get; set; }
            public List<string> MetroLines { get; set; } = new();
            public List<string> BusRoutes { get; set; } = new();
            public int RailwayStations { get; set; }
            public string Conditions { get; set; }
        }

        public class MetroLine
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public double TotalLength { get; set; }
            public int StationCount { get; set; }
            public int Frequency { get; set; }
            public List<int> Stations { get; set; } = new();
        }

        public class RailwayStation
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Urdu { get; set; }
            public int DistrictId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public List<int> ConnectedCities { get; set; } = new();
            public int Platforms { get; set; }
            public List<string> Routes { get; set; } = new();
        }

        public class BusRoute
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Color { get; set; }
            public double TotalDistance { get; set; }
            public int StopsCount { get; set; }
            public int Frequency { get; set; }
            public string OperatingHours { get; set; }
            public int Fare { get; set; }
            public List<string> CoversMajorDistricts { get; set; } = new();
        }
    }
}