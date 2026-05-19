import { FormEvent, useState } from "react";
import { QuestionType, type CreateTestRequest, type TestDetailsDto } from "../../types/tests";
import { Badge } from "../ui/Badge";
import { Button } from "../ui/Button";
import { Card } from "../ui/Card";
import { Input } from "../ui/Input";
import { Textarea } from "../ui/Textarea";

interface TestFormProps {
  initialValue?: TestDetailsDto;
  isSaving?: boolean;
  submitLabel: string;
  onCancel: () => void;
  onSubmit: (request: CreateTestRequest) => Promise<void>;
}

interface AnswerOptionFormValue {
  clientId: string;
  isCorrect: boolean;
  text: string;
}

interface QuestionFormValue {
  answerOptions: AnswerOptionFormValue[];
  clientId: string;
  text: string;
  type: QuestionType;
}

interface TestFormValue {
  description: string;
  questions: QuestionFormValue[];
  title: string;
}

const createClientId = () => Math.random().toString(36).slice(2, 11);

function createAnswerOption(isCorrect = false): AnswerOptionFormValue {
  return {
    clientId: createClientId(),
    isCorrect,
    text: "",
  };
}

function createQuestion(): QuestionFormValue {
  return {
    answerOptions: [createAnswerOption(true), createAnswerOption(false)],
    clientId: createClientId(),
    text: "",
    type: QuestionType.SingleChoice,
  };
}

function toFormValue(initialValue?: TestDetailsDto): TestFormValue {
  if (!initialValue) {
    return {
      description: "",
      questions: [createQuestion()],
      title: "",
    };
  }

  return {
    description: initialValue.description ?? "",
    questions: initialValue.questions.map((question) => ({
      answerOptions: question.answerOptions.map((answerOption) => ({
        clientId: createClientId(),
        isCorrect: answerOption.isCorrect,
        text: answerOption.text,
      })),
      clientId: createClientId(),
      text: question.text,
      type: question.type,
    })),
    title: initialValue.title,
  };
}

function getQuestionTypeLabel(type: QuestionType) {
  return type === QuestionType.SingleChoice ? "Single choice" : "Multiple choice";
}

function validateForm(value: TestFormValue): string[] {
  const errors: string[] = [];

  if (!value.title.trim()) {
    errors.push("Title is required.");
  }

  if (value.title.trim().length > 200) {
    errors.push("Title must be 200 characters or fewer.");
  }

  if (value.questions.length === 0) {
    errors.push("Add at least one question.");
  }

  value.questions.forEach((question, questionIndex) => {
    const questionNumber = questionIndex + 1;

    if (!question.text.trim()) {
      errors.push(`Question ${questionNumber}: text is required.`);
    }

    if (question.answerOptions.length < 2) {
      errors.push(`Question ${questionNumber}: add at least two answer options.`);
    }

    question.answerOptions.forEach((answerOption, answerIndex) => {
      if (!answerOption.text.trim()) {
        errors.push(`Question ${questionNumber}, option ${answerIndex + 1}: text is required.`);
      }
    });

    const correctAnswersCount = question.answerOptions.filter((answerOption) => answerOption.isCorrect).length;

    if (question.type === QuestionType.SingleChoice && correctAnswersCount !== 1) {
      errors.push(`Question ${questionNumber}: choose exactly one correct answer.`);
    }

    if (question.type === QuestionType.MultipleChoice && correctAnswersCount < 1) {
      errors.push(`Question ${questionNumber}: choose at least one correct answer.`);
    }
  });

  return errors;
}

function toRequest(value: TestFormValue): CreateTestRequest {
  return {
    description: value.description.trim() || undefined,
    questions: value.questions.map((question) => ({
      answerOptions: question.answerOptions.map((answerOption) => ({
        isCorrect: answerOption.isCorrect,
        text: answerOption.text.trim(),
      })),
      text: question.text.trim(),
      type: question.type,
    })),
    title: value.title.trim(),
  };
}

