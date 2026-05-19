namespace TestManager.Application.DTOs;

public class AnswerOptionDto
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public bool IsCorrect { get; set; }
}
