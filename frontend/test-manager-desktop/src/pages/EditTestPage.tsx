import { useParams } from "react-router-dom";

export function EditTestPage() {
  const { id } = useParams();

  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>Edit test</h1>
          <p>Test #{id}</p>
        </div>
      </header>
      <div className="placeholder">Edit form placeholder.</div>
    </section>
  );
}
