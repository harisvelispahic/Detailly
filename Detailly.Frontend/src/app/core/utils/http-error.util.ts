interface ApiErrorBody {
  message?: string;
  title?: string;
  errors?: Record<string, string[]>;
}

export function extractHttpError(err: unknown): string | null {
  if (err == null || typeof err !== 'object') return null;
  const e = err as { error?: unknown; message?: string; statusText?: string };

  if (e.error != null) {
    if (typeof e.error === 'string') return e.error;

    const body = e.error as ApiErrorBody;
    if (body.message) return body.message;
    if (body.title) return body.title;
    if (body.errors) {
      const flat = Object.values(body.errors).flat();
      if (flat.length > 0) return flat.join(', ');
    }
  }

  if (e.message) return e.message;
  if (e.statusText && e.statusText !== 'Unknown Error') return e.statusText;

  return null;
}
