export enum QuestionType {
  SingleChoice = 0,
  MultipleChoice = 1,
}

export interface TestListItemDto {
  description?: string;
  id: number;
  title: string;
  questionsCount: number;
}

export interface AnswerOptionDto {
  id: number;
  text: string;
  isCorrect: boolean;
}

export interface QuestionDto {
  id: number;
  text: string;
  type: QuestionType;
  answerOptions: AnswerOptionDto[];
}

export interface TestDetailsDto {
  description?: string;
  id: number;
  title: string;
  questions: QuestionDto[];
}

export interface TakeAnswerOptionDto {
  id: number;
  text: string;
}

export interface TakeQuestionDto {
  id: number;
  text: string;
  type: QuestionType;
  answerOptions: TakeAnswerOptionDto[];
}

export interface TakeTestDto {
  description?: string;
  id: number;
  title: string;
  questions: TakeQuestionDto[];
}

export interface CreateAnswerOptionRequest {
  text: string;
  isCorrect: boolean;
}

export interface CreateQuestionRequest {
  text: string;
  type: QuestionType;
  answerOptions: CreateAnswerOptionRequest[];
}

export interface CreateTestRequest {
  description?: string;
  title: string;
  questions: CreateQuestionRequest[];
}

export type UpdateTestRequest = CreateTestRequest;

export interface SubmittedQuestionAnswerDto {
  questionId: number;
  selectedAnswerOptionIds: number[];
}

export interface SubmitTestAttemptRequest {
  answers: SubmittedQuestionAnswerDto[];
}

export interface TestAttemptResultDto {
  testId: number;
  score: number;
  maxScore: number;
  percentage: number;
}
