namespace ContactsApp.Models
{
    public class ParcelModel
    {
        public int Id { get; set; }
        public string ParcelId { get; set; } = "";
        public string Name { get; set; } = "";
        public double? Weight { get; set; }
        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
    }
}
