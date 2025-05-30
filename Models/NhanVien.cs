namespace libraryproject.Models
{
    public class NhanVien
    {
        public int ID { get; set; }
        public string? MaNhanVien { get; set; }
        public string? HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string? Email { get; set; }
        public string? SDT { get; set; }

        // Navigation property
        public virtual ICollection<PhieuMuon> PhieuMuons { get; set; }
    }

}
