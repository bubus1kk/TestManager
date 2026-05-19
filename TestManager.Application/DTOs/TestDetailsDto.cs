namespace TestManager.Application.DTOs;

public class TestDetailsDto
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
}
