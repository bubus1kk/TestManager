namespace TestManager.Application.DTOs;

public class TestListItemDto
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public int QuestionsCount { get; set; }
}
