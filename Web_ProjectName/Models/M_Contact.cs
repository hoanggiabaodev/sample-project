using Web_ProjectName.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Web_ProjectName.Models
{
    public class M_Contact : M_BaseModel.BaseCustom
    {
        public int? Id { get; set; }
        public int? SchoolId { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Remark { get; set; }
    }

    public class EM_Contact : M_BaseModel.BaseCustom
    {
        public int? Id { get; set; }
        public int? SchoolId { get; set; }
        public int? ProductId { get; set; }
        [Required(ErrorMessage = "Tên không được để trống!")]
        [StringLength(50, ErrorMessage = "Tên tối đa 50 ký tự!")]
        public string? Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự!")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Điện thoại không được để trống!")]
        [StringLength(20, ErrorMessage = "Điện thoại tối đa 20 ký tự!")]
        public string? Phone { get; set; }
        [StringLength(100, ErrorMessage = "Tiêu đề tối đa 100 ký tự!")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Nội dung không được để trống!")]
        [StringLength(500, ErrorMessage = "Nội dung tối đa 500 ký tự!")]
        public string? Detail { get; set; }
        [StringLength(150, ErrorMessage = "Ghi chú tối đa 150 ký tự!")]
        public string? Remark { get; set; }
    }
}