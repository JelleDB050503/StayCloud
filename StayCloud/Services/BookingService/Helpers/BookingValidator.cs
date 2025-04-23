namespace BookingService.Helpers
{
    public static class BookingValidator
    {
        public static bool IsValidAccommodation(string accommodationType)
        {
            var allowed = new[] { "caravan-1", "caravan-2", "chalet" };
            return allowed.Contains(accommodationType.ToLower());
        }

        public static bool IsValidStayType(string stayType)
        {
            var allowed = new[] { "weekend", "midweek", "week" };
            return allowed.Contains(stayType.ToLower());
        }

        public static bool IsValidSeason(string season)
        {
            var allowed = new[] { "laagseizoen", "pasen", "tussenseizoen", "hoogseizoen", "herfst", "kerst", "nieuwjaar" };
            return allowed.Contains(season.ToLower());
        }
    }
}