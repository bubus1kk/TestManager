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
      setError(requestError instanceof Error ? requestError.message : "Не удалось создать тест.");
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Новая проверка</span>
          <h1>Создать тест</h1>
        </div>
      </header>

      {error ? <StateBlock message={error} title="Не удалось сохранить тест" tone="error" /> : null}

      <TestForm
        isSaving={isSaving}
        onCancel={() => navigate("/")}
        onSubmit={createTest}
        submitLabel="Создать тест"
      />
    </section>
  );
}
