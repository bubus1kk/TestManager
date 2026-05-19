namespace TestManager.Application.DTOs;

public class UpdateTestRequest
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
}
