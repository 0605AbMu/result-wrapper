using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library.Interfaces;
using ModelError = ResultWrapper.Library.Common.ModelError;

namespace ResultWrapper.Library;

public partial class Wrapper<T> : IWrapper<T>
{
    [JsonPropertyName("id")] public virtual Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("code")] public int Code { get; set; }

    [JsonPropertyName("content")] public T? Content { get; init; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonIgnore] public string? StackTrace { get; init; }

    #region With

    public Wrapper<T> WithId(Guid id)
    {
        this.Id = id;
        return this;
    }

    public Wrapper<T> WithCode(int code)
    {
        this.Code = code;
        return this;
    }

    public Wrapper<T> WithCode(HttpStatusCode code) => this.WithCode((int)code);

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

    public static Wrapper<IReadOnlyCollection<ModelError>> FromModelState(ModelStateDictionary modelState,
        string? error, int code)
    {
        return new Wrapper<IReadOnlyCollection<ModelError>>()
        {
            Code = code,
            Message = error,
            Content = modelState.Select(x => new ModelError()
            {
                Key = x.Key,
                ErrorMessage = x.Value?.Errors.FirstOrDefault()?.ErrorMessage
            }).ToList()
        };
    }

    public static Wrapper<IReadOnlyCollection<ModelError>> FromModelState(ModelStateDictionary modelState) =>
        FromModelState(modelState, null, (int)HttpStatusCode.BadRequest);

    public static Wrapper<IReadOnlyCollection<ModelError>> FromModelState(ModelStateDictionary modelState,
        Exception? exception) => FromModelState(modelState, exception?.Message, (int)HttpStatusCode.BadRequest);

    public static Wrapper<IReadOnlyCollection<ModelError>> FromModelState(ModelStateDictionary modelState,
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