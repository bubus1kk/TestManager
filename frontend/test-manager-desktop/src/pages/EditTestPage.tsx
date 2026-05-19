import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { testsApi } from "../api/testsApi";
import { TestForm } from "../components/tests/TestForm";
import { StateBlock } from "../components/ui/StateBlock";
import type { CreateTestRequest, TestDetailsDto } from "../types/tests";

export function EditTestPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const testId = Number(id);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [test, setTest] = useState<TestDetailsDto | null>(null);

  useEffect(() => {
    const loadTest = async () => {
      setError(null);
      setIsLoading(true);

      try {
        setTest(await testsApi.getById(testId));
      } catch (requestError) {
        setError(requestError instanceof Error ? requestError.message : "Failed to load test.");
      } finally {
        setIsLoading(false);
      }
    };

    void loadTest();
  }, [testId]);

  const updateTest = async (request: CreateTestRequest) => {
    setError(null);
    setIsSaving(true);

    try {
      const updatedTest = await testsApi.update(testId, request);
      navigate(`/tests/${updatedTest.id}`);
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Failed to update test.");
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return <StateBlock message="Loading the current test content." title="Opening editor" />;
  }

  if (!test) {
    return <StateBlock message={error ?? "Test was not found."} title="Could not edit test" tone="error" />;
  }

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Edit assessment</span>
          <h1>Edit test</h1>
          <p>Refine the title, questions, and answer options.</p>
        </div>
      </header>

      {error ? <StateBlock message={error} title="Could not save changes" tone="error" /> : null}

      <TestForm
        initialValue={test}
        isSaving={isSaving}
        onCancel={() => navigate(`/tests/${test.id}`)}
        onSubmit={updateTest}
        submitLabel="Save changes"
      />
    </section>
  );
}
