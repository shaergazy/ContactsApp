namespace ContactsApp.Models
{
    public class ShipmentModel
    {
        public int Id { get; set; }
        public string ShipmentId { get; set; } = "";
        public string TrackingNumber { get; set; } = "";
        public string FromAddress { get; set; } = "";   
        public string ToAddress { get; set; } = "";
        public string Service { get; set; } = "";
        public string Carrier { get; set; } = "";
        public int ParcelId { get; set; }
        public ParcelModel Parcel { get; set; }
    }
}
