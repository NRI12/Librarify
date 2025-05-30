using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using libraryproject.Models;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")] // Đảm bảo chỉ Admin mới có thể truy cập controller này
    public class LoaiBanDocController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10;

        public LoaiBanDocController(QLTVContext context)
        {
            _context = context;
        }

        // GET: LoaiBanDoc
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["TenSortParam"] = string.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";

            var currentPage = page ?? 1;

            var query = _context.LoaiBanDocs.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.TenLoai.Contains(searchString) ||
                                   l.MoTa.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "ten_desc":
                    query = query.OrderByDescending(l => l.TenLoai);
                    break;
                default:
                    query = query.OrderBy(l => l.TenLoai);
                    break;
            }

            // Tổng số bản ghi
            var count = await query.CountAsync();

            // Phân trang
            var items = await query.Skip((currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(items);
        }

        // GET: LoaiBanDoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBanDoc = await _context.LoaiBanDocs
                .FirstOrDefaultAsync(m => m.ID == id);

            if (loaiBanDoc == null)
            {
                return NotFound();
            }

            return View(loaiBanDoc);
        }

        // GET: LoaiBanDoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiBanDoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoai,MoTa")] LoaiBanDoc loaiBanDoc)
        {
            // Loại bỏ validation cho NguoiDungs
            ModelState.Remove("NguoiDungs");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(loaiBanDoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm loại bạn đọc thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm loại bạn đọc: " + ex.Message);
                }
            }
            return View(loaiBanDoc);
        }

        // GET: LoaiBanDoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBanDoc = await _context.LoaiBanDocs.FindAsync(id);
            if (loaiBanDoc == null)
            {
                return NotFound();
            }
            return View(loaiBanDoc);
        }

        // POST: LoaiBanDoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,TenLoai,MoTa")] LoaiBanDoc loaiBanDoc)
        {
            if (id != loaiBanDoc.ID)
            {
                return NotFound();
            }

            // Loại bỏ NguoiDungs khỏi validation - điều này đã hoạt động
            ModelState.Remove("NguoiDungs");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy entity hiện có và chỉ cập nhật các thuộc tính chúng ta quan tâm
                    var existingLoaiBanDoc = await _context.LoaiBanDocs.FindAsync(id);
                    if (existingLoaiBanDoc == null)
                    {
                        return NotFound();
                    }

                    existingLoaiBanDoc.TenLoai = loaiBanDoc.TenLoai;
                    existingLoaiBanDoc.MoTa = loaiBanDoc.MoTa;

                    _context.Update(existingLoaiBanDoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật loại bạn đọc thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiBanDocExists(loaiBanDoc.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loaiBanDoc);
        }

        // GET: LoaiBanDoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiBanDoc = await _context.LoaiBanDocs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (loaiBanDoc == null)
            {
                return NotFound();
            }

            return View(loaiBanDoc);
        }

        // POST: LoaiBanDoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Loại bỏ validation cho NguoiDungs
            ModelState.Remove("NguoiDungs");

            // Kiểm tra xem có bất kỳ người dùng nào đang sử dụng loại bạn đọc này không
            var isInUse = await _context.NguoiDungs.AnyAsync(n => n.LoaiBanDocID == id);
            if (isInUse)
            {
                TempData["ErrorMessage"] = "Không thể xóa loại bạn đọc này vì đang được sử dụng!";
                return RedirectToAction(nameof(Index));
            }

            var loaiBanDoc = await _context.LoaiBanDocs.FindAsync(id);
            if (loaiBanDoc == null)
            {
                return NotFound();
            }

            try
            {
                _context.LoaiBanDocs.Remove(loaiBanDoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa loại bạn đọc thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi xảy ra khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoaiBanDocExists(int id)
        {
            return _context.LoaiBanDocs.Any(e => e.ID == id);
        }
    }
}