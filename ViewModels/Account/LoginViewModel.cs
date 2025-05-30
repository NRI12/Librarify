
// Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Mã bạn đọc")]
        [Display(Name = "Mã bạn đọc")]
        public string MaBanDoc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập?")]
        public bool RememberMe { get; set; }
    }
}