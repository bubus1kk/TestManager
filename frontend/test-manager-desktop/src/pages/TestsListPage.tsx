import { useEffect, useState } from "react";
import { testsApi } from "../api/testsApi";
import { Badge } from "../components/ui/Badge";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { EmptyState } from "../components/ui/EmptyState";
import { StateBlock } from "../components/ui/StateBlock";
import type { TestListItemDto } from "../types/tests";

const DESCRIPTION_PREVIEW_LENGTH = 50;

function getDescriptionPreview(description?: string) {
  const trimmedDescription = description?.trim();

  if (!trimmedDescription) {
    return "Описание отсутствует";
  }

  return trimmedDescription.length > DESCRIPTION_PREVIEW_LENGTH
    ? `${trimmedDescription.slice(0, DESCRIPTION_PREVIEW_LENGTH)}...`
    : trimmedDescription;
}

export function TestsListPage() {
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [tests, setTests] = useState<TestListItemDto[]>([]);

  const loadTests = async () => {
    setError(null);
    setIsLoading(true);

    try {
      setTests(await testsApi.getAll());
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Не удалось загрузить тесты.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadTests();
  }, []);

  const deleteTest = async (test: TestListItemDto) => {
    const confirmed = window.confirm(`Удалить "${test.title}"? Это действие нельзя отменить.`);

    if (!confirmed) {
      return;
    }

    try {
      await testsApi.delete(test.id);
      setTests((current) => current.filter((item) => item.id !== test.id));
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Не удалось удалить тест.");
    }
  };

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Рабочая область</span>
          <h1>Тесты</h1>
        </div>
        <Button to="/tests/create" variant="primary">
          Новый тест
        </Button>
      </header>

      {isLoading ? (
        <StateBlock message="Получаем актуальный список тестов." title="Загрузка тестов" />
      ) : null}

      {!isLoading && error ? (
        <StateBlock message={error} title="Не удалось загрузить тесты" tone="error" />
      ) : null}

      {!isLoading && !error && tests.length === 0 ? (
        <EmptyState
          action={
            <Button to="/tests/create" variant="primary">
              Создать первый тест
            </Button>
          }
          description="Начните с названия, затем добавьте вопросы и варианты ответов."
          title="Тестов пока нет"
        />
      ) : null}

      {!isLoading && !error && tests.length > 0 ? (
        <div className="test-grid">
          {tests.map((test) => (
            <Card className="test-card" key={test.id}>
              <div className="test-card-top">
                <Badge tone="terra">Вопросов: {test.questionsCount}</Badge>
              </div>
              <h2>{test.title}</h2>
              <p>{getDescriptionPreview(test.description)}</p>
              <div className="test-card-actions">
                <Button to={`/tests/${test.id}`} variant="secondary">
                  Открыть
                </Button>
                <Button to={`/tests/${test.id}/edit`} variant="ghost">
                  Изменить
                </Button>
                <Button to={`/tests/${test.id}/take`} variant="primary">
                  Пройти тест
                </Button>
                <Button onClick={() => void deleteTest(test)} variant="danger">
                  Удалить
                </Button>
              </div>
            </Card>
          ))}
        </div>
      ) : null}
    </section>
  );
}
