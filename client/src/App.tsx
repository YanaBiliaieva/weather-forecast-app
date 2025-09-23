import { useEffect, useMemo, useState } from "react";
import "./App.css";
import { createForecast, deleteForecast, fetchForecasts, updateForecast } from "./api";
import { ForecastForm } from "./components/ForecastForm";
import { ForecastList } from "./components/ForecastList";
import type { WeatherForecast, WeatherForecastRequest } from "./types";

function App() {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedForecast, setSelectedForecast] = useState<WeatherForecast | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const loadForecasts = async () => {
    try {
      setLoading(true);
      const data = await fetchForecasts();
      setForecasts(
        data.map((item) => ({
          ...item,
          temperatureC: Number(item.temperatureC),
          temperatureF: Number(item.temperatureF),
        }))
      );
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load forecasts");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadForecasts().catch(() => {
      setError("Failed to load forecasts");
    });
  }, []);

  const handleCreate = async (values: WeatherForecastRequest) => {
    try {
      setIsSubmitting(true);
      await createForecast(values);
      await loadForecasts();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create forecast");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdate = async (values: WeatherForecastRequest) => {
    if (!selectedForecast) {
      return;
    }

    try {
      setIsSubmitting(true);
      await updateForecast(selectedForecast.id, values);
      setSelectedForecast(null);
      await loadForecasts();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update forecast");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (forecast: WeatherForecast) => {
    if (!window.confirm(`Delete the forecast for ${forecast.city}?`)) {
      return;
    }

    try {
      await deleteForecast(forecast.id);
      if (selectedForecast?.id === forecast.id) {
        setSelectedForecast(null);
      }
      await loadForecasts();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to delete forecast");
    }
  };

  const formDefaults = useMemo(() => {
    if (!selectedForecast) {
      return undefined;
    }

    return {
      date: new Date(selectedForecast.date).toISOString().split("T")[0],
      temperatureC: selectedForecast.temperatureC,
      summary: selectedForecast.summary,
      city: selectedForecast.city,
    };
  }, [selectedForecast]);

  return (
    <div className="app">
      <header>
        <h1>Weather Forecast Dashboard</h1>
        <p>Manage daily forecasts stored in SQL Server through the Web API.</p>
      </header>

      {error && <div className="error-banner">{error}</div>}

      <main>
        <section className="panel">
          <div className="panel-header">
            <h2>{selectedForecast ? "Edit Forecast" : "Create Forecast"}</h2>
            {selectedForecast && (
              <button type="button" className="link" onClick={() => setSelectedForecast(null)}>
                Start a new entry
              </button>
            )}
          </div>
          <ForecastForm
            defaultValues={formDefaults}
            submitLabel={selectedForecast ? "Update" : "Create"}
            isSubmitting={isSubmitting}
            onSubmit={selectedForecast ? handleUpdate : handleCreate}
            onCancel={selectedForecast ? () => setSelectedForecast(null) : undefined}
          />
        </section>

        <section className="panel">
          <div className="panel-header">
            <h2>Existing Forecasts</h2>
            <button type="button" onClick={loadForecasts} className="link" disabled={loading}>
              Refresh
            </button>
          </div>
          {loading ? (
            <p>Loading…</p>
          ) : (
            <ForecastList
              forecasts={forecasts}
              onEdit={(forecast) => setSelectedForecast(forecast)}
              onDelete={handleDelete}
            />
          )}
        </section>
      </main>
    </div>
  );
}

export default App;
