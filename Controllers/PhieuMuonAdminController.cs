using libraryproject.Data;
using libraryproject.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using libraryproject.ViewModels.PhieuMuonAdmin;
using libraryproject.Models;

namespace libraryproject.Controllers
{
    [CustomAuthorize("Admin")]
    public class PhieuMuonAdminController : Controller
    {
        private readonly QLTVContext _context;
        private readonly int _pageSize = 10;

        public PhieuMuonAdminController(QLTVContext context)
        {
            _context = context;
        }

        // GET: PhieuMuonAdmin
        public async Task<IActionResult> Index(string searchString, string sortOrder, string trangThai, int? page)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentTrangThai"] = trangThai ?? "";
            ViewData["MaPhieuMuonSortParam"] = string.IsNullOrEmpty(sortOrder) ? "maphieumuon_desc" : "";
            ViewData["NgayMuonSortParam"] = sortOrder == "ngaymuon" ? "ngaymuon_desc" : "ngaymuon";
            ViewData["NgayHenTraSortParam"] = sortOrder == "ngayhentra" ? "ngayhentra_desc" : "ngayhentra";
            ViewData["TrangThaiSortParam"] = sortOrder == "trangthai" ? "trangthai_desc" : "trangthai";

            var currentPage = page ?? 1;

            // Kiểm tra xem có phiếu mượn nào không
            var hasAnyPhieuMuon = await _context.PhieuMuons.AnyAsync();

            // Chuẩn bị danh sách trạng thái cho dropdown
            ViewBag.TrangThais = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Tất cả" },
                new SelectListItem { Value = "Đang mượn", Text = "Đang mượn" },
                new SelectListItem { Value = "Đã trả", Text = "Đã trả" },
                new SelectListItem { Value = "Quá hạn", Text = "Quá hạn" },
                new SelectListItem { Value = "Đã hủy", Text = "Đã hủy" }
            };

            // Nếu không có phiếu mượn nào, trả về danh sách trống với thông tin phân trang
            if (!hasAnyPhieuMuon)
            {
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;
                ViewBag.PageStart = 0;
                ViewBag.PageEnd = 0;
                ViewBag.TotalItems = 0;

                return View(new List<PhieuMuonAdminViewModel>());
            }

            var query = _context.PhieuMuons
                .Include(p => p.NguoiDung)
                .Include(p => p.NhanVien)
                .Include(p => p.ChiTietPhieuMuons)
                    .ThenInclude(c => c.TaiLieu)
                .AsQueryable();

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(p => p.TrangThai == trangThai);
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.MaPhieuMuon.Contains(searchString) ||
                                     p.NguoiDung.HoTen.Contains(searchString) ||
                                     p.NguoiDung.MaBanDoc.Contains(searchString) ||
                                     p.NhanVien.HoTen.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "maphieumuon_desc":
                    query = query.OrderByDescending(p => p.MaPhieuMuon);
                    break;
                case "ngaymuon":
                    query = query.OrderBy(p => p.NgayMuon);
                    break;
                case "ngaymuon_desc":
                    query = query.OrderByDescending(p => p.NgayMuon);
                    break;
                case "ngayhentra":
                    query = query.OrderBy(p => p.NgayHenTra);
                    break;
                case "ngayhentra_desc":
                    query = query.OrderByDescending(p => p.NgayHenTra);
                    break;
                case "trangthai":
                    query = query.OrderBy(p => p.TrangThai);
                    break;
                case "trangthai_desc":
                    query = query.OrderByDescending(p => p.TrangThai);
                    break;
                default:
                    query = query.OrderByDescending(p => p.NgayMuon);
                    break;
            }

            // Tổng số bản ghi
            var count = await query.CountAsync();

            // Nếu sau khi lọc không có kết quả nào
            if (count == 0)
            {
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;
                ViewBag.PageStart = 0;
                ViewBag.PageEnd = 0;
                ViewBag.TotalItems = 0;

                return View(new List<PhieuMuonAdminViewModel>());
            }

            // Phân trang
            var items = await query.Skip((currentPage - 1) * _pageSize)
                                  .Take(_pageSize)
                                  .ToListAsync();

            // Chuyển đổi sang view model
            var viewModels = items.Select(PhieuMuonAdminViewModel.FromEntity).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)_pageSize);
            ViewBag.CurrentPage = currentPage;
            ViewBag.HasPreviousPage = currentPage > 1;
            ViewBag.HasNextPage = currentPage < ViewBag.TotalPages;
            ViewBag.PageStart = (currentPage - 1) * _pageSize + 1;
            ViewBag.PageEnd = Math.Min(currentPage * _pageSize, count);
            ViewBag.TotalItems = count;

            return View(viewModels);
        }

        // GET: PhieuMuonAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuMuon = await _context.PhieuMuons
                .Include(p => p.NguoiDung)
                .Include(p => p.NhanVien)
                .Include(p => p.ChiTietPhieuMuons)
                    .ThenInclude(c => c.TaiLieu)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (phieuMuon == null)
            {
                return NotFound();
            }

            var viewModel = PhieuMuonAdminViewModel.FromEntity(phieuMuon);
            return View(viewModel);
        }

        // GET: PhieuMuonAdmin/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra xem có NguoiDung nào không
            var hasNguoiDung = await _context.NguoiDungs.AnyAsync(n => n.Role == "BanDoc");
            if (!hasNguoiDung)
            {
                TempData["ErrorMessage"] = "Chưa có bạn đọc nào trong hệ thống! Vui lòng thêm bạn đọc trước.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem có NhanVien nào không
            var hasNhanVien = await _context.NhanViens.AnyAsync();
            if (!hasNhanVien)
            {
                TempData["ErrorMessage"] = "Chưa có nhân viên nào trong hệ thống! Vui lòng thêm nhân viên trước.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xem có TaiLieu nào có sẵn không
            var hasTaiLieu = await _context.TaiLieus.AnyAsync(t => t.SoLuongHienCo > 0);
            if (!hasTaiLieu)
            {
                TempData["ErrorMessage"] = "Chưa có tài liệu nào có sẵn trong hệ thống! Vui lòng thêm tài liệu trước.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách bạn đọc
            var nguoiDungs = await _context.NguoiDungs
                .Where(n => n.Role == "BanDoc" && n.NgayHetHan >= DateTime.Now)
                .Select(n => new SelectListItem
                {
                    Value = n.ID.ToString(),
                    Text = $"{n.MaBanDoc} - {n.HoTen}"
                })
                .ToListAsync();

            // Lấy danh sách nhân viên
            var nhanViens = await _context.NhanViens
                .Select(n => new SelectListItem
                {
                    Value = n.ID.ToString(),
                    Text = $"{n.MaNhanVien} - {n.HoTen}"
                })
                .ToListAsync();

            // Lấy danh sách tài liệu có sẵn
            var taiLieus = await _context.TaiLieus
                .Where(t => t.SoLuongHienCo > 0)
                .Select(t => new SelectListItem
                {
                    Value = t.ID.ToString(),
                    Text = $"{t.MaSach} - {t.NhanDe} ({t.TacGia})"
                })
                .ToListAsync();

            ViewBag.NguoiDungs = nguoiDungs;
            ViewBag.NhanViens = nhanViens;
            ViewBag.TaiLieus = taiLieus;

            // Tạo mã phiếu mượn mới dựa trên ngày hiện tại
            var today = DateTime.Now;
            var maPhieuMuon = $"PM{today:yyyyMMdd}-{await GetNextPhieuMuonSequence()}";

            // Initialize view model with default values
            var viewModel = new PhieuMuonAdminViewModel
            {
                MaPhieuMuon = maPhieuMuon,
                NgayMuon = DateTime.Now,
                NgayHenTra = DateTime.Now.AddDays(14),
                ChiTietPhieuMuons = new List<ChiTietPhieuMuonAdminViewModel>()
            };

            return View(viewModel);
        }

        // POST: PhieuMuonAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuMuonAdminViewModel viewModel, int[] selectedTaiLieus)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(viewModel.MaPhieuMuon))
                ModelState.AddModelError("MaPhieuMuon", "Vui lòng nhập mã phiếu mượn");

            if (viewModel.NguoiDungID <= 0)
                ModelState.AddModelError("NguoiDungID", "Vui lòng chọn người mượn");

            if (viewModel.NhanVienID <= 0)
                ModelState.AddModelError("NhanVienID", "Vui lòng chọn nhân viên");

            if (selectedTaiLieus == null || selectedTaiLieus.Length == 0)
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một tài liệu");
            if (viewModel.NgayHenTra < viewModel.NgayMuon)
            {
                ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả phải sau hoặc trùng với ngày mượn");
                await LoadRelatedData();
                return View(viewModel);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra mã phiếu mượn đã tồn tại chưa
                    if (await _context.PhieuMuons.AnyAsync(p => p.MaPhieuMuon == viewModel.MaPhieuMuon))
                    {
                        ModelState.AddModelError("MaPhieuMuon", "Mã phiếu mượn đã tồn tại");
                        await LoadRelatedData();
                        return View(viewModel);
                    }

                    // Convert view model to entity
                    var phieuMuon = viewModel.ToEntity();
                    phieuMuon.TrangThai = "Đang mượn";

                    // Lưu phiếu mượn
                    _context.Add(phieuMuon);
                    await _context.SaveChangesAsync();

                    // Lưu chi tiết phiếu mượn
                    foreach (var taiLieuId in selectedTaiLieus)
                    {
                        var chiTiet = new ChiTietPhieuMuon
                        {
                            PhieuMuonID = phieuMuon.ID,
                            TaiLieuID = taiLieuId,
                            TrangThai = "Đang mượn"
                        };

                        _context.Add(chiTiet);

                        // Cập nhật số lượng tài liệu hiện có
                        var taiLieu = await _context.TaiLieus.FindAsync(taiLieuId);
                        if (taiLieu != null && taiLieu.SoLuongHienCo > 0)
                        {
                            taiLieu.SoLuongHienCo--;
                            _context.Update(taiLieu);
                        }
                    }

                    // Thêm thông báo cho người dùng
                    var thongBao = new ThongBao
                    {
                        NguoiDungID = phieuMuon.NguoiDungID,
                        TieuDe = "Mượn sách thành công",
                        NoiDung = $"Bạn đã mượn {selectedTaiLieus.Length} cuốn sách. Ngày hẹn trả: {phieuMuon.NgayHenTra.ToString("dd/MM/yyyy")}.",
                        NgayTao = DateTime.Now,
                        DaDoc = false,
                        LoaiThongBao = "ThongBaoKhac"
                    };

                    _context.ThongBaos.Add(thongBao);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm phiếu mượn thành công!";
                    return RedirectToAction(nameof(Details), new { id = phieuMuon.ID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm phiếu mượn: " + ex.Message);
                }
            }

            await LoadRelatedData();
            return View(viewModel);
        }

        // GET: PhieuMuonAdmin/TraSach/5
        public async Task<IActionResult> TraSach(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuMuon = await _context.PhieuMuons
                .Include(p => p.NguoiDung)
                .Include(p => p.NhanVien)
                .Include(p => p.ChiTietPhieuMuons)
                    .ThenInclude(c => c.TaiLieu)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (phieuMuon == null)
            {
                return NotFound();
            }


            if (phieuMuon.TrangThai != "Đang mượn")
            {
                TempData["ErrorMessage"] = "Phiếu mượn này đã được trả hoặc đã hủy!";
                return RedirectToAction(nameof(Details), new { id = phieuMuon.ID });
            }

            var viewModel = new TraSachAdminViewModel
            {
                PhieuMuonID = phieuMuon.ID,
                MaPhieuMuon = phieuMuon.MaPhieuMuon,
                TenNguoiMuon = phieuMuon.NguoiDung.HoTen,
                MaBanDoc = phieuMuon.NguoiDung.MaBanDoc,
                NgayMuon = phieuMuon.NgayMuon,
                NgayHenTra = phieuMuon.NgayHenTra,
                NgayTra = DateTime.Now,
                ChiTietTraSach = new List<ChiTietTraSachAdminViewModel>()
            };

            foreach (var chiTiet in phieuMuon.ChiTietPhieuMuons.Where(c => c.TrangThai == "Đang mượn"))
            {
                viewModel.ChiTietTraSach.Add(new ChiTietTraSachAdminViewModel
                {
                    ChiTietPhieuMuonID = chiTiet.ID,
                    TaiLieuID = chiTiet.TaiLieuID,
                    TenTaiLieu = chiTiet.TaiLieu.NhanDe,
                    MaSach = chiTiet.TaiLieu.MaSach,
                    TrangThai = "Đã trả"
                });
            }

            return View(viewModel);
        }

        // POST: PhieuMuonAdmin/TraSach/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TraSach(TraSachAdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var phieuMuon = await _context.PhieuMuons
                        .Include(p => p.ChiTietPhieuMuons)
                        .FirstOrDefaultAsync(p => p.ID == viewModel.PhieuMuonID);

                    if (phieuMuon == null)
                    {
                        return NotFound();
                    }

                    bool allReturned = true;
                    if (viewModel.NgayTra < phieuMuon.NgayMuon)
                    {
                        ModelState.AddModelError("NgayTra", "Ngày trả không thể trước ngày mượn");
                        return View(viewModel);
                    }
                    // Cập nhật từng chi tiết phiếu mượn
                    foreach (var chiTiet in viewModel.ChiTietTraSach)
                    {
                        var chiTietPhieuMuon = await _context.ChiTietPhieuMuons.FindAsync(chiTiet.ChiTietPhieuMuonID);
                        if (chiTietPhieuMuon != null)
                        {
                            chiTietPhieuMuon.TrangThai = chiTiet.TrangThai;
                            chiTietPhieuMuon.NgayTra = viewModel.NgayTra;
                            chiTietPhieuMuon.TienPhat = chiTiet.TienPhat;

                            _context.Update(chiTietPhieuMuon);

                            // Cập nhật số lượng tài liệu hiện có nếu sách được trả
                            if (chiTiet.TrangThai == "Đã trả" || chiTiet.TrangThai == "Mất sách")
                            {
                                var taiLieu = await _context.TaiLieus.FindAsync(chiTiet.TaiLieuID);
                                if (taiLieu != null)
                                {
                                    if (chiTiet.TrangThai == "Đã trả")
                                    {
                                        taiLieu.SoLuongHienCo++;
                                    }
                                    _context.Update(taiLieu);
                                }
                            }
                            else
                            {
                                allReturned = false;
                            }
                        }
                    }

                    // Cập nhật trạng thái phiếu mượn
                    phieuMuon.TrangThai = allReturned ? "Đã trả" : "Đang mượn";
                    phieuMuon.GhiChu = viewModel.GhiChu;
                    _context.Update(phieuMuon);

                    // Thêm thông báo cho người dùng
                    var thongBao = new ThongBao
                    {
                        NguoiDungID = phieuMuon.NguoiDungID,
                        TieuDe = "Trả sách thành công",
                        NoiDung = $"Bạn đã trả {viewModel.ChiTietTraSach.Count(c => c.TrangThai == "Đã trả")} cuốn sách vào ngày {viewModel.NgayTra.ToString("dd/MM/yyyy")}.",
                        NgayTao = DateTime.Now,
                        DaDoc = false,
                        LoaiThongBao = "ThongBaoKhac"
                    };

                    _context.ThongBaos.Add(thongBao);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật trạng thái trả sách thành công!";
                    return RedirectToAction(nameof(Details), new { id = phieuMuon.ID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật trạng thái trả sách: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // POST: PhieuMuonAdmin/Huy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id, string lyDo)
        {
            if (string.IsNullOrEmpty(lyDo))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập lý do hủy phiếu mượn!";
                return RedirectToAction(nameof(Details), new { id = id });
            }

            var phieuMuon = await _context.PhieuMuons
                .Include(p => p.ChiTietPhieuMuons)
                .FirstOrDefaultAsync(p => p.ID == id);

            if (phieuMuon == null)
            {
                return NotFound();
            }

            if (phieuMuon.TrangThai != "Đang mượn")
            {
                TempData["ErrorMessage"] = "Phiếu mượn này đã được trả hoặc đã hủy!";
                return RedirectToAction(nameof(Details), new { id = phieuMuon.ID });
            }

            try
            {
                // Cập nhật trạng thái phiếu mượn
                phieuMuon.TrangThai = "Đã hủy";
                phieuMuon.GhiChu = (phieuMuon.GhiChu ?? "") + $" | Hủy vào {DateTime.Now:dd/MM/yyyy HH:mm}: {lyDo}";
                _context.Update(phieuMuon);

                // Cập nhật trạng thái chi tiết phiếu mượn
                foreach (var chiTiet in phieuMuon.ChiTietPhieuMuons.Where(c => c.TrangThai == "Đang mượn"))
                {
                    chiTiet.TrangThai = "Đã hủy";
                    _context.Update(chiTiet);

                    // Cập nhật số lượng tài liệu hiện có
                    var taiLieu = await _context.TaiLieus.FindAsync(chiTiet.TaiLieuID);
                    if (taiLieu != null)
                    {
                        taiLieu.SoLuongHienCo++;
                        _context.Update(taiLieu);
                    }
                }

                // Thêm thông báo cho người dùng
                var thongBao = new ThongBao
                {
                    NguoiDungID = phieuMuon.NguoiDungID,
                    TieuDe = "Phiếu mượn đã bị hủy",
                    NoiDung = $"Phiếu mượn {phieuMuon.MaPhieuMuon} đã bị hủy. Lý do: {lyDo}",
                    NgayTao = DateTime.Now,
                    DaDoc = false,
                    LoaiThongBao = "ThongBaoKhac"
                };

                _context.ThongBaos.Add(thongBao);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hủy phiếu mượn thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi hủy phiếu mượn: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = phieuMuon.ID });
        }

        // Phương thức hỗ trợ
        private async Task LoadRelatedData()
        {
            // Lấy danh sách bạn đọc
            ViewBag.NguoiDungs = await _context.NguoiDungs
                .Where(n => n.Role == "BanDoc" && n.NgayHetHan >= DateTime.Now)
                .Select(n => new SelectListItem
                {
                    Value = n.ID.ToString(),
                    Text = $"{n.MaBanDoc} - {n.HoTen}"
                })
                .ToListAsync();

            // Lấy danh sách nhân viên
            ViewBag.NhanViens = await _context.NhanViens
                .Select(n => new SelectListItem
                {
                    Value = n.ID.ToString(),
                    Text = $"{n.MaNhanVien} - {n.HoTen}"
                })
                .ToListAsync();

            ViewBag.TaiLieus = await _context.TaiLieus
                .Where(t => t.SoLuongHienCo > 0)
                .Select(t => new SelectListItem
                {
                    Value = t.ID.ToString(),
                    Text = $"{t.MaSach} - {t.NhanDe} ({t.TacGia})"
                })
                .ToListAsync();
        }

        private async Task<int> GetNextPhieuMuonSequence()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            // Đếm số phiếu mượn trong ngày
            var countToday = await _context.PhieuMuons
                .Where(p => p.NgayMuon >= today && p.NgayMuon < tomorrow)
                .CountAsync();

            return countToday + 1;
        }
    }
}