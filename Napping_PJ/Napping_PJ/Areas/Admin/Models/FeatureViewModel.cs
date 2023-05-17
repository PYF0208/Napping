using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Areas.Admin.Models
{
    public partial class FeatureViewModel
    {

        [Key]
        public int FeatureId { get; set; }

        public string Name { get; set; }
        public string Image { get; set; }


    }
}
