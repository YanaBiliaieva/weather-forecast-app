import type { WeatherForecast } from "../types";

interface ForecastListProps {
  readonly forecasts: WeatherForecast[];
  readonly onEdit: (forecast: WeatherForecast) => void;
  readonly onDelete: (forecast: WeatherForecast) => void;
}

export function ForecastList({ forecasts, onEdit, onDelete }: ForecastListProps) {
  if (!forecasts.length) {
    return <p className="empty">No forecasts yet. Create one to get started.</p>;
  }

  const formatter = new Intl.DateTimeFormat(undefined, {
    year: "numeric",
    month: "short",
    day: "numeric",
  });

  return (
    <ul className="forecast-list">
      {forecasts.map((forecast) => (
        <li key={forecast.id} className="forecast-card">
          <div>
            <h3>
              {forecast.city} — {formatter.format(new Date(forecast.date))}
            </h3>
            <p className="temperature">
              {forecast.temperatureC.toFixed(1)}°C / {forecast.temperatureF.toFixed(1)}°F
            </p>
            <p>{forecast.summary}</p>
          </div>
          <div className="card-actions">
            <button type="button" onClick={() => onEdit(forecast)}>
              Edit
            </button>
            <button type="button" className="danger" onClick={() => onDelete(forecast)}>
              Delete
            </button>
          </div>
        </li>
      ))}
    </ul>
  );
}
