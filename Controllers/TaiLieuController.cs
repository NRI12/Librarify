using libraryproject.Data;
using libraryproject.ViewModels.TaiLieu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace libraryproject.Controllers
{
    public class TaiLieuController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10; // Số lượng tài liệu mỗi trang

        public TaiLieuController(QLTVContext context)
        {
            _context = context;
        }

        // GET: TaiLieu
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            // Tìm kiếm đơn giản
            ViewData["CurrentFilter"] = searchString;
            var currentPage = page ?? 1;

            var query = _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t =>
                    t.NhanDe.Contains(searchString) ||
                    t.TacGia.Contains(searchString) ||
                    t.NhaXuatBan.Contains(searchString) ||
                    t.ThuatNguChuDe.Contains(searchString)
                );
            }

            var count = await query.CountAsync();
            var items = await query.Skip((currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;

            return View(items);
        }

        // GET: TaiLieu/Search
        public async Task<IActionResult> Search(TaiLieuSearchViewModel model)
        {
            // Tìm kiếm nâng cao
            var query = _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .AsQueryable();

            // Áp dụng các điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(model.NhanDe))
            {
                query = query.Where(t => t.NhanDe.Contains(model.NhanDe));
            }

            if (!string.IsNullOrEmpty(model.TacGia))
            {
                query = query.Where(t => t.TacGia.Contains(model.TacGia));
            }

            if (!string.IsNullOrEmpty(model.NhaXuatBan))
            {
                query = query.Where(t => t.NhaXuatBan.Contains(model.NhaXuatBan));
            }

            if (model.NamXuatBan > 0)
            {
                query = query.Where(t => t.NamXuatBan == model.NamXuatBan);
            }

            if (model.BoSuuTapID > 0)
            {
                query = query.Where(t => t.BoSuuTapID == model.BoSuuTapID);
            }

            if (!string.IsNullOrEmpty(model.ThuatNguChuDe))
            {
                query = query.Where(t => t.ThuatNguChuDe.Contains(model.ThuatNguChuDe));
            }

            // Đếm tổng số kết quả
            var count = await query.CountAsync();

            // Phân trang
            var currentPage = model.Page ?? 1;
            var items = await query
                .Skip((currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalItems = count;

            // Lấy danh sách bộ sưu tập cho dropdown
            ViewBag.BoSuuTaps = await _context.BoSuuTaps
                .Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.TenBoSuuTap
                })
                .ToListAsync();

            model.Results = items;
            return View(model);
        }

        // GET: TaiLieu/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var taiLieu = await _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            // Lấy 4 sách ngẫu nhiên khác để hiển thị ở phần "Có thể bạn cũng thích"
            var randomBooks = await _context.TaiLieus
                .Where(t => t.ID != id && t.BoSuuTapID == taiLieu.BoSuuTapID) // Cùng bộ sưu tập
                .OrderBy(t => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
                .Take(4)
                .ToListAsync();

            // Nếu không đủ 4 sách cùng bộ sưu tập, lấy thêm sách ngẫu nhiên
            if (randomBooks.Count < 4)
            {
                var additionalBooks = await _context.TaiLieus
                    .Where(t => t.ID != id && !randomBooks.Select(rb => rb.ID).Contains(t.ID))
                    .OrderBy(t => Guid.NewGuid())
                    .Take(4 - randomBooks.Count)
                    .ToListAsync();

                randomBooks.AddRange(additionalBooks);
            }

            ViewBag.RandomBooks = randomBooks;

            return View(taiLieu);
        }
        // GET: TaiLieu/ReadOnline/5
        public async Task<IActionResult> ReadOnline(int id)
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đọc sách trực tuyến.";
                return RedirectToAction("Login", "Account");
            }

            var taiLieu = await _context.TaiLieus
                .FirstOrDefaultAsync(m => m.ID == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(taiLieu.FilePDF))
            {
                TempData["ErrorMessage"] = "Tài liệu này không có bản PDF để đọc trực tuyến.";
                return RedirectToAction("Details", new { id = id });
            }

            ViewBag.PdfPath = taiLieu.FilePDF;
            ViewBag.TaiLieuTitle = taiLieu.NhanDe;
            return View();
        }
      
    }
}