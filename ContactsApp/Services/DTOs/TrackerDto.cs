namespace ContactsApp.Services.DTOs
{
    public class TrackerDto
    {
        public string Carrier { get; set; }
        public string PublicUrl { get; set; }
        public string TrackingCode { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
