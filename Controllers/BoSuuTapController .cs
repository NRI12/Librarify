using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using libraryproject.ViewModels.BoSuuTap;
using libraryproject.Models;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")]
    public class BoSuuTapController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10;

        public BoSuuTapController(QLTVContext context)
        {
            _context = context;
        }

        // GET: BoSuuTap
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["TenSortParam"] = string.IsNullOrEmpty(sortOrder) ? "ten_desc" : "";

            var currentPage = page ?? 1;

            var query = _context.BoSuuTaps
                .Include(b => b.TaiLieus)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.TenBoSuuTap.Contains(searchString) ||
                                      b.MoTa.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "ten_desc":
                    query = query.OrderByDescending(b => b.TenBoSuuTap);
                    break;
                default:
                    query = query.OrderBy(b => b.TenBoSuuTap);
                    break;
            }

            // Tổng số bản ghi
            var count = await query.CountAsync();

            // Phân trang
            var items = await query.Skip((currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToListAsync();

            // Chuyển đổi từ entity sang view model
            var viewModels = items.Select(BoSuuTapViewModel.FromEntity).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = count == 0 ? 0 : (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(viewModels);
        }

        // GET: BoSuuTap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boSuuTap = await _context.BoSuuTaps
                .Include(b => b.TaiLieus)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (boSuuTap == null)
            {
                return NotFound();
            }

            var viewModel = BoSuuTapViewModel.FromEntity(boSuuTap);
            return View(viewModel);
        }

        // GET: BoSuuTap/Create
        public IActionResult Create()
        {
            return View(new BoSuuTapViewModel());
        }

        // POST: BoSuuTap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoSuuTapViewModel viewModel)
        {
            // Kiểm tra tên bộ sưu tập
            if (string.IsNullOrEmpty(viewModel.TenBoSuuTap))
                ModelState.AddModelError("TenBoSuuTap", "Vui lòng nhập tên bộ sưu tập");

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tên bộ sưu tập đã tồn tại chưa
                    if (await _context.BoSuuTaps.AnyAsync(b => b.TenBoSuuTap == viewModel.TenBoSuuTap))
                    {
                        ModelState.AddModelError("TenBoSuuTap", "Tên bộ sưu tập đã tồn tại");
                        return View(viewModel);
                    }

                    // Convert view model to entity
                    var boSuuTap = viewModel.ToEntity();

                    _context.Add(boSuuTap);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm bộ sưu tập thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm bộ sưu tập: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: BoSuuTap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boSuuTap = await _context.BoSuuTaps
                .Include(b => b.TaiLieus)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (boSuuTap == null)
            {
                return NotFound();
            }

            var viewModel = BoSuuTapViewModel.FromEntity(boSuuTap);
            return View(viewModel);
        }

        // POST: BoSuuTap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BoSuuTapViewModel viewModel)
        {
            if (id != viewModel.ID)
            {
                return NotFound();
            }

            // Kiểm tra tên bộ sưu tập
            if (string.IsNullOrEmpty(viewModel.TenBoSuuTap))
                ModelState.AddModelError("TenBoSuuTap", "Vui lòng nhập tên bộ sưu tập");

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bộ sưu tập hiện tại từ database
                    var existingBoSuuTap = await _context.BoSuuTaps
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.ID == id);

                    if (existingBoSuuTap == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra tên bộ sưu tập đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.TenBoSuuTap != existingBoSuuTap.TenBoSuuTap &&
                        await _context.BoSuuTaps.AnyAsync(b => b.TenBoSuuTap == viewModel.TenBoSuuTap))
                    {
                        ModelState.AddModelError("TenBoSuuTap", "Tên bộ sưu tập đã tồn tại");
                        return View(viewModel);
                    }

                    // Convert to entity
                    var boSuuTap = viewModel.ToEntity();

                    _context.Update(boSuuTap);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật bộ sưu tập thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoSuuTapExists(viewModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật bộ sưu tập: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: BoSuuTap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boSuuTap = await _context.BoSuuTaps
                .Include(b => b.TaiLieus)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (boSuuTap == null)
            {
                return NotFound();
            }

            var viewModel = BoSuuTapViewModel.FromEntity(boSuuTap);
            return View(viewModel);
        }

        // POST: BoSuuTap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Kiểm tra bộ sưu tập có tài liệu không
                var hasTaiLieu = await _context.TaiLieus.AnyAsync(t => t.BoSuuTapID == id);
                if (hasTaiLieu)
                {
                    TempData["ErrorMessage"] = "Không thể xóa bộ sưu tập vì đang có tài liệu thuộc bộ sưu tập này!";
                    return RedirectToAction(nameof(Index));
                }

                var boSuuTap = await _context.BoSuuTaps.FindAsync(id);
                if (boSuuTap == null)
                {
                    return NotFound();
                }

                _context.BoSuuTaps.Remove(boSuuTap);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa bộ sưu tập thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa bộ sưu tập: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BoSuuTapExists(int id)
        {
            return _context.BoSuuTaps.Any(e => e.ID == id);
        }
    }
}