using ContactsApp.Models;
using ContactsApp.Services.Interfaces;

namespace ContactsApp.Services
{
    public class ShipmentService : GenericService<ShipmentModel, int>, IShipmentService
    {
        public ShipmentService(ContactsDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
