// src/app/core/models/page-result.ts
// Matches backend PageResult<T>: { total: number, items: T[] }
export interface PageResult<T> {
  total: number;
  items: T[];
}
