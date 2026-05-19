import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { testsApi } from "../api/testsApi";
import { Badge } from "../components/ui/Badge";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { Section } from "../components/ui/Section";
import { StateBlock } from "../components/ui/StateBlock";
import { QuestionType, type TestDetailsDto } from "../types/tests";

function getQuestionTypeLabel(type: QuestionType) {
  return type === QuestionType.SingleChoice ? "Один ответ" : "Несколько ответов";
}

export function TestDetailsPage() {
  const { id } = useParams();
  const testId = Number(id);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [test, setTest] = useState<TestDetailsDto | null>(null);

  useEffect(() => {
    const loadTest = async () => {
      setError(null);
      setIsLoading(true);

      try {
        setTest(await testsApi.getById(testId));
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Не удалось загрузить тест.");
      } finally {
        setIsLoading(false);
      }
    };

    void loadTest();
  }, [testId]);

  if (isLoading) {
    return <StateBlock message="Подготавливаем полную структуру теста." title="Загрузка теста" />;
  }

  if (error || !test) {
    return <StateBlock message={error ?? "Тест не найден."} title="Не удалось открыть тест" tone="error" />;
  }

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Детали теста</span>
          <h1>{test.title}</h1>
          <p>{test.description || "Описание пока не добавлено. Структура вопросов показана ниже."}</p>
        </div>
        <div className="actions">
          <Button to={`/tests/${test.id}/edit`} variant="secondary">
            Изменить
          </Button>
          <Button to={`/tests/${test.id}/take`} variant="primary">
            Пройти тест
          </Button>
        </div>
      </header>

      <Section
        description={`Настроено вопросов: ${test.questions.length}`}
        title="Структура вопросов"
      >
        <div className="details-list">
          {test.questions.map((question, questionIndex) => (
            <Card className="details-question-card" key={question.id}>
              <div className="details-question-header">
                <div>
                  <Badge tone="peach">Вопрос {questionIndex + 1}</Badge>
                  <h2>{question.text}</h2>
                </div>
                <Badge tone={question.type === QuestionType.SingleChoice ? "sage" : "terra"}>
                  {getQuestionTypeLabel(question.type)}
                </Badge>
              </div>

              <div className="details-options">
                {question.answerOptions.map((answerOption, answerIndex) => (
                  <div className="details-option" key={answerOption.id}>
                    <span className="option-index">{answerIndex + 1}</span>
                    <span>{answerOption.text}</span>
                    {answerOption.isCorrect ? <Badge tone="sage">Правильный</Badge> : null}
                  </div>
                ))}
              </div>
            </Card>
          ))}
        </div>
      </Section>
    </section>
  );
}
