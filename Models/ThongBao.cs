using libraryproject.Models;
using System;

namespace libraryproject
{
    public class ThongBao
    {
        public int ID { get; set; }
        public int NguoiDungID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public bool DaDoc { get; set; }
        public string LoaiThongBao { get; set; } // SapDenHan, QuaHan, ThongBaoKhac

        // Navigation property
        public virtual Models.NguoiDung NguoiDung { get; set; }
    }
}