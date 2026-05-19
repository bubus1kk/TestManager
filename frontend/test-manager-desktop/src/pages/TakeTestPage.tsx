import { useParams } from "react-router-dom";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";

export function TakeTestPage() {
  const { id } = useParams();

  return (
    <section className="page">
      <header className="page-header hero-header">
        <div>
          <span className="eyebrow">Attempt mode</span>
          <h1>Take test</h1>
          <p>Test #{id} is ready for the learner flow once the attempt UI is connected.</p>
        </div>
        <Button to={`/tests/${id}`} variant="secondary">
          Back to details
        </Button>
      </header>

      <Card>
        <div className="soft-message">
          The CRUD workspace is ready. The dedicated test-taking interface can build on the submit API next.
        </div>
      </Card>
    </section>
  );
}
