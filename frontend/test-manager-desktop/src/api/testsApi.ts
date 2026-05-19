import type {
  CreateTestRequest,
  SubmitTestAttemptRequest,
  TestAttemptResultDto,
  TestDetailsDto,
  TestListItemDto,
  UpdateTestRequest,
} from "../types/tests";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5151";

async function request<TResponse>(
  path: string,
  options: RequestInit = {},
): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
    ...options,
  });

  if (!response.ok) {
    const errorBody = await readErrorBody(response);
    throw new Error(errorBody || `Request failed with status ${response.status}.`);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}

async function readErrorBody(response: Response): Promise<string | null> {
  const contentType = response.headers.get("content-type");

  if (!contentType?.includes("application/json")) {
    return response.text();
  }

  const body = (await response.json()) as { error?: string; title?: string };
  return body.error ?? body.title ?? null;
}

export const testsApi = {
  getAll(): Promise<TestListItemDto[]> {
    return request<TestListItemDto[]>("/api/tests");
  },

  getById(id: number): Promise<TestDetailsDto> {
    return request<TestDetailsDto>(`/api/tests/${id}`);
  },

  create(requestBody: CreateTestRequest): Promise<TestDetailsDto> {
    return request<TestDetailsDto>("/api/tests", {
      method: "POST",
      body: JSON.stringify(requestBody),
    });
  },

  update(id: number, requestBody: UpdateTestRequest): Promise<TestDetailsDto> {
    return request<TestDetailsDto>(`/api/tests/${id}`, {
      method: "PUT",
      body: JSON.stringify(requestBody),
    });
  },

  delete(id: number): Promise<void> {
    return request<void>(`/api/tests/${id}`, {
      method: "DELETE",
    });
  },

  submit(id: number, requestBody: SubmitTestAttemptRequest): Promise<TestAttemptResultDto> {
    return request<TestAttemptResultDto>(`/api/tests/${id}/submit`, {
      method: "POST",
      body: JSON.stringify(requestBody),
    });
  },
};
