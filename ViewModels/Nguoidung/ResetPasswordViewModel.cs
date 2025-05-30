using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.NguoiDung
{
    public class ResetPasswordViewModel
    {
        public int UserID { get; set; }

        [Display(Name = "Tên người dùng")]
        public string ?UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string ?NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không khớp.")]
        public string ?ConfirmPassword { get; set; }
    }
}