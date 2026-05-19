interface StateBlockProps {
  message: string;
  title: string;
  tone?: "loading" | "error";
}

export function StateBlock({ message, title, tone = "loading" }: StateBlockProps) {
  return (
    <div className={`state-block state-${tone}`}>
      <div className="state-pulse" />
      <div>
        <h2>{title}</h2>
        <p>{message}</p>
      </div>
    </div>
  );
}
