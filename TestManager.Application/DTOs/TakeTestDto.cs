namespace TestManager.Application.DTOs;

public class TakeTestDto
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public ICollection<TakeQuestionDto> Questions { get; set; } = new List<TakeQuestionDto>();
}
