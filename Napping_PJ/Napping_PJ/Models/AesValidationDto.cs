namespace Napping_PJ.Models
{
    public class AesValidationDto
    {        public AesValidationDto(string Email, DateTime ExpiredDate)
        {
            this.Email = Email;
            this.ExpiredDate = ExpiredDate;
        }
        public string Email { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
