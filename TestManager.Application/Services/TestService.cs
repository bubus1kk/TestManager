using System.ComponentModel.DataAnnotations;
using TestManager.Application.DTOs;
using TestManager.Application.Repositories;
using TestManager.Domain.Entities;

namespace TestManager.Application.Services;

public class TestService(ITestRepository testRepository) : ITestService
{
    public async Task<IReadOnlyCollection<TestListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tests = await testRepository.GetAllAsync(cancellationToken);

        return tests
            .Select(test => new TestListItemDto
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                QuestionsCount = test.Questions.Count
            })
            .ToList();
    }

    public async Task<TestDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);

        return test is null ? null : MapToDetailsDto(test);
    }

    public async Task<TakeTestDto?> GetForTakingAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);

        return test is null ? null : MapToTakeDto(test);
    }

    public async Task<TestDetailsDto> CreateAsync(CreateTestRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Questions);

        var test = new Test
        {
            Title = request.Title.Trim(),
            Description = NormalizeDescription(request.Description),
            Questions = MapQuestions(request.Questions)
        };

        await testRepository.AddAsync(test, cancellationToken);
        await testRepository.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(test);
    }

    public async Task<TestDetailsDto?> UpdateAsync(int id, UpdateTestRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Questions);

        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
        {
            return null;
        }

        test.Title = request.Title.Trim();
        test.Description = NormalizeDescription(request.Description);
        test.Questions.Clear();

        foreach (var question in MapQuestions(request.Questions))
        {
            test.Questions.Add(question);
        }

        await testRepository.SaveChangesAsync(cancellationToken);

        return MapToDetailsDto(test);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
        {
            return false;
        }

        await testRepository.DeleteAsync(test, cancellationToken);
        await testRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<TestAttemptResultDto?> SubmitAsync(
        int id,
        SubmitTestAttemptRequest request,
        CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);
        if (test is null)
        {
            return null;
        }

        if (request is null)
        {
            throw new ValidationException("Submit request is required.");
        }

        var submittedAnswers = BuildSubmittedAnswersLookup(test, request);
        var score = test.Questions.Sum(question => CalculateQuestionScore(question, submittedAnswers));
        var maxScore = test.Questions.Count;

        return new TestAttemptResultDto
        {
            TestId = test.Id,
            Score = score,
            MaxScore = maxScore,
            Percentage = maxScore == 0 ? 0 : score / maxScore * 100
        };
    }

    private static void Validate(string? title, ICollection<QuestionDto>? questions)
    {
        var trimmedTitle = title?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedTitle))
        {
            throw new ValidationException("Title is required.");
        }

        if (trimmedTitle.Length > 200)
        {
            throw new ValidationException("Title must be 200 characters or fewer.");
        }

        if (questions is null || questions.Count == 0)
        {
            throw new ValidationException("Test must contain at least one question.");
        }

        foreach (var question in questions)
        {
            if (question is null)
            {
                throw new ValidationException("Question is required.");
            }

            ValidateQuestion(question);
        }
    }

    private static void ValidateQuestion(QuestionDto question)
    {
        if (string.IsNullOrWhiteSpace(question.Text))
        {
            throw new ValidationException("Question text is required.");
        }

        if (question.AnswerOptions is null || question.AnswerOptions.Count < 2)
        {
            throw new ValidationException("Question must contain at least two answer options.");
        }

        foreach (var answerOption in question.AnswerOptions)
        {
            if (answerOption is null)
            {
                throw new ValidationException("Answer option is required.");
            }

            if (string.IsNullOrWhiteSpace(answerOption.Text))
            {
                throw new ValidationException("Answer option text is required.");
            }
        }

        var correctAnswersCount = question.AnswerOptions.Count(answerOption => answerOption.IsCorrect);

        switch (question.Type)
        {
            case QuestionType.SingleChoice when correctAnswersCount != 1:
                throw new ValidationException("Single choice question must contain exactly one correct answer option.");
            case QuestionType.MultipleChoice when correctAnswersCount < 1:
                throw new ValidationException("Multiple choice question must contain at least one correct answer option.");
            case QuestionType.SingleChoice:
            case QuestionType.MultipleChoice:
                return;
            default:
                throw new ValidationException("Question type is invalid.");
        }
    }

    private static Dictionary<int, HashSet<int>> BuildSubmittedAnswersLookup(
        Test test,
        SubmitTestAttemptRequest request)
    {
        if (request.Answers is null)
        {
            throw new ValidationException("Answers are required.");
        }

        var questionIds = test.Questions.Select(question => question.Id).ToHashSet();
        var answersByQuestionId = new Dictionary<int, HashSet<int>>();

        foreach (var answer in request.Answers)
        {
            if (answer is null)
            {
                throw new ValidationException("Submitted answer is required.");
            }

            if (!questionIds.Contains(answer.QuestionId))
            {
                throw new ValidationException("Submitted question does not belong to the test.");
            }

            var question = test.Questions.First(question => question.Id == answer.QuestionId);
            var answerOptionIds = question.AnswerOptions
                .Select(answerOption => answerOption.Id)
                .ToHashSet();
            var selectedAnswerOptionIds = answer.SelectedAnswerOptionIds?.ToHashSet() ?? new HashSet<int>();

            if (selectedAnswerOptionIds.Any(selectedId => !answerOptionIds.Contains(selectedId)))
            {
                throw new ValidationException("Selected answer option does not belong to the question.");
            }

            if (answersByQuestionId.ContainsKey(answer.QuestionId))
            {
                throw new ValidationException("Question answer was submitted more than once.");
            }

            answersByQuestionId[answer.QuestionId] = selectedAnswerOptionIds;
        }

        return answersByQuestionId;
    }

    private static double CalculateQuestionScore(
        Question question,
        IReadOnlyDictionary<int, HashSet<int>> submittedAnswers)
    {
        submittedAnswers.TryGetValue(question.Id, out var selectedAnswerOptionIds);
        selectedAnswerOptionIds ??= new HashSet<int>();

        var correctAnswerOptionIds = question.AnswerOptions
            .Where(answerOption => answerOption.IsCorrect)
            .Select(answerOption => answerOption.Id)
            .ToHashSet();

        return question.Type switch
        {
            QuestionType.SingleChoice => CalculateSingleChoiceScore(selectedAnswerOptionIds, correctAnswerOptionIds),
            QuestionType.MultipleChoice => CalculateMultipleChoiceScore(selectedAnswerOptionIds, correctAnswerOptionIds),
            _ => 0
        };
    }

    private static double CalculateSingleChoiceScore(
        IReadOnlySet<int> selectedAnswerOptionIds,
        IReadOnlySet<int> correctAnswerOptionIds)
    {
        return selectedAnswerOptionIds.Count == 1
            && correctAnswerOptionIds.Count == 1
            && correctAnswerOptionIds.Contains(selectedAnswerOptionIds.Single())
            ? 1
            : 0;
    }

    private static double CalculateMultipleChoiceScore(
        IReadOnlySet<int> selectedAnswerOptionIds,
        IReadOnlySet<int> correctAnswerOptionIds)
    {
        if (correctAnswerOptionIds.Count == 0)
        {
            return 0;
        }

        var correctWeight = 1d / correctAnswerOptionIds.Count;
        var correctSelected = selectedAnswerOptionIds.Count(correctAnswerOptionIds.Contains);
        var incorrectSelected = selectedAnswerOptionIds.Count(selectedId => !correctAnswerOptionIds.Contains(selectedId));
        var questionScore = correctSelected * correctWeight - incorrectSelected * correctWeight;

        return Math.Max(0, questionScore);
    }

    private static List<Question> MapQuestions(IEnumerable<QuestionDto> questionDtos)
    {
        return questionDtos
            .Select(questionDto => new Question
            {
                Text = questionDto.Text.Trim(),
                Type = questionDto.Type,
                AnswerOptions = questionDto.AnswerOptions
                    .Select(answerOptionDto => new AnswerOption
                    {
                        Text = answerOptionDto.Text.Trim(),
                        IsCorrect = answerOptionDto.IsCorrect
                    })
                    .ToList()
            })
            .ToList();
    }

    private static TestDetailsDto MapToDetailsDto(Test test)
    {
        return new TestDetailsDto
        {
            Id = test.Id,
            Title = test.Title,
            Description = test.Description,
            Questions = test.Questions
                .Select(question => new QuestionDto
                {
                    Id = question.Id,
                    Text = question.Text,
                    Type = question.Type,
                    AnswerOptions = question.AnswerOptions
                        .Select(answerOption => new AnswerOptionDto
                        {
                            Id = answerOption.Id,
                            Text = answerOption.Text,
                            IsCorrect = answerOption.IsCorrect
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private static TakeTestDto MapToTakeDto(Test test)
    {
        return new TakeTestDto
        {
            Id = test.Id,
            Title = test.Title,
            Description = test.Description,
            Questions = test.Questions
                .Select(question => new TakeQuestionDto
                {
                    Id = question.Id,
                    Text = question.Text,
                    Type = question.Type,
                    AnswerOptions = question.AnswerOptions
                        .Select(answerOption => new TakeAnswerOptionDto
                        {
                            Id = answerOption.Id,
                            Text = answerOption.Text
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private static string? NormalizeDescription(string? description)
    {
        var trimmedDescription = description?.Trim();

        return string.IsNullOrWhiteSpace(trimmedDescription) ? null : trimmedDescription;
    }
}
