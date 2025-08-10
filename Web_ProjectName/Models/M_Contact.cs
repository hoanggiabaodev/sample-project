using Web_ProjectName.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Web_ProjectName.Models
{
    public class M_Contact : M_BaseModel.BaseCustom
    {
        // New schema (PascalCase)
        public int? Id { get; set; }
        public int? SchoolId { get; set; }
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Remark { get; set; }
        public int? Status { get; set; }

        // Backward-compatible aliases (camelCase/legacy names)
        public int? id { get => Id; set => Id = value; }
        public int? supplierId { get => SchoolId; set => SchoolId = value; }
        public int? productId { get => ProductId; set => ProductId = value; }
        public string name { get => Name; set => Name = value; }
        public string email { get => Email; set => Email = value; }
        public string phone { get => Phone; set => Phone = value; }
        public string title { get => Title; set => Title = value; }
        public string detail { get => Detail; set => Detail = value; }
        public string remark { get => Remark; set => Remark = value; }
        public int? status { get => Status; set => Status = value; }
    }

    public class EM_Contact : M_BaseModel.BaseCustom
    {
        // New schema (PascalCase)
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
        public int? Status { get; set; }

        // Backward-compatible aliases (camelCase/legacy names)
        public int? id { get => Id; set => Id = value; }
        public int? supplierId { get => SchoolId; set => SchoolId = value; }
        public int? productId { get => ProductId; set => ProductId = value; }
        public string name { get => Name; set => Name = value; }
        public string email { get => Email; set => Email = value; }
        public string phone { get => Phone; set => Phone = value; }
        public string title { get => Title; set => Title = value; }
        public string detail { get => Detail; set => Detail = value; }
        public string remark { get => Remark; set => Remark = value; }
        public int? status { get => Status; set => Status = value; }
    }
}
