using TestManager.Domain.Entities;

namespace TestManager.Application.DTOs;

public class QuestionDto
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public QuestionType Type { get; set; }

    public ICollection<AnswerOptionDto> AnswerOptions { get; set; } = new List<AnswerOptionDto>();
}
