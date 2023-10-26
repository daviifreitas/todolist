using Activity.API.Context;
using Activity.API.Interfaces;

namespace Activity.API.Repository
{
    public class ActivityTicketTicketRepository : RepositoryBase<Entities.ActivityTicket>, IActivityTicketRepository
    {
        public ActivityTicketTicketRepository(TaskDbContext context) : base(context)
        {
        }
    }
}
