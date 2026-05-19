namespace TestManager.Application.DTOs;

public class TestAttemptResultDto
{
    public int TestId { get; set; }

    public double Score { get; set; }

    public int MaxScore { get; set; }

    public double Percentage { get; set; }
}
