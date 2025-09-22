import type { WeatherForecast, WeatherForecastRequest } from "./types";

const API_BASE = `${import.meta.env.VITE_API_URL ?? "/api"}/WeatherForecast`;

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || response.statusText);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export async function fetchForecasts(): Promise<WeatherForecast[]> {
  const response = await fetch(API_BASE);
  return handleResponse<WeatherForecast[]>(response);
}

export async function fetchForecast(id: string): Promise<WeatherForecast> {
  const response = await fetch(`${API_BASE}/${id}`);
  return handleResponse<WeatherForecast>(response);
}

export async function createForecast(payload: WeatherForecastRequest): Promise<void> {
  const response = await fetch(API_BASE, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  await handleResponse<void>(response);
}

export async function updateForecast(id: string, payload: WeatherForecastRequest): Promise<void> {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  await handleResponse<void>(response);
}

export async function deleteForecast(id: string): Promise<void> {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "DELETE",
  });

  await handleResponse<void>(response);
}
