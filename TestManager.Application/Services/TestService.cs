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
                QuestionsCount = test.Questions.Count
            })
            .ToList();
    }

    public async Task<TestDetailsDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var test = await testRepository.GetByIdAsync(id, cancellationToken);

        return test is null ? null : MapToDetailsDto(test);
    }

    public async Task<TestDetailsDto> CreateAsync(CreateTestRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Title, request.Questions);

        var test = new Test
        {
            Title = request.Title.Trim(),
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
}
