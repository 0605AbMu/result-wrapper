using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelError = ResultWrapper.Library.Common.ModelError;

namespace ResultWrapper.Library.Interfaces;

/// <summary>
/// A generic interface that wrapper implements its properties
/// </summary>
/// <typeparam name="T">The Type of Result</typeparam>
public interface IWrapperGeneric<T>
{
    /// <summary>
    /// Result id that is auto generated
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Response result code
    /// </summary>
    HttpStatusCode Code { get; init; }

    /// <summary>
    /// Result data
    /// </summary>
    T? Content { get; init; }

    /// <summary>
    /// Result as exception message when any exception encountered
    /// Else is null
    /// </summary>
    string? Error { get; init; }

    /// <summary>
    /// Data query from request
    /// </summary>
    object? Query { get; set; }

    /// <summary>
    /// It is model binding errors
    /// </summary>
    List<ModelError>? ModelStateError { get; init; }

    string? StackTrace { get; init; }
}