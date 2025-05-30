using libraryproject.Data;
using libraryproject.Models;
using System.Collections.Generic;


namespace libraryproject.ViewModels.Account
{
    public class ProfileViewModel
    {
        public Models.NguoiDung NguoiDung { get; set; }
        public List<PhieuMuon> PhieuMuonDangMuon { get; set; }
        public List<PhieuMuon> LichSuMuonTra { get; set; }
        public List<ThongBao> ThongBao { get; set; }
    }
}