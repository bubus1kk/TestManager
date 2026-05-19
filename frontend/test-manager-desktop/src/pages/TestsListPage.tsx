import { useEffect, useState } from "react";
import { testsApi } from "../api/testsApi";
import { Badge } from "../components/ui/Badge";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { EmptyState } from "../components/ui/EmptyState";
import { StateBlock } from "../components/ui/StateBlock";
import type { TestListItemDto } from "../types/tests";

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
      setError(requestError instanceof Error ? requestError.message : "Failed to load tests.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadTests();
  }, []);

  const deleteTest = async (test: TestListItemDto) => {
    const confirmed = window.confirm(`Delete "${test.title}"? This action cannot be undone.`);

    if (!confirmed) {
      return;
    }

    try {
      await testsApi.delete(test.id);
      setTests((current) => current.filter((item) => item.id !== test.id));
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Failed to delete test.");
    }
  };

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Assessment workspace</span>
          <h1>Tests</h1>
          <p>Design, review, and launch structured knowledge checks from one calm workspace.</p>
        </div>
        <Button to="/tests/create" variant="primary">
          New test
        </Button>
      </header>

      {isLoading ? (
        <StateBlock message="Fetching the latest test catalog." title="Loading tests" />
      ) : null}

      {!isLoading && error ? (
        <StateBlock message={error} title="Could not load tests" tone="error" />
      ) : null}

      {!isLoading && !error && tests.length === 0 ? (
        <EmptyState
          action={
            <Button to="/tests/create" variant="primary">
              Create first test
            </Button>
          }
          description="Start with a title, then add questions and answer options when you are ready."
          title="No tests yet"
        />
      ) : null}

      {!isLoading && !error && tests.length > 0 ? (
        <div className="test-grid">
          {tests.map((test) => (
            <Card className="test-card" key={test.id}>
              <div className="test-card-top">
                <Badge tone="terra">{test.questionsCount} questions</Badge>
              </div>
              <h2>{test.title}</h2>
              <p>{test.description || "No description yet. Add one when editing this test."}</p>
              <div className="test-card-actions">
                <Button to={`/tests/${test.id}`} variant="secondary">
                  View
                </Button>
                <Button to={`/tests/${test.id}/edit`} variant="ghost">
                  Edit
                </Button>
                <Button to={`/tests/${test.id}/take`} variant="primary">
                  Take Test
                </Button>
                <Button onClick={() => void deleteTest(test)} variant="danger">
                  Delete
                </Button>
              </div>
            </Card>
          ))}
        </div>
      ) : null}
    </section>
  );
}
