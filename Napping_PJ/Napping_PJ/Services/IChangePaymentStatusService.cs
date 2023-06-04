using Napping_PJ.Enums;

namespace Napping_PJ.Services
{
	public interface IChangePaymentStatusService
	{
		public void ChangePaymentStatus(int orderId, PaymentStatusEnum status);
	}
}
