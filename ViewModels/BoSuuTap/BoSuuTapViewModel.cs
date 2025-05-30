using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using libraryproject.Models;

namespace libraryproject.ViewModels.BoSuuTap
{
    public class BoSuuTapViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bộ sưu tập")]
        [StringLength(100, ErrorMessage = "Tên bộ sưu tập không được vượt quá 100 ký tự")]
        [Display(Name = "Tên bộ sưu tập")]
        public string TenBoSuuTap { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Số lượng tài liệu")]
        public int SoLuongTaiLieu { get; set; }

        // Method to map from entity to view model
        public static BoSuuTapViewModel FromEntity(Models.BoSuuTap entity)
        {
            return new BoSuuTapViewModel
            {
                ID = entity.ID,
                TenBoSuuTap = entity.TenBoSuuTap,
                MoTa = entity.MoTa,
                SoLuongTaiLieu = entity.TaiLieus?.Count ?? 0
            };
        }

        // Method to map from view model to entity
        public Models.BoSuuTap ToEntity()
        {
            return new Models.BoSuuTap
            {
                ID = this.ID,
                TenBoSuuTap = this.TenBoSuuTap,
                MoTa = this.MoTa ?? ""
            };
        }
    }
}