import { FormEvent, useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { testsApi } from "../api/testsApi";
import { Badge } from "../components/ui/Badge";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { StateBlock } from "../components/ui/StateBlock";
import {
  QuestionType,
  type SubmitTestAttemptRequest,
  type TakeTestDto,
  type TestAttemptResultDto,
} from "../types/tests";

type SelectedAnswers = Record<number, number[]>;

function getQuestionTypeLabel(type: QuestionType) {
  return type === QuestionType.SingleChoice ? "Один ответ" : "Несколько ответов";
}

function formatScore(value: number) {
  return value.toFixed(2).replace(/\.?0+$/, "");
}

function formatPercentage(value: number) {
  return value.toFixed(1).replace(/\.0$/, "");
}

function buildInitialAnswers(test: TakeTestDto): SelectedAnswers {
  return Object.fromEntries(test.questions.map((question) => [question.id, []]));
}

function buildSubmitRequest(test: TakeTestDto, selectedAnswers: SelectedAnswers): SubmitTestAttemptRequest {
  return {
    answers: test.questions.map((question) => ({
      questionId: question.id,
      selectedAnswerOptionIds: selectedAnswers[question.id] ?? [],
    })),
  };
}

export function TakeTestPage() {
  const { id } = useParams();
  const testId = Number(id);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [result, setResult] = useState<TestAttemptResultDto | null>(null);
  const [selectedAnswers, setSelectedAnswers] = useState<SelectedAnswers>({});
  const [test, setTest] = useState<TakeTestDto | null>(null);
  const isAttemptLocked = result !== null;

  useEffect(() => {
    const loadTest = async () => {
      setError(null);
      setIsLoading(true);

      try {
        const loadedTest = await testsApi.getForTaking(testId);
        setTest(loadedTest);
        setSelectedAnswers(buildInitialAnswers(loadedTest));
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Не удалось загрузить тест.");
      } finally {
        setIsLoading(false);
      }
    };

    void loadTest();
  }, [testId]);

  const answeredCount = useMemo(() => {
    if (!test) {
      return 0;
    }

    return test.questions.filter((question) => (selectedAnswers[question.id]?.length ?? 0) > 0).length;
  }, [selectedAnswers, test]);

  const setSingleChoiceAnswer = (questionId: number, answerOptionId: number) => {
    if (isAttemptLocked) {
      return;
    }

    setSelectedAnswers((current) => ({
      ...current,
      [questionId]: [answerOptionId],
    }));
  };

  const toggleMultipleChoiceAnswer = (questionId: number, answerOptionId: number, checked: boolean) => {
    if (isAttemptLocked) {
      return;
    }

    setSelectedAnswers((current) => {
      const currentQuestionAnswers = current[questionId] ?? [];

      return {
        ...current,
        [questionId]: checked
          ? [...currentQuestionAnswers, answerOptionId]
          : currentQuestionAnswers.filter((id) => id !== answerOptionId),
      };
    });
  };

  const submitAttempt = async (event: FormEvent) => {
    event.preventDefault();

    if (!test) {
      return;
    }

    if (isAttemptLocked) {
      return;
    }

    setError(null);
    setIsSubmitting(true);

    try {
      setResult(await testsApi.submit(test.id, buildSubmitRequest(test, selectedAnswers)));
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Не удалось отправить ответы.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return <StateBlock message="Загружаем вопросы и варианты ответов." title="Подготовка теста" />;
  }

  if (!test) {
    return <StateBlock message={error ?? "Тест не найден."} title="Не удалось открыть тест" tone="error" />;
  }

  return (
    <section className="page take-page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Режим прохождения</span>
          <h1>{test.title}</h1>
          <p>{test.description || "Выберите ответы на вопросы и отправьте попытку для подсчёта результата."}</p>
        </div>
        <Button to={`/tests/${test.id}`} variant="secondary">
          Назад к деталям
        </Button>
      </header>

      {error ? <StateBlock message={error} title="Ошибка прохождения" tone="error" /> : null}

      {result ? (
        <Card className="result-card">
          <span className="eyebrow">Результат</span>
          <div className="result-score">
            {formatScore(result.score)} / {result.maxScore}
          </div>
          <div className="result-percent">{formatPercentage(result.percentage)}%</div>
        </Card>
      ) : null}

      <form className={isAttemptLocked ? "attempt-form attempt-locked" : "attempt-form"} onSubmit={submitAttempt}>
        <div className="attempt-summary">
          <Badge tone="terra">
            Отвечено: {answeredCount} / {test.questions.length}
          </Badge>
        </div>

        <div className="details-list">
          {test.questions.map((question, questionIndex) => (
            <Card className="attempt-question-card" key={question.id}>
              <div className="attempt-question-header">
                <div className="attempt-question-title">
                  <div className="attempt-question-number">{questionIndex + 1}</div>
                  <h2>{question.text}</h2>
                </div>
                <Badge tone={question.type === QuestionType.SingleChoice ? "sage" : "terra"}>
                  {getQuestionTypeLabel(question.type)}
                </Badge>
              </div>

              <div className="attempt-options">
                {question.answerOptions.map((answerOption) => {
                  const selectedQuestionAnswers = selectedAnswers[question.id] ?? [];
                  const isChecked = selectedQuestionAnswers.includes(answerOption.id);

                  return (
                    <label className="attempt-option" key={answerOption.id}>
                      <input
                        checked={isChecked}
                        disabled={isAttemptLocked}
                        name={`question-${question.id}`}
                        onChange={(event) =>
                          question.type === QuestionType.SingleChoice
                            ? setSingleChoiceAnswer(question.id, answerOption.id)
                            : toggleMultipleChoiceAnswer(question.id, answerOption.id, event.target.checked)
                        }
                        type={question.type === QuestionType.SingleChoice ? "radio" : "checkbox"}
                      />
                      <span className="attempt-control" aria-hidden="true" />
                      <span>{answerOption.text}</span>
                    </label>
                  );
                })}
              </div>
            </Card>
          ))}
        </div>

        <div className="form-footer">
          <Button to={isAttemptLocked ? "/" : `/tests/${test.id}`} variant="ghost">
            {isAttemptLocked ? "На главную" : "Отмена"}
          </Button>
          <Button disabled={isSubmitting || isAttemptLocked} type="submit" variant="primary">
            {isAttemptLocked ? "Ответы отправлены" : isSubmitting ? "Отправка..." : "Отправить ответы"}
          </Button>
        </div>
      </form>
    </section>
  );
}
