namespace Room2Room.Models.DTOs
{
    public class AdminStatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalListings { get; set; }
        public int UniversitiesWithListings { get; set; }
        public double AverageListingsPerUser { get; set; }

        public IEnumerable<UniversityListingStat> ListingsByUniversity { get; set; } = new List<UniversityListingStat>();
        public IEnumerable<CategoryListingStat> ListingsByCategory { get; set; } = new List<CategoryListingStat>();
        public IEnumerable<TopUserListingStat> TopUsersByListings { get; set; } = new List<TopUserListingStat>();
    }

    public class UniversityListingStat
    {
        public string UniversityName { get; set; } = string.Empty;
        public int ListingCount { get; set; }
    }

    public class CategoryListingStat
    {
        public string CategoryName { get; set; } = string.Empty;
        public int ListingCount { get; set; }
    }

    public class TopUserListingStat
    {
        public string UserDisplayName { get; set; } = string.Empty;
        public int ListingCount { get; set; }
    }
}