namespace TestManager.Domain.Entities;

public class Question
{
    public int Id { get; set; }

    public required string Text { get; set; }

    public QuestionType Type { get; set; }

    public int TestId { get; set; }

    public Test? Test { get; set; }

    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
}
