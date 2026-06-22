namespace ResultWrapper.Library.Interfaces;

/// <summary>
/// Defines the contract for a response wrapper that standardises API output.
/// </summary>
/// <typeparam name="T">The type of the response payload.</typeparam>
public interface IWrapper<T>
{
    /// <summary>
    /// Auto-generated request identifier. Defaults to <see cref="System.Diagnostics.Activity.Current"/>.Id when
    /// a tracing activity is active, otherwise a new <see cref="System.Guid"/>.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// HTTP or custom status code for the response.
    /// </summary>
    int Code { get; set; }

    /// <summary>
    /// Response payload.
    /// </summary>
    T? Content { get; init; }

    /// <summary>
    /// Optional human-readable message, typically describing an error.
    /// </summary>
    string? Message { get; set; }
}