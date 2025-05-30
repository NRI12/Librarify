using libraryproject.Models;
using System.Collections.Generic;

namespace libraryproject.ViewModels
{
    public class HomeViewModel
    {
        // Sách mới nhất
        public List<Models.TaiLieu> LatestBooks { get; set; }

        // Bộ sưu tập
        public List<Models.BoSuuTap> Collections { get; set; }

        // Thống kê
        public StatisticsViewModel Statistics { get; set; }

        // Thông báo/Tin tức
        public List<NewsItemViewModel> NewsItems { get; set; }
    }

    public class StatisticsViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCollections { get; set; }
        public int ActiveLoans { get; set; }
    }

    public class NewsItemViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
    }
}