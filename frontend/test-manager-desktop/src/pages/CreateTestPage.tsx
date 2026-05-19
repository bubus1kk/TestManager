import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { testsApi } from "../api/testsApi";
import { TestForm } from "../components/tests/TestForm";
import { StateBlock } from "../components/ui/StateBlock";
import type { CreateTestRequest } from "../types/tests";

export function CreateTestPage() {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const createTest = async (request: CreateTestRequest) => {
    setError(null);
    setIsSaving(true);

    try {
      const createdTest = await testsApi.create(request);
      navigate(`/tests/${createdTest.id}`);
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "Failed to create test.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">New assessment</span>
          <h1>Create test</h1>
          <p>Build the test structure now; publishing controls can come later.</p>
        </div>
      </header>

      {error ? <StateBlock message={error} title="Could not save test" tone="error" /> : null}

      <TestForm
        isSaving={isSaving}
        onCancel={() => navigate("/")}
        onSubmit={createTest}
        submitLabel="Create test"
      />
    </section>
  );
}
