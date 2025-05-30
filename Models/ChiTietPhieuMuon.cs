namespace libraryproject.Models
{
    public class ChiTietPhieuMuon
    {
        public int ID { get; set; }
        public int PhieuMuonID { get; set; }
        public int TaiLieuID { get; set; }
        public DateTime? NgayTra { get; set; }
        public decimal? TienPhat { get; set; }
        public string? TrangThai { get; set; }

        // Navigation properties
        public virtual PhieuMuon PhieuMuon { get; set; }
        public virtual TaiLieu TaiLieu { get; set; }
    }

}
