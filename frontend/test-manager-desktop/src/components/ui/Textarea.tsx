import type { TextareaHTMLAttributes } from "react";

interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  error?: string;
  label: string;
}

export function Textarea({ error, id, label, ...props }: TextareaProps) {
  const textareaId = id ?? props.name;

  return (
    <label className="field" htmlFor={textareaId}>
      <span className="field-label">{label}</span>
      <textarea
        className={error ? "textarea input-error" : "textarea"}
        id={textareaId}
        {...props}
      />
      {error ? <span className="field-error">{error}</span> : null}
    </label>
  );
}
