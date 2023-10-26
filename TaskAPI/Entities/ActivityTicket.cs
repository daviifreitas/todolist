namespace Activity.API.Entities;

public class ActivityTicket : EntityBase
{
    public String Title { get; set; }
    public string Description { get; set; }
    public ActivityStatus Status { get; set; }
    public string Requester { get; set; }
    public string Assigned { get; set; }
    public DateTime DueDate { get; set; }
        
}