namespace TestManager.Domain.Entities;

public class Test
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
