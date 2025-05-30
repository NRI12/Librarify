using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using libraryproject.ViewModels.NhanVien;
using libraryproject.Models;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")]
    public class NhanVienController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10;

        public NhanVienController(QLTVContext context)
        {
            _context = context;
        }

        // GET: NhanVien
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["HoTenSortParam"] = string.IsNullOrEmpty(sortOrder) ? "hoten_desc" : "";
            ViewData["MaNhanVienSortParam"] = sortOrder == "manhanvien" ? "manhanvien_desc" : "manhanvien";
            ViewData["NgaySinhSortParam"] = sortOrder == "ngaysinh" ? "ngaysinh_desc" : "ngaysinh";

            var currentPage = page ?? 1;

            var query = _context.NhanViens.AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.MaNhanVien.Contains(searchString) ||
                                      n.HoTen.Contains(searchString) ||
                                      n.Email.Contains(searchString) ||
                                      n.SDT.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "hoten_desc":
                    query = query.OrderByDescending(n => n.HoTen);
                    break;
                case "manhanvien":
                    query = query.OrderBy(n => n.MaNhanVien);
                    break;
                case "manhanvien_desc":
                    query = query.OrderByDescending(n => n.MaNhanVien);
                    break;
                case "ngaysinh":
                    query = query.OrderBy(n => n.NgaySinh);
                    break;
                case "ngaysinh_desc":
                    query = query.OrderByDescending(n => n.NgaySinh);
                    break;
                default:
                    query = query.OrderBy(n => n.HoTen);
                    break;
            }

            // Tổng số bản ghi
            var count = await query.CountAsync();

            // Phân trang
            var items = await query.Skip((currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToListAsync();

            // Chuyển đổi sang view model
            var viewModels = items.Select(NhanVienViewModel.FromEntity).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = count == 0 ? 0 : (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(viewModels);
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            var viewModel = NhanVienViewModel.FromEntity(nhanVien);
            return View(viewModel);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            return View(new NhanVienViewModel { NgaySinh = DateTime.Now.AddYears(-20) });
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVienViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã nhân viên đã tồn tại chưa
                    if (await _context.NhanViens.AnyAsync(n => n.MaNhanVien == viewModel.MaNhanVien))
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên đã tồn tại");
                        return View(viewModel);
                    }

                    // Kiểm tra email đã tồn tại chưa
                    if (await _context.NhanViens.AnyAsync(n => n.Email == viewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng");
                        return View(viewModel);
                    }

                    // Convert view model to entity
                    var nhanVien = viewModel.ToEntity();

                    _context.Add(nhanVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm nhân viên: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            var viewModel = NhanVienViewModel.FromEntity(nhanVien);
            return View(viewModel);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhanVienViewModel viewModel)
        {
            if (id != viewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy nhân viên hiện tại từ database
                    var existingNhanVien = await _context.NhanViens
                        .AsNoTracking()
                        .FirstOrDefaultAsync(n => n.ID == id);

                    if (existingNhanVien == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mã nhân viên đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.MaNhanVien != existingNhanVien.MaNhanVien &&
                        await _context.NhanViens.AnyAsync(n => n.MaNhanVien == viewModel.MaNhanVien))
                    {
                        ModelState.AddModelError("MaNhanVien", "Mã nhân viên đã tồn tại");
                        return View(viewModel);
                    }

                    // Kiểm tra email đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.Email != existingNhanVien.Email &&
                        await _context.NhanViens.AnyAsync(n => n.Email == viewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng");
                        return View(viewModel);
                    }

                    // Convert to entity
                    var nhanVien = viewModel.ToEntity();

                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(viewModel.ID))
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
                    ModelState.AddModelError("", "Lỗi khi cập nhật nhân viên: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nhanVien == null)
            {
                return NotFound();
            }

            var viewModel = NhanVienViewModel.FromEntity(nhanVien);
            return View(viewModel);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Kiểm tra nhân viên có phiếu mượn không
                var hasLoans = await _context.PhieuMuons.AnyAsync(p => p.NhanVienID == id);
                if (hasLoans)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhân viên vì còn phiếu mượn liên quan!";
                    return RedirectToAction(nameof(Index));
                }

                var nhanVien = await _context.NhanViens.FindAsync(id);
                if (nhanVien == null)
                {
                    return NotFound();
                }

                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa nhân viên: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienExists(int id)
        {
            return _context.NhanViens.Any(e => e.ID == id);
        }
    }
}