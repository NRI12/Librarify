using System.Text.RegularExpressions;

namespace libraryproject.Models
{
    public class TaiLieu
    {
        public int ID { get; set; }
        public string? MaSach { get; set; }
        public string? DDC { get; set; }
        public string? Cutter { get; set; }
        public string? TacGia { get; set; }
        public string? NhanDe { get; set; }
        public string? PhanConLaiNhanDe { get; set; }
        public string? ThongTinTrachNhiem { get; set; }
        public string? NhanDeSongSong { get; set; }
        public string? NoiXuatBan { get; set; }
        public string? NhaXuatBan { get; set; }
        public int NamXuatBan { get; set; }
        public int TongSoTrang { get; set; }
        public string? KhoGiay { get; set; }
        public string? PhuChu { get; set; }
        public string? ThuatNguChuDe { get; set; }
        public string? Khoa { get; set; }
        public string? NoiLuuTru { get; set; }
        public string? KyHieuKho { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuongNhapVe { get; set; }
        public int SoLuongHienCo { get; set; }
        public int BoSuuTapID { get; set; }
        public string? FilePDF { get; set; }
        public DateTime NgayNhap { get; set; }

        // Navigation properties
        public virtual BoSuuTap BoSuuTap { get; set; }
        public virtual ICollection<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
        public virtual ICollection<MaVach> MaVachs { get; set; }
    }
}
