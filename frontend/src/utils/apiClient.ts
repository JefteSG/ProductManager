import axios, { type AxiosError } from 'axios';
import type { ErrorResponse } from '../types/product';

/**
 * Axios instance with the base URL pointing at the .NET API.
 * In development, Vite proxies /api → http://localhost:5241,
 * so no CORS issues arise and the base URL works in both dev and prod.
 */
const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Type guard that narrows an unknown error to a typed Axios API error.
 * Avoids `as string` casts spread across every catch block.
 */
export function isApiError(err: unknown): err is AxiosError<ErrorResponse> {
  return axios.isAxiosError(err) && typeof (err as AxiosError<ErrorResponse>).response?.data?.message === 'string';
}

/**
 * Extracts the API error message or falls back to a generic string.
 */
export function getApiErrorMessage(err: unknown, fallback: string): string {
  return isApiError(err) ? err.response!.data.message : fallback;
}

export default apiClient;
