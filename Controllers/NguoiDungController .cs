using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using libraryproject.Helpers;
using libraryproject.ViewModels.NguoiDung;
using libraryproject.Models;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")]
    public class NguoiDungController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10;

        public NguoiDungController(QLTVContext context)
        {
            _context = context;
        }

        // GET: NguoiDung
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["HoTenSortParam"] = string.IsNullOrEmpty(sortOrder) ? "hoten_desc" : "";
            ViewData["MaBanDocSortParam"] = sortOrder == "mabandoc" ? "mabandoc_desc" : "mabandoc";

            var currentPage = page ?? 1;

            var query = _context.NguoiDungs
                .Include(n => n.LoaiBanDoc)
                .AsQueryable();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(n => n.MaBanDoc.Contains(searchString) ||
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
                case "mabandoc":
                    query = query.OrderBy(n => n.MaBanDoc);
                    break;
                case "mabandoc_desc":
                    query = query.OrderByDescending(n => n.MaBanDoc);
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

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = count == 0 ? 0 : (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(items);
        }

        // GET: NguoiDung/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.LoaiBanDoc)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // GET: NguoiDung/Create
        public async Task<IActionResult> Create()
        {
            // Initialize view model with default values
            var viewModel = new NguoiDungViewModel
            {
                NgayThanhVien = DateTime.Now,
                NgayHetHan = DateTime.Now.AddYears(1),
                Role = "BanDoc"
            };

            await LoadLoaiBanDocData();
            return View(viewModel);
        }

        // POST: NguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NguoiDungViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã bạn đọc tồn tại chưa
                    if (await _context.NguoiDungs.AnyAsync(n => n.MaBanDoc == viewModel.MaBanDoc))
                    {
                        ModelState.AddModelError("MaBanDoc", "Mã bạn đọc đã tồn tại.");
                        await LoadLoaiBanDocData();
                        return View(viewModel);
                    }

                    // Kiểm tra email tồn tại chưa
                    if (await _context.NguoiDungs.AnyAsync(n => n.Email == viewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng.");
                        await LoadLoaiBanDocData();
                        return View(viewModel);
                    }

                    // Convert view model to entity
                    var nguoiDung = viewModel.ToEntity();

                    // Mã hóa mật khẩu
                    nguoiDung.MatKhau = PasswordHasher.HashPassword(viewModel.MatKhau);

                    // Thiết lập các giá trị mặc định
                    nguoiDung.NgayThanhVien = DateTime.Now;
                    nguoiDung.NgayHetHan = DateTime.Now.AddYears(1);
                    nguoiDung.AuthToken = ""; // Giá trị mặc định cho AuthToken

                    _context.Add(nguoiDung);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm người dùng: " + ex.Message);
                }
            }

            await LoadLoaiBanDocData();
            return View(viewModel);
        }

        // GET: NguoiDung/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.LoaiBanDoc)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            // Convert to view model
            var viewModel = NguoiDungViewModel.FromEntity(nguoiDung);

            await LoadLoaiBanDocData();
            return View(viewModel);
        }

        // POST: NguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NguoiDungViewModel viewModel)
        {
            if (id != viewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy người dùng hiện tại từ database
                    var existingUser = await _context.NguoiDungs
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ID == id);

                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra mã bạn đọc đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.MaBanDoc != existingUser.MaBanDoc &&
                        await _context.NguoiDungs.AnyAsync(n => n.MaBanDoc == viewModel.MaBanDoc))
                    {
                        ModelState.AddModelError("MaBanDoc", "Mã bạn đọc đã tồn tại.");
                        await LoadLoaiBanDocData();
                        return View(viewModel);
                    }

                    // Kiểm tra email đã tồn tại chưa (nếu đã thay đổi)
                    if (viewModel.Email != existingUser.Email &&
                        await _context.NguoiDungs.AnyAsync(n => n.Email == viewModel.Email))
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng.");
                        await LoadLoaiBanDocData();
                        return View(viewModel);
                    }

                    // Convert to entity
                    var nguoiDung = viewModel.ToEntity();

                    // Giữ lại mật khẩu cũ nếu không nhập mật khẩu mới
                    if (string.IsNullOrEmpty(viewModel.MatKhau))
                    {
                        nguoiDung.MatKhau = existingUser.MatKhau;
                    }
                    else
                    {
                        // Mã hóa mật khẩu mới
                        nguoiDung.MatKhau = PasswordHasher.HashPassword(viewModel.MatKhau);
                        nguoiDung.AuthToken = ""; // Xóa token buộc người dùng đăng nhập lại
                    }

                    // Giữ lại thông tin không thay đổi
                    nguoiDung.NgayThanhVien = existingUser.NgayThanhVien;

                    if (string.IsNullOrEmpty(viewModel.AuthToken))
                    {
                        nguoiDung.AuthToken = existingUser.AuthToken;
                    }

                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(viewModel.ID))
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
                    ModelState.AddModelError("", "Lỗi khi cập nhật người dùng: " + ex.Message);
                }
            }

            await LoadLoaiBanDocData();
            return View(viewModel);
        }

        // GET: NguoiDung/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.LoaiBanDoc)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        // POST: NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Kiểm tra người dùng có phiếu mượn không
                var hasLoans = await _context.PhieuMuons.AnyAsync(p => p.NguoiDungID == id);
                if (hasLoans)
                {
                    TempData["ErrorMessage"] = "Không thể xóa người dùng vì còn phiếu mượn!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra người dùng có thông báo không và xóa chúng
                var notifications = await _context.ThongBaos.Where(t => t.NguoiDungID == id).ToListAsync();
                if (notifications.Any())
                {
                    _context.ThongBaos.RemoveRange(notifications);
                }

                var nguoiDung = await _context.NguoiDungs.FindAsync(id);
                if (nguoiDung == null)
                {
                    return NotFound();
                }

                _context.NguoiDungs.Remove(nguoiDung);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa người dùng: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // PHƯƠNG THỨC GIA HẠN THẺ
        // GET: NguoiDung/GiaHanThe/5
        public async Task<IActionResult> GiaHanThe(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .Include(n => n.LoaiBanDoc)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            var viewModel = new GiaHanTheViewModel
            {
                UserID = nguoiDung.ID,
                HoTen = nguoiDung.HoTen,
                MaBanDoc = nguoiDung.MaBanDoc,
                NgayHetHanHienTai = nguoiDung.NgayHetHan,
                SoThangGiaHan = 12 // Mặc định là 12 tháng
            };

            return View(viewModel);
        }

        // POST: NguoiDung/GiaHanThe/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiaHanThe(GiaHanTheViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.SoThangGiaHan <= 0 || viewModel.SoThangGiaHan > 36)
                {
                    ModelState.AddModelError("SoThangGiaHan", "Số tháng gia hạn phải từ 1 đến 36 tháng!");
                    return View(viewModel);
                }

                try
                {
                    var nguoiDung = await _context.NguoiDungs.FindAsync(viewModel.UserID);
                    if (nguoiDung == null)
                    {
                        return NotFound();
                    }

                    // Gia hạn thẻ thư viện
                    DateTime ngayHetHanMoi;
                    if (nguoiDung.NgayHetHan < DateTime.Now)
                    {
                        // Nếu thẻ đã hết hạn, tính từ ngày hiện tại
                        ngayHetHanMoi = DateTime.Now.AddMonths(viewModel.SoThangGiaHan);
                    }
                    else
                    {
                        // Nếu thẻ chưa hết hạn, cộng thêm vào ngày hết hạn hiện tại
                        ngayHetHanMoi = nguoiDung.NgayHetHan.AddMonths(viewModel.SoThangGiaHan);
                    }

                    nguoiDung.NgayHetHan = ngayHetHanMoi;
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();

                    // Tạo thông báo cho người dùng
                    var thongBao = new ThongBao
                    {
                        NguoiDungID = nguoiDung.ID,
                        TieuDe = "Gia hạn thẻ thư viện",
                        NoiDung = $"Thẻ thư viện của bạn đã được gia hạn đến ngày {ngayHetHanMoi.ToString("dd/MM/yyyy")}.",
                        NgayTao = DateTime.Now,
                        DaDoc = false,
                        LoaiThongBao = "ThongBaoKhac"
                    };

                    _context.ThongBaos.Add(thongBao);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Gia hạn thẻ thành công! Ngày hết hạn mới: {ngayHetHanMoi.ToString("dd/MM/yyyy")}";
                    return RedirectToAction(nameof(Details), new { id = viewModel.UserID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi gia hạn thẻ: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // PHƯƠNG THỨC ĐẶT LẠI MẬT KHẨU
        // GET: NguoiDung/ResetPassword/5
        public async Task<IActionResult> ResetPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.ID == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            var viewModel = new ResetPasswordViewModel
            {
                UserID = nguoiDung.ID,
                UserName = nguoiDung.HoTen
            };

            return View(viewModel);
        }

        // POST: NguoiDung/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var nguoiDung = await _context.NguoiDungs.FindAsync(viewModel.UserID);
                    if (nguoiDung == null)
                    {
                        return NotFound();
                    }

                    // Mã hóa mật khẩu mới
                    nguoiDung.MatKhau = PasswordHasher.HashPassword(viewModel.NewPassword);
                    nguoiDung.AuthToken = ""; // Xóa token để buộc đăng nhập lại
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();

                    // Tạo thông báo cho người dùng
                    var thongBao = new ThongBao
                    {
                        NguoiDungID = nguoiDung.ID,
                        TieuDe = "Đặt lại mật khẩu",
                        NoiDung = "Mật khẩu của bạn đã được đặt lại. Nếu bạn không thực hiện thao tác này, vui lòng liên hệ với quản trị viên.",
                        NgayTao = DateTime.Now,
                        DaDoc = false,
                        LoaiThongBao = "ThongBaoKhac"
                    };

                    _context.ThongBaos.Add(thongBao);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công!";
                    return RedirectToAction(nameof(Details), new { id = viewModel.UserID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi đặt lại mật khẩu: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.ID == id);
        }

        private async Task LoadLoaiBanDocData()
        {
            ViewBag.LoaiBanDocs = await _context.LoaiBanDocs
                .Select(l => new SelectListItem
                {
                    Value = l.ID.ToString(),
                    Text = l.TenLoai
                })
                .ToListAsync();
        }
    }
}