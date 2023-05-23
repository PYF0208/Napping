using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Areas.Admin.Models
{
    public class CustomersViewModel
    {
        public int customerId { get; set; }
        [Required(ErrorMessage = "請輸入姓名")]
        public string? name { get; set; }
        [Required(ErrorMessage = "birthday為必填欄位")]
        public DateTime? birthday { get; set; }
        [Required(ErrorMessage = "請輸入電話")]
        public string? phone { get; set; }
        [Required(ErrorMessage = "請選擇性別")]
        public bool? gender { get; set; }
        [Required(ErrorMessage = "請選擇城市")]
        public string? city { get; set; }
        [Required(ErrorMessage = "請選擇地區")]
        public string? region { get; set; }
        [Required(ErrorMessage = "請輸入地址")]
        public string? country { get; set; }
        [Required(ErrorMessage = "email為必填欄位")]
        [EmailAddress(ErrorMessage = "須符合信箱格式")]
        public string? email { get; set; }
        [Required(ErrorMessage = "請賦予會員等級")]
        public int? levelId { get; set; }
        [Required(ErrorMessage = "請設定鎖定狀態")]
        public bool? locked { get; set; }
    }
}
