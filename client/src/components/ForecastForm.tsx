import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import type { WeatherForecastRequest } from "../types";

const forecastSchema = z.object({
  date: z
    .string({ required_error: "Date is required" })
    .nonempty("Date is required"),
  temperatureC: z
    .number({ invalid_type_error: "Temperature must be a number" })
    .min(-100, "Too cold to be realistic")
    .max(100, "Too hot to be realistic"),
  summary: z
    .string({ required_error: "Summary is required" })
    .min(3, "Summary must be at least 3 characters")
    .max(256, "Summary is too long"),
  city: z
    .string({ required_error: "City is required" })
    .min(2, "City name must be at least 2 characters")
    .max(128, "City name is too long"),
});

export type ForecastFormValues = z.infer<typeof forecastSchema>;

interface ForecastFormProps {
  readonly defaultValues?: ForecastFormValues;
  readonly submitLabel: string;
  readonly isSubmitting?: boolean;
  readonly onSubmit: (values: WeatherForecastRequest) => Promise<void> | void;
  readonly onCancel?: () => void;
}

export function ForecastForm({
  defaultValues,
  submitLabel,
  isSubmitting = false,
  onSubmit,
  onCancel,
}: ForecastFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ForecastFormValues>({
    resolver: zodResolver(forecastSchema),
    defaultValues,
  });

  const handleFormSubmit = async (values: ForecastFormValues) => {
    await onSubmit({
      ...values,
      temperatureC: Number(values.temperatureC),
    });
    if (!defaultValues) {
      reset();
    }
  };

  return (
    <form className="forecast-form" onSubmit={handleSubmit(handleFormSubmit)} noValidate>
      <div className="field">
        <label htmlFor="date">Date</label>
        <input
          type="date"
          id="date"
          {...register("date")}
          disabled={isSubmitting}
        />
        {errors.date && <span className="error">{errors.date.message}</span>}
      </div>

      <div className="field">
        <label htmlFor="temperatureC">Temperature (°C)</label>
        <input
          type="number"
          id="temperatureC"
          step="0.1"
          {...register("temperatureC", { valueAsNumber: true })}
          disabled={isSubmitting}
        />
        {errors.temperatureC && (
          <span className="error">{errors.temperatureC.message}</span>
        )}
      </div>

      <div className="field">
        <label htmlFor="summary">Summary</label>
        <input
          type="text"
          id="summary"
          {...register("summary")}
          disabled={isSubmitting}
        />
        {errors.summary && <span className="error">{errors.summary.message}</span>}
      </div>

      <div className="field">
        <label htmlFor="city">City</label>
        <input
          type="text"
          id="city"
          {...register("city")}
          disabled={isSubmitting}
        />
        {errors.city && <span className="error">{errors.city.message}</span>}
      </div>

      <div className="form-actions">
        <button type="submit" disabled={isSubmitting}>
          {submitLabel}
        </button>
        {onCancel && (
          <button type="button" onClick={onCancel} className="link" disabled={isSubmitting}>
            Cancel
          </button>
        )}
      </div>
    </form>
  );
}
