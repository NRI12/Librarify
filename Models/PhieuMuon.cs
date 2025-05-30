namespace libraryproject.Models
{
    public class PhieuMuon
    {
        public int ID { get; set; }
        public string? MaPhieuMuon { get; set; }
        public int NguoiDungID { get; set; }
        public int NhanVienID { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }

        // Navigation properties
        public virtual NguoiDung NguoiDung { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
    }
}
