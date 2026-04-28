// Matches backend PageResult<T>: { total: number, items: T[] }
export interface PageResult<T> {
  total: number;
  items: T[];
}
