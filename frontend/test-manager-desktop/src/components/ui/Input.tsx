import type { InputHTMLAttributes } from "react";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  error?: string;
  label: string;
}

export function Input({ error, id, label, ...props }: InputProps) {
  const inputId = id ?? props.name;

  return (
    <label className="field" htmlFor={inputId}>
      <span className="field-label">{label}</span>
      <input className={error ? "input input-error" : "input"} id={inputId} {...props} />
      {error ? <span className="field-error">{error}</span> : null}
    </label>
  );
}
