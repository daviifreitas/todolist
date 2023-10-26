using Activity.API.Entities;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Activity.API.ApiModel
{
    public class ActivityTicketApiModel
    {
        public String Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public string Requester { get; set; }
        public string Assigned { get; set; }
        public DateTime DueDate { get; set; }

        public static implicit operator ActivityTicket(ActivityTicketApiModel model)
        {
            return new ActivityTicket()
            {
                DueDate = model.DueDate,
                Title = model.Title,
                Description = model.Description,
                Assigned = model.Assigned,
                Status = ActivityStatus.Pending,
                Requester = model.Requester,
                
            };
        }
    }
}
