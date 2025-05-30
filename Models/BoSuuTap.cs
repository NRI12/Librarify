namespace libraryproject.Models
{
    public class BoSuuTap
    {
        public int ID { get; set; }
        public string? TenBoSuuTap { get; set; }
        public string? MoTa { get; set; }

        // Navigation property
        public virtual ICollection<TaiLieu> TaiLieus { get; set; }
    }

}
