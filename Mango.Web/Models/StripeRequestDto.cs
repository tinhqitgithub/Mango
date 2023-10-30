namespace Mango.Web.Models
{
    public class StripeRequestDto
    {
        public string? StripeSessonId { get; set; }
        public string? StripeSessonUrl { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
