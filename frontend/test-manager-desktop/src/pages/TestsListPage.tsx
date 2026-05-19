import { Link } from "react-router-dom";

export function TestsListPage() {
  return (
    <section className="page">
      <header className="page-header">
        <div>
          <h1>Tests</h1>
          <p>Test catalog</p>
        </div>
        <Link className="button primary" to="/tests/create">
          New test
        </Link>
      </header>
      <div className="placeholder">No tests loaded.</div>
    </section>
  );
}
