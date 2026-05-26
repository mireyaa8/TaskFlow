namespace TaskFlow.Data.Models;

public class Label
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = "secondary";

    public ICollection<TaskLabel> TaskLabels { get; set; } = new HashSet<TaskLabel>();
}
