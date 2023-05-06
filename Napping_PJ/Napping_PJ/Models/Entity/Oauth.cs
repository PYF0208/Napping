using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
{
    public partial class Oauth
    {
        public string OauthId { get; set; } = null!;
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}
