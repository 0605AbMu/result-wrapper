using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library.Interfaces;

namespace ResultWrapper.Library;

public partial class Wrapper : IWrapper<object?>
{
    [JsonPropertyName("id")] public Guid Id { get; set; } = Guid.NewGuid();
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("content")] public object? Content { get; init; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonIgnore] public string? StackTrace { get; init; }

    #region With

    public Wrapper WithId(Guid id)
    {
        this.Id = id;
        return this;
    }

    public Wrapper WithCode(int code)
    {
        this.Code = code;
        return this;
    }

    public Wrapper WithCode(HttpStatusCode code) => this.WithCode((int)code);

    #endregion

    #region From Exception

    private static Wrapper FromError(string error, int code)
    {
        return new Wrapper()
        {
            Message = error,
            Code = code
        };
    }

    public static Wrapper FromError(Exception exception, int code)
    {
        return new Wrapper()
        {
            Message = exception.Message,
            Code = code
        };
    }

    public static Wrapper FromError(Exception exception) =>
        FromError(exception, (int)HttpStatusCode.InternalServerError);

    public static Wrapper FromError(Exception exception, HttpStatusCode code) =>
        FromError(exception, (int)code);

    public static Wrapper FromError(string error) => FromError(error, (int)HttpStatusCode.InternalServerError);

    #endregion

    #region From model state error

    public static Wrapper FromModelState(ModelStateDictionary modelState,
        string? error, int code)
    {
        return new Wrapper()
        {
            Code = code,
            Message = error,
            Content = modelState.Select(x => new Common.ModelError()
            {
                Key = x.Key,
                ErrorMessage = x.Value?.Errors.FirstOrDefault()?.ErrorMessage
            }).ToList()
        };
    }

    public static Wrapper FromModelState(ModelStateDictionary modelState) =>
        FromModelState(modelState, null, (int)HttpStatusCode.BadRequest);

    public static Wrapper FromModelState(ModelStateDictionary modelState,
        Exception? exception) => FromModelState(modelState, exception?.Message, (int)HttpStatusCode.BadRequest);

    public static Wrapper FromModelState(ModelStateDictionary modelState,
        string? error) =>
        FromModelState(modelState, error, (int)HttpStatusCode.BadRequest);

    #endregion

    #region From success result

    public static Wrapper FromSuccess(object? content, int code)
    {
        return new Wrapper()
        {
            Content = content,
            Code = code
        };
    }

    public static Wrapper FromSuccess(object? content) =>
        FromSuccess(content, (int)HttpStatusCode.OK);

    public static Wrapper FromSuccess(object? content, HttpStatusCode code) =>
        FromSuccess(content, (int)code);

    #endregion

    #region From Status Code

    public static Wrapper FromStatus(int code = 200)
    {
        return new Wrapper()
        {
            Code = code
        };
    }

    public static Wrapper FromStatus(HttpStatusCode code = HttpStatusCode.OK) => FromStatus((int)code);

    #endregion
}