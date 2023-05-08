using System;
using System.Collections.Generic;

namespace Napping_PJ.Models.Entity
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
            UserRoles = new HashSet<UserRole>();
        }

        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public decimal? Phone { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public string Email { get; set; } = null!;
        public int? LevelId { get; set; }

        public virtual Level? Level { get; set; }
        public virtual ICollection<BellEvent> BellEvents { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CustomerGift> CustomerGifts { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Oauth> Oauths { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
