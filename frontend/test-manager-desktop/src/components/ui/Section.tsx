import type { ReactNode } from "react";

interface SectionProps {
  actions?: ReactNode;
  children: ReactNode;
  description?: string;
  title: string;
}

export function Section({ actions, children, description, title }: SectionProps) {
  return (
    <section className="section">
      <div className="section-header">
        <div>
          <h2>{title}</h2>
          {description ? <p>{description}</p> : null}
        </div>
        {actions ? <div className="section-actions">{actions}</div> : null}
      </div>
      {children}
    </section>
  );
}
