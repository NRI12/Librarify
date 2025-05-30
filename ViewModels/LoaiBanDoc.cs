using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using libraryproject.Models;

namespace libraryproject.ViewModels
{
    public class LoaiBanDoc
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại bạn đọc")]
        [StringLength(100, ErrorMessage = "Tên loại bạn đọc không được vượt quá 100 ký tự")]
        [Display(Name = "Tên loại bạn đọc")]
        public string TenLoai { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        // Navigation property
        public virtual ICollection<Models.NguoiDung> NguoiDungs { get; set; }
    }
}