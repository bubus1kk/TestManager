using TestManager.Domain.Entities;

namespace TestManager.Application.DTOs;

public class TakeQuestionDto
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public QuestionType Type { get; set; }

    public ICollection<TakeAnswerOptionDto> AnswerOptions { get; set; } = new List<TakeAnswerOptionDto>();
}
