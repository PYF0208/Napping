using System;
using System.Collections.Generic;

namespace Napping_PJ.Models
{
    public partial class Customer
    {
        public Customer()
        {
            BellEvents = new HashSet<BellEvent>();
            Comments = new HashSet<Comment>();
            CustomerGifts = new HashSet<CustomerGift>();
            Likes = new HashSet<Like>();
            Oauths = new HashSet<Oauth>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public string Password { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public decimal Phone { get; set; }
        public string Gender { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Region { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int LevelId { get; set; }

        public virtual Level Level { get; set; } = null!;
        public virtual ICollection<BellEvent> BellEvents { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CustomerGift> CustomerGifts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Oauth> Oauths { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
