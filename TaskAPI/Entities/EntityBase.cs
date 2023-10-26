namespace Activity.API.Entities;

public abstract class EntityBase 
{
    public int Id { get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public StatusDefault StatusDefault { get; set; }

}