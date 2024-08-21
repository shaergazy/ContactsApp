using ContactsApp.Models;
using ContactsApp.Services.Interfaces;

namespace ContactsApp.Services
{
    public class ParcelService : GenericService<ParcelModel, int>, IParcelService
    {
        public ParcelService(ContactsDbContext dbContext) : base(dbContext)
        {
            
        }
    }
}
