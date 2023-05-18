using Microsoft.AspNetCore.Mvc;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Areas.Admin.Models { 
public  class PromotionViewModel
{
    
    public int PromotionId { get; set; }
    public int LevelId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double Discount { get; set; }

}
}