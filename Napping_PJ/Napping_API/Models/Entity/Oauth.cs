using System;
using System.Collections.Generic;

namespace Napping_API.Models.Entity
{
    public partial class Oauth
    {
        public string OauthId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; } = null!;
    }
}
