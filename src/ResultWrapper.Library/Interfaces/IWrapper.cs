using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModelError = ResultWrapper.Library.Common.ModelError;

namespace ResultWrapper.Library.Interfaces;

/// <summary>
/// A generic interface that wrapper implements its properties
/// </summary>
/// <typeparam name="T">The Type of Result</typeparam>
public interface IWrapper<T>
{
    /// <summary>
    /// Result id that is auto generated
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Response result code
    /// </summary>
    public int Code { get; protected internal set; }

    /// <summary>
    /// Result data
    /// </summary>
    T? Content { get; init; }

    public string? Message { get; set; }

    string? StackTrace { get; init; }
}