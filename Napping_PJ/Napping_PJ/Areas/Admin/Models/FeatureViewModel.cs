using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models.Entity
{
    public partial class FeatureViewModel
    {
        public FeatureViewModel()
        {
            Rooms = new HashSet<Room>();
        }

        public int FeatureId { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Image { get; set; } = null!;

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
