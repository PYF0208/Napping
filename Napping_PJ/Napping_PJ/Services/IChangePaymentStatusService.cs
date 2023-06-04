using Napping_PJ.Enums;

namespace Napping_PJ.Services
{
	public interface IChangePaymentStatusService
	{
		public void CheckPaymentStatus(int orderId);
		public void ChangePaymentStatus(int orderId, PaymentStatusEnum status);
	}
}
