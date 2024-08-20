namespace ContactsApp.Models
{
    public class AddressModel
    {
        public int Id { get; set; }
        public string AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