export function TestForm({ initialValue, isSaving = false, onCancel, onSubmit, submitLabel }: TestFormProps) {
  const [formValue, setFormValue] = useState<TestFormValue>(() => toFormValue(initialValue));
  const [errors, setErrors] = useState<string[]>([]);

  const updateQuestion = (questionClientId: string, update: Partial<QuestionFormValue>) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) =>
        question.clientId === questionClientId ? { ...question, ...update } : question,
      ),
    }));
  };

  const updateAnswerOption = (
    questionClientId: string,
    answerClientId: string,
    update: Partial<AnswerOptionFormValue>,
  ) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) => {
        if (question.clientId !== questionClientId) {
          return question;
        }

        return {
          ...question,
          answerOptions: question.answerOptions.map((answerOption) =>
            answerOption.clientId === answerClientId ? { ...answerOption, ...update } : answerOption,
          ),
        };
      }),
    }));
  };

  const setQuestionType = (questionClientId: string, type: QuestionType) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) => {
        if (question.clientId !== questionClientId) {
          return question;
        }

        if (type === QuestionType.MultipleChoice) {
          return { ...question, type };
        }

        const firstCorrectAnswer = question.answerOptions.find((answerOption) => answerOption.isCorrect);
        const selectedClientId = firstCorrectAnswer?.clientId ?? question.answerOptions[0]?.clientId;

        return {
          ...question,
          answerOptions: question.answerOptions.map((answerOption) => ({
            ...answerOption,
            isCorrect: answerOption.clientId === selectedClientId,
          })),
          type,
        };
      }),
    }));
  };

  const setCorrectAnswer = (questionClientId: string, answerClientId: string, isCorrect: boolean) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) => {
        if (question.clientId !== questionClientId) {
          return question;
        }

        return {
          ...question,
          answerOptions: question.answerOptions.map((answerOption) => ({
            ...answerOption,
            isCorrect:
              question.type === QuestionType.SingleChoice
                ? answerOption.clientId === answerClientId
                : answerOption.clientId === answerClientId
                  ? isCorrect
                  : answerOption.isCorrect,
          })),
        };
      }),
    }));
  };

  const addQuestion = () => {
    setFormValue((current) => ({
      ...current,
      questions: [...current.questions, createQuestion()],
    }));
  };

  const removeQuestion = (questionClientId: string) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.filter((question) => question.clientId !== questionClientId),
    }));
  };

  const addAnswerOption = (questionClientId: string) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) =>
        question.clientId === questionClientId
          ? { ...question, answerOptions: [...question.answerOptions, createAnswerOption(false)] }
          : question,
      ),
    }));
  };

  const removeAnswerOption = (questionClientId: string, answerClientId: string) => {
    setFormValue((current) => ({
      ...current,
      questions: current.questions.map((question) => {
        if (question.clientId !== questionClientId || question.answerOptions.length <= 2) {
          return question;
        }

        const nextAnswerOptions = question.answerOptions.filter(
          (answerOption) => answerOption.clientId !== answerClientId,
        );

        if (
          question.type === QuestionType.SingleChoice
          && !nextAnswerOptions.some((answerOption) => answerOption.isCorrect)
        ) {
          nextAnswerOptions[0].isCorrect = true;
        }

        return {
          ...question,
          answerOptions: nextAnswerOptions,
        };
      }),
    }));
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();

    const nextErrors = validateForm(formValue);
    setErrors(nextErrors);

    if (nextErrors.length > 0) {
      return;
    }

    await onSubmit(toRequest(formValue));
  };

  return (
    <form className="test-form" onSubmit={handleSubmit}>
      {errors.length > 0 ? (
        <div className="form-errors" role="alert">
          <strong>Check the form</strong>
          <ul>
            {errors.map((error) => (
              <li key={error}>{error}</li>
            ))}
          </ul>
        </div>
      ) : null}

      <Card className="form-card">
        <div className="form-grid">
          <Input
            label="Title"
            maxLength={200}
            name="title"
            onChange={(event) => setFormValue((current) => ({ ...current, title: event.target.value }))}
            placeholder="Final JavaScript fundamentals"
            value={formValue.title}
          />
          <Textarea
            label="Description"
            name="description"
            onChange={(event) => setFormValue((current) => ({ ...current, description: event.target.value }))}
            placeholder="A concise note about the test scope, audience, or timing."
            rows={4}
            value={formValue.description}
          />
        </div>
      </Card>

      <div className="question-list">
        {formValue.questions.map((question, questionIndex) => (
          <Card className="question-card" key={question.clientId}>
            <div className="question-card-header">
              <div>
                <Badge tone="peach">Question {questionIndex + 1}</Badge>
                <h2>{question.text.trim() || "Untitled question"}</h2>
              </div>
              <Button
                disabled={formValue.questions.length <= 1}
                onClick={() => removeQuestion(question.clientId)}
                variant="danger"
              >
                Remove question
              </Button>
            </div>

            <Textarea
              label="Question text"
              name={`question-${question.clientId}`}
              onChange={(event) => updateQuestion(question.clientId, { text: event.target.value })}
              placeholder="What should the learner answer?"
              rows={3}
              value={question.text}
            />

            <div className="type-selector" aria-label="Question type">
              {[QuestionType.SingleChoice, QuestionType.MultipleChoice].map((type) => (
                <button
                  className={question.type === type ? "type-chip active" : "type-chip"}
                  key={type}
                  onClick={() => setQuestionType(question.clientId, type)}
                  type="button"
                >
                  {getQuestionTypeLabel(type)}
                </button>
              ))}
            </div>

            <div className="answer-list">
              {question.answerOptions.map((answerOption, answerIndex) => (
                <div className="answer-row" key={answerOption.clientId}>
                  <label className="correct-toggle">
                    <input
                      checked={answerOption.isCorrect}
                      name={`correct-${question.clientId}`}
                      onChange={(event) =>
                        setCorrectAnswer(question.clientId, answerOption.clientId, event.target.checked)
                      }
                      type={question.type === QuestionType.SingleChoice ? "radio" : "checkbox"}
                    />
                    <span>Correct</span>
                  </label>
                  <Input
                    label={`Option ${answerIndex + 1}`}
                    name={`answer-${answerOption.clientId}`}
                    onChange={(event) =>
                      updateAnswerOption(question.clientId, answerOption.clientId, {
                        text: event.target.value,
                      })
                    }
                    placeholder="Answer option"
                    value={answerOption.text}
                  />
                  <Button
                    disabled={question.answerOptions.length <= 2}
                    onClick={() => removeAnswerOption(question.clientId, answerOption.clientId)}
                    variant="ghost"
                  >
                    Remove
                  </Button>
                </div>
              ))}
            </div>

            <Button onClick={() => addAnswerOption(question.clientId)} variant="secondary">
              Add answer option
            </Button>
          </Card>
        ))}
      </div>

      <div className="form-footer">
        <Button onClick={addQuestion} variant="secondary">
          Add question
        </Button>
        <div className="form-footer-actions">
          <Button onClick={onCancel} variant="ghost">
            Cancel
          </Button>
          <Button disabled={isSaving} type="submit" variant="primary">
            {isSaving ? "Saving..." : submitLabel}
          </Button>
        </div>
      </div>
    </form>
  );
}
