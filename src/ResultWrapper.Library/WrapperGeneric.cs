using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library.Interfaces;

namespace ResultWrapper.Library;

public partial class Wrapper<T> : IWrapper<T>
{
    /// <inheritdoc/>
    [JsonPropertyName("id")] public virtual string Id { get; set; } = Activity.Current?.Id ?? Guid.NewGuid().ToString();

    /// <inheritdoc/>
    [JsonPropertyName("code")] public int Code { get; set; }

    /// <inheritdoc/>
    [JsonPropertyName("content")] public T? Content { get; init; }

    /// <inheritdoc/>
    [JsonPropertyName("message")] public string? Message { get; set; }

    #region With

    /// <summary>Sets a custom <see cref="Id"/> and returns the same instance for chaining.</summary>
    public virtual Wrapper<T> WithId(string id)
    {
        this.Id = id;
        return this;
    }

    /// <summary>Sets <see cref="Code"/> and returns the same instance for chaining.</summary>
    public virtual Wrapper<T> WithCode(int code)
    {
        this.Code = code;
        return this;
    }

    /// <summary>Sets <see cref="Code"/> from an <see cref="HttpStatusCode"/> and returns the same instance for chaining.</summary>
    public virtual Wrapper<T> WithCode(HttpStatusCode code) => this.WithCode((int)code);

    /// <summary>Sets <see cref="Message"/> and returns the same instance for chaining.</summary>
    public virtual Wrapper<T> WithMessage(string? message)
    {
        this.Message = message;
        return this;
    }

    #endregion

    #region From Exception

    private static Wrapper<T> FromError(string error, int code)
    {
        return new Wrapper<T>()
        {
            Message = error,
            Code = code
        };
    }

    public static Wrapper<T> FromError(Exception exception, int code)
    {
        return new Wrapper<T>()
        {
            Message = exception.Message,
            Code = code
        };
    }

    public static Wrapper<T> FromError(Exception exception) =>
        FromError(exception, (int)HttpStatusCode.InternalServerError);

    public static Wrapper<T> FromError(Exception exception, HttpStatusCode code) =>
        FromError(exception, (int)code);

    public static Wrapper<T> FromError(string error) => FromError(error, (int)HttpStatusCode.InternalServerError);

    #endregion

    #region From model state error

    public static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        string? error, int code)
    {
        return new Wrapper<IReadOnlyDictionary<string, string?>>()
        {
            Code = code,
            Message = error,
            Content = modelState.ToDictionary(x => x.Key, x => x.Value?.Errors.FirstOrDefault()?.ErrorMessage)
        };
    }

    public static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState) =>
        FromModelState(modelState, null, (int)HttpStatusCode.BadRequest);

    public static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        Exception? exception) => FromModelState(modelState, exception?.Message, (int)HttpStatusCode.BadRequest);

    public static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        string? error) =>
        FromModelState(modelState, error, (int)HttpStatusCode.BadRequest);

    #endregion

    #region From success result

    public static Wrapper<T> FromSuccess(T content, int code)
    {
        return new Wrapper<T>()
        {
            Content = content,
            Code = code
        };
    }

    public static Wrapper<T> FromSuccess(T content) =>
        FromSuccess(content, (int)HttpStatusCode.OK);

    public static Wrapper<T> FromSuccess(T content, HttpStatusCode code) =>
        FromSuccess(content, (int)code);

    /// <summary>
    /// Returns a 200 OK wrapper when <paramref name="content"/> is not null,
    /// otherwise a 404 Not Found wrapper.
    /// </summary>
    public static Wrapper<T> FromSuccessOrNotFound(T? content)
        => content is null ? FromStatus(HttpStatusCode.NotFound) : FromSuccess(content);

    #endregion

    #region Deconstruct

    public void Deconstruct(out T? content, out int code, out string? message)
    {
        content = Content;
        code = Code;
        message = Message;
    }

    public void Deconstruct(out T? content, out int code)
    {
        content = Content;
        code = Code;
    }

    #endregion

    #region From Status Code

    public static Wrapper<T> FromStatus(int code = 200)
    {
        return new Wrapper<T>()
        {
            Code = code
        };
    }

    public static Wrapper<T> FromStatus(HttpStatusCode code = HttpStatusCode.OK) => FromStatus((int)code);

    #endregion
}