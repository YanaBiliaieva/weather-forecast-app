export interface WeatherForecast {
  id: string;
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
  city: string;
}

export interface WeatherForecastRequest {
  date: string;
  temperatureC: number;
  summary: string;
  city: string;
}
