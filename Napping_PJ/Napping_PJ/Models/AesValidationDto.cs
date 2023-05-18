namespace Napping_PJ.Models
{
    public class AesValidationDto
    {        public AesValidationDto(string Account, DateTime ExpiredDate)
        {
            this.Account = Account;
            this.ExpiredDate = ExpiredDate;
        }
        public string Account { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
