import { useParams } from "react-router-dom";

export function TakeTestPage() {
  const { id } = useParams();

  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>Take test</h1>
          <p>Test #{id}</p>
        </div>
      </header>
      <div className="placeholder">Attempt placeholder.</div>
    </section>
  );
}
