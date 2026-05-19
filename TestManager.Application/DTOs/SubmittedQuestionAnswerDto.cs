namespace TestManager.Application.DTOs;

public class SubmittedQuestionAnswerDto
{
    public int QuestionId { get; set; }

    public ICollection<int> SelectedAnswerOptionIds { get; set; } = new List<int>();
}
