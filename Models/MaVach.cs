namespace libraryproject.Models
{
    public class MaVach
    {
        public int ID { get; set; }
        public int TaiLieuID { get; set; }
        public string? MaBarcode { get; set; }
        public DateTime NgayTao { get; set; }

        // Navigation property
        public virtual TaiLieu TaiLieu { get; set; }
    }
}
