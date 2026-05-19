import type { ButtonHTMLAttributes, ReactNode } from "react";
import { Link } from "react-router-dom";

type ButtonVariant = "primary" | "secondary" | "ghost" | "danger";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
  className?: string;
  to?: string;
  variant?: ButtonVariant;
}

export function Button({
  children,
  className = "",
  to,
  variant = "secondary",
  type = "button",
  ...props
}: ButtonProps) {
  const buttonClassName = ["ui-button", `ui-button-${variant}`, className]
    .filter(Boolean)
    .join(" ");

  if (to) {
    return (
      <Link className={buttonClassName} to={to}>
        {children}
      </Link>
    );
  }

  return (
    <button className={buttonClassName} type={type} {...props}>
      {children}
    </button>
  );
}
