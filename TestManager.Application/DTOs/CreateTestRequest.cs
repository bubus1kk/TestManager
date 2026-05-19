namespace TestManager.Application.DTOs;

public class CreateTestRequest
{
    public required string Title { get; set; }

    public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
}
