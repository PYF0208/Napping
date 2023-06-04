using Napping_PJ.Enums;
using Napping_PJ.Models.Entity;

namespace Napping_PJ.Services
{
	public class ChangePaymentStatusService : IChangePaymentStatusService
	{
		private readonly db_a989f8_nappingContext db;

		public ChangePaymentStatusService(db_a989f8_nappingContext db)
		{
			this.db = db;
		}
		public void CheckPaymentStatus(int orderId)
		{
			var result = db.Orders.FirstOrDefault(x => x.OrderId == orderId);
			if (result == null) throw new Exception("查無ID");
			if (result.Status == (int)PaymentStatusEnum.NotPay)
			{
				ChangePaymentStatus(orderId, PaymentStatusEnum.Cancel);
			}
		}
		public void ChangePaymentStatus(int orderId, PaymentStatusEnum status)
		{
			var result = db.Orders.FirstOrDefault(x => x.OrderId == orderId);
			if (result == null) throw new Exception("查無ID");
			result.Status = (int)status;

			db.Payments.Add(new Payment
			{
				OrderId = orderId,
				Date = DateTime.Now,
				Status = (int)PaymentStatusEnum.Cancel,
				Type = "信用卡"
			});
			db.SaveChanges();
		}
	}
}
