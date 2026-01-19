using System.Text.Json.Serialization;

namespace TR.Connector.ApiDto;

/// <summary>
/// Универсальная обёртка ответа от API управляемой системы.
/// Соответствует формату:
/// {
///   "data": { ... },
///   "success": true,
///   "errorText": null,
///   "count": 1
/// }
/// </summary>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Полезная нагрузка ответа.
    /// Может быть объектом, списком или null при ошибке.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    /// <summary>
    /// Признак успешности операции.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    /// <summary>
    /// Текст ошибки, если Success = false.
    /// </summary>
    [JsonPropertyName("errorText")]
    public string? ErrorText { get; init; }

    /// <summary>
    /// Количество элементов (используется сервером, даже без пагинации).
    /// </summary>
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    /// <summary>
    /// Удобный метод для валидации ответа.
    /// Бросает исключение, если сервер вернул ошибку.
    /// </summary>
    public T EnsureSuccess()
    {
        if (!Success)
            throw new InvalidOperationException(ErrorText ?? "Произошла ошибка");

        if (Data is null)
            throw new InvalidOperationException("Ошибка: данные пустые");

        return Data;
    }
}
