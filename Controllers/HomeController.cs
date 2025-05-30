using System.Diagnostics;
using libraryproject.Data;
using libraryproject.Models;
using libraryproject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace libraryproject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QLTVContext _context;

        public HomeController(ILogger<HomeController> logger, QLTVContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Lấy 6 sách mới nhất
                LatestBooks = await _context.TaiLieus
                    .Include(t => t.BoSuuTap)
                    .OrderByDescending(t => t.NgayNhap)
                    .Take(6)
                    .ToListAsync(),

                // Lấy 6 bộ sưu tập
                Collections = await _context.BoSuuTaps
                    .Include(b => b.TaiLieus)
                    .OrderBy(b => b.TenBoSuuTap)
                    .Take(6)
                    .ToListAsync(),

                // Thống kê
                Statistics = new StatisticsViewModel
                {
                    TotalBooks = await _context.TaiLieus.CountAsync(),
                    TotalUsers = await _context.NguoiDungs.CountAsync(),
                    TotalCollections = await _context.BoSuuTaps.CountAsync(),
                    ActiveLoans = await _context.PhieuMuons
                        .Where(p => p.TrangThai == "Đang mượn")
                        .CountAsync()
                },

                // Thông báo/Tin tức mẫu
                NewsItems = new List<NewsItemViewModel>
                {
                    new NewsItemViewModel
                    {
                        Title = "Thông báo nghỉ lễ",
                        Content = "Thư viện sẽ đóng cửa vào ngày 30/4 và 1/5 nhân dịp lễ.",
                        Date = "20/04/2025"
                    },
                    new NewsItemViewModel
                    {
                        Title = "Triển lãm sách mới",
                        Content = "Triển lãm sách mới sẽ được tổ chức vào ngày 15/05/2025.",
                        Date = "15/04/2025"
                    },
                    new NewsItemViewModel
                    {
                        Title = "Hướng dẫn sử dụng thư viện số",
                        Content = "Buổi hướng dẫn sử dụng thư viện số sẽ được tổ chức vào ngày 10/05/2025.",
                        Date = "10/04/2025"
                    }
                }
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}