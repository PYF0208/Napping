using MessagePack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Napping_PJ.Models
{
	public class ResultViewModel
	{
		[System.ComponentModel.DataAnnotations.Key]
		public int HotelId { get; set; }
		public string Name { get; set; } = null!;
		public string Image { get; set; } = null!;
		public string City { get; set; } = null!;
		public string Region { get; set; } = null!;

		public bool IsLike { get; set; }

    }
}
