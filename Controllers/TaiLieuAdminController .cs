using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using libraryproject.ViewModels.TaiLieu;
using libraryproject.Models;
using libraryproject.Helpers;
using Microsoft.AspNetCore.Http;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")]
    public class TaiLieuAdminController : Controller
    {
        private readonly QLTVContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly int _pageSize = 10;

        public TaiLieuAdminController(QLTVContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: TaiLieuAdmin
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NhanDeSortParam"] = string.IsNullOrEmpty(sortOrder) ? "nhande_desc" : "";
            ViewData["TacGiaSortParam"] = sortOrder == "tacgia" ? "tacgia_desc" : "tacgia";
            ViewData["NamXuatBanSortParam"] = sortOrder == "namxuatban" ? "namxuatban_desc" : "namxuatban";

            var currentPage = page ?? 1;

            var query = _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => t.NhanDe.Contains(searchString) ||
                                      t.TacGia.Contains(searchString) ||
                                      t.MaSach.Contains(searchString) ||
                                      t.NhaXuatBan.Contains(searchString) ||
                                      t.ThuatNguChuDe.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "nhande_desc":
                    query = query.OrderByDescending(t => t.NhanDe);
                    break;
                case "tacgia":
                    query = query.OrderBy(t => t.TacGia);
                    break;
                case "tacgia_desc":
                    query = query.OrderByDescending(t => t.TacGia);
                    break;
                case "namxuatban":
                    query = query.OrderBy(t => t.NamXuatBan);
                    break;
                case "namxuatban_desc":
                    query = query.OrderByDescending(t => t.NamXuatBan);
                    break;
                default:
                    query = query.OrderBy(t => t.NhanDe);
                    break;
            }

            // Tổng số bản ghi
            var count = await query.CountAsync();

            // Phân trang
            var items = await query.Skip((currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToListAsync();

            // Chuyển đổi từ entity sang view model
            var viewModels = items.Select(TaiLieuViewModel.FromEntity).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = count == 0 ? 0 : (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(viewModels);
        }

        // GET: TaiLieuAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            var viewModel = TaiLieuViewModel.FromEntity(taiLieu);
            return View(viewModel);
        }

        // GET: TaiLieuAdmin/Create
        public async Task<IActionResult> Create()
        {
            await LoadBoSuuTapData();

            // Initialize view model with default values
            var viewModel = new TaiLieuViewModel
            {
                NgayNhap = DateTime.Now,
                SoLuongHienCo = 0,
                SoLuongNhapVe = 0
            };

            return View(viewModel);
        }

        // POST: TaiLieuAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiLieuViewModel viewModel)
        {
            // Kiểm tra các trường bắt buộc
            ValidateTaiLieu(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã sách đã tồn tại chưa
                    if (await _context.TaiLieus.AnyAsync(t => t.MaSach == viewModel.MaSach))
                    {
                        ModelState.AddModelError("MaSach", "Mã sách đã tồn tại");
                        await LoadBoSuuTapData();
                        return View(viewModel);
                    }

                    // Xử lý upload hình ảnh
                    if (viewModel.HinhAnhFile != null)
                    {
                        string fileName = await UploadFile(viewModel.HinhAnhFile, "books");
                        viewModel.HinhAnh = fileName;
                    }

                    // Xử lý upload file PDF
                    if (viewModel.FilePDFFile != null)
                    {
                        string fileName = await UploadFile(viewModel.FilePDFFile, "pdfs");
                        viewModel.FilePDF = fileName;
                    }

                    // Convert view model to entity
                    var taiLieu = viewModel.ToEntity();

                    // Mặc định số lượng hiện có bằng số lượng nhập về
                    if (taiLieu.SoLuongHienCo <= 0)
                    {
                        taiLieu.SoLuongHienCo = taiLieu.SoLuongNhapVe;
                    }

                    // Thiết lập ngày nhập là ngày hiện tại nếu chưa có
                    if (taiLieu.NgayNhap == DateTime.MinValue)
                    {
                        taiLieu.NgayNhap = DateTime.Now;
                    }

                    _context.Add(taiLieu);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm tài liệu thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm tài liệu: " + ex.Message);
                }
            }

            await LoadBoSuuTapData();
            return View(viewModel);
        }

        // GET: TaiLieuAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            var viewModel = TaiLieuViewModel.FromEntity(taiLieu);
            await LoadBoSuuTapData();
            return View(viewModel);
        }

        // POST: TaiLieuAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiLieuViewModel viewModel)
        {
            if (id != viewModel.ID)
            {
                return NotFound();
            }

            // Kiểm tra các trường bắt buộc
            ValidateTaiLieu(viewModel);

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy tài liệu hiện tại từ database
                    var existingTaiLieu = await _context.TaiLieus
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.ID == id);

                    if (existingTaiLieu == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mã sách đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.MaSach != existingTaiLieu.MaSach &&
                        await _context.TaiLieus.AnyAsync(t => t.MaSach == viewModel.MaSach))
                    {
                        ModelState.AddModelError("MaSach", "Mã sách đã tồn tại");
                        await LoadBoSuuTapData();
                        return View(viewModel);
                    }

                    // Xử lý upload hình ảnh
                    if (viewModel.HinhAnhFile != null)
                    {
                        // Xóa hình ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingTaiLieu.HinhAnh))
                        {
                            DeleteFile(existingTaiLieu.HinhAnh, "books");
                        }

                        string fileName = await UploadFile(viewModel.HinhAnhFile, "books");
                        viewModel.HinhAnh = fileName;
                    }
                    else
                    {
                        // Giữ lại hình ảnh cũ
                        viewModel.HinhAnh = existingTaiLieu.HinhAnh;
                    }

                    // Xử lý upload file PDF
                    if (viewModel.FilePDFFile != null)
                    {
                        // Xóa file PDF cũ nếu có
                        if (!string.IsNullOrEmpty(existingTaiLieu.FilePDF))
                        {
                            DeleteFile(existingTaiLieu.FilePDF, "pdfs");
                        }

                        string fileName = await UploadFile(viewModel.FilePDFFile, "pdfs");
                        viewModel.FilePDF = fileName;
                    }
                    else
                    {
                        // Giữ lại file PDF cũ
                        viewModel.FilePDF = existingTaiLieu.FilePDF;
                    }

                    // Convert to entity
                    var taiLieu = viewModel.ToEntity();

                    _context.Update(taiLieu);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tài liệu thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiLieuExists(viewModel.ID))
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
                    ModelState.AddModelError("", "Lỗi khi cập nhật tài liệu: " + ex.Message);
                }
            }

            await LoadBoSuuTapData();
            return View(viewModel);
        }

        // GET: TaiLieuAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiLieu = await _context.TaiLieus
                .Include(t => t.BoSuuTap)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (taiLieu == null)
            {
                return NotFound();
            }

            var viewModel = TaiLieuViewModel.FromEntity(taiLieu);
            return View(viewModel);
        }

        // POST: TaiLieuAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Kiểm tra tài liệu có đang được mượn không
                var inUse = await _context.ChiTietPhieuMuons
                    .AnyAsync(c => c.TaiLieuID == id && c.TrangThai == "Đang mượn");

                if (inUse)
                {
                    TempData["ErrorMessage"] = "Không thể xóa tài liệu vì đang được mượn!";
                    return RedirectToAction(nameof(Index));
                }

                var taiLieu = await _context.TaiLieus.FindAsync(id);
                if (taiLieu == null)
                {
                    return NotFound();
                }

                // Xóa hình ảnh và file PDF nếu có
                if (!string.IsNullOrEmpty(taiLieu.HinhAnh))
                {
                    DeleteFile(taiLieu.HinhAnh, "books");
                }

                if (!string.IsNullOrEmpty(taiLieu.FilePDF))
                {
                    DeleteFile(taiLieu.FilePDF, "pdfs");
                }

                // Xóa các mã vạch liên quan
                var maVachs = await _context.MaVachs.Where(m => m.TaiLieuID == id).ToListAsync();
                if (maVachs.Any())
                {
                    _context.MaVachs.RemoveRange(maVachs);
                }

                // Xóa tài liệu
                _context.TaiLieus.Remove(taiLieu);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa tài liệu thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa tài liệu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TaiLieuExists(int id)
        {
            return _context.TaiLieus.Any(e => e.ID == id);
        }

        private async Task LoadBoSuuTapData()
        {
            ViewBag.BoSuuTaps = await _context.BoSuuTaps
                .Select(b => new SelectListItem
                {
                    Value = b.ID.ToString(),
                    Text = b.TenBoSuuTap
                })
                .ToListAsync();
        }

        private void ValidateTaiLieu(TaiLieuViewModel viewModel)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(viewModel.MaSach))
                ModelState.AddModelError("MaSach", "Vui lòng nhập mã sách");

            if (string.IsNullOrEmpty(viewModel.NhanDe))
                ModelState.AddModelError("NhanDe", "Vui lòng nhập nhan đề");

            if (string.IsNullOrEmpty(viewModel.TacGia))
                ModelState.AddModelError("TacGia", "Vui lòng nhập tên tác giả");

            if (string.IsNullOrEmpty(viewModel.NhaXuatBan))
                ModelState.AddModelError("NhaXuatBan", "Vui lòng nhập nhà xuất bản");

            if (viewModel.NamXuatBan <= 0)
                ModelState.AddModelError("NamXuatBan", "Vui lòng nhập năm xuất bản");
            else if (viewModel.NamXuatBan < 1800 || viewModel.NamXuatBan > DateTime.Now.Year)
                ModelState.AddModelError("NamXuatBan", $"Năm xuất bản phải từ 1800 đến {DateTime.Now.Year}");

            if (viewModel.BoSuuTapID <= 0)
                ModelState.AddModelError("BoSuuTapID", "Vui lòng chọn bộ sưu tập");

            // Kiểm tra file hình ảnh
            if (viewModel.HinhAnhFile != null)
            {
                var extension = Path.GetExtension(viewModel.HinhAnhFile.FileName).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                {
                    ModelState.AddModelError("HinhAnhFile", "Chỉ chấp nhận file hình ảnh có định dạng .jpg, .jpeg, .png hoặc .gif");
                }
                else if (viewModel.HinhAnhFile.Length > 10485760) // 10MB
                {
                    ModelState.AddModelError("HinhAnhFile", "Kích thước file không được vượt quá 10MB");
                }
            }

            // Kiểm tra file PDF
            if (viewModel.FilePDFFile != null)
            {
                var extension = Path.GetExtension(viewModel.FilePDFFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("FilePDFFile", "Chỉ chấp nhận file có định dạng .pdf");
                }
                else if (viewModel.FilePDFFile.Length > 104857600) // 100MB
                {
                    ModelState.AddModelError("FilePDFFile", "Kích thước file không được vượt quá 100MB");
                }
            }
        }

        private async Task<string> UploadFile(IFormFile file, string folder)
        {
            if (file == null)
            {
                return null;
            }

            string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "uploads", folder);

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Tạo tên file duy nhất
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadDir, fileName);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        private void DeleteFile(string fileName, string folder)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string filePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", folder, fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}