namespace libraryproject.Models
{
    public class LoaiBanDoc
    {
        public int ID { get; set; }
        public string? TenLoai { get; set; }
        public string? MoTa { get; set; }

        public virtual ICollection<NguoiDung> NguoiDungs { get; set; }
    }

}
