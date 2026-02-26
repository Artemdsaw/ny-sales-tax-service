using System;
using System.Collections.Generic;
using System.Text;

namespace SurveySystem.Application
{
    public static class GeoValidator
    {
        private const double MinLat = 40.477399;
        private const double MaxLat = 45.015865;
        private const double MinLon = -79.762152;
        private const double MaxLon = -71.751708;

        public static bool IsWithinNewYork(double latitude, double longitude)
        {
            return latitude >= MinLat && latitude <= MaxLat &&
                   longitude >= MinLon && longitude <= MaxLon;
        }
    }
}
