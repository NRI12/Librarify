// ViewModels/TaiLieu/TaiLieuSearchViewModel.cs
using libraryproject.Data;
using System.Collections.Generic;

namespace libraryproject.ViewModels.TaiLieu
{
    public class TaiLieuSearchViewModel
    {
        // Các trường tìm kiếm
        public string NhanDe { get; set; }
        public string TacGia { get; set; }
        public string NhaXuatBan { get; set; }
        public int NamXuatBan { get; set; }
        public int BoSuuTapID { get; set; }
        public string ThuatNguChuDe { get; set; }

        // Phân trang
        public int? Page { get; set; }

        // Kết quả tìm kiếm
        public List<Models.TaiLieu> Results { get; set; }
    }
}