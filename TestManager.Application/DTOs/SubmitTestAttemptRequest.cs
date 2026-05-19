namespace TestManager.Application.DTOs;

public class SubmitTestAttemptRequest
{
    public ICollection<SubmittedQuestionAnswerDto> Answers { get; set; } = new List<SubmittedQuestionAnswerDto>();
}
