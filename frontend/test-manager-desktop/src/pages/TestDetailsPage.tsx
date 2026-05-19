import { Link, useParams } from "react-router-dom";

export function TestDetailsPage() {
  const { id } = useParams();

  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>Test details</h1>
          <p>Test #{id}</p>
        </div>
        <div className="actions">
          <Link className="button" to={`/tests/${id}/edit`}>
            Edit
          </Link>
          <Link className="button primary" to={`/tests/${id}/take`}>
            Take
          </Link>
        </div>
      </header>
      <div className="placeholder">Details placeholder.</div>
    </section>
  );
}
