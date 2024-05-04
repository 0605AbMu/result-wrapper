using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library.Interfaces;
using ModelError = ResultWrapper.Library.Common.ModelError;

namespace ResultWrapper.Library;

public class Wrapper : WrapperGeneric<object>, IWrapper
{
    public Wrapper(Exception? exception, HttpStatusCode code = HttpStatusCode.InternalServerError) : base(
        exception, code)
    {
    }

    public Wrapper(object? content, HttpStatusCode code = HttpStatusCode.OK) : base(content, code)
    {
    }

    public Wrapper(HttpStatusCode code) : base(code)
    {
    }

    public Wrapper(ModelStateDictionary modelState, Exception? exception = null) : base(modelState, exception)
    {
    }

    public Wrapper(IEnumerable<object> content, int total, HttpStatusCode code = HttpStatusCode.OK) :
        base(code)
    {
        Content = content;
        Code = code;
        Total = total;
    }

    public Wrapper(string message, HttpStatusCode code = HttpStatusCode.OK) : base(message, code)
    {
        Content = message;
        Code = code;
    }

    public static implicit operator Wrapper(string s)
    {
        return new Wrapper(s);
    }

    public static Wrapper FromIConvertible(IConvertible s)
    {
        return new Wrapper(s);
    }

    public static implicit operator Wrapper((string content, int statusCode) data)
    {
        return new Wrapper(data.content, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator Wrapper((IConvertible content, int statusCode) data)
    {
        return new Wrapper(data.content, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator Wrapper((IEnumerable<object> items, int total) data)
    {
        return new Wrapper(data.items, data.total);
    }

    public static implicit operator Wrapper((IEnumerable<IComparable> items, int total) data)
    {
        return new Wrapper(data.items, data.total);
    }

    public static implicit operator Wrapper((IEnumerable<object> items, int total, int statusCode) data)
    {
        return new Wrapper(data.items, data.total, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator
        Wrapper((IEnumerable<IComparable> items, int total, int statusCode) data)
    {
        return new Wrapper(data.items, data.total, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator Wrapper(Exception exception)
    {
        return new Wrapper(exception);
    }

    public static implicit operator Wrapper((Exception exception, int statusCode) data)
    {
        return new Wrapper(data.exception, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator Wrapper((object? content, int statusCode) data)
    {
        return new Wrapper(data.content, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator Wrapper(int statusCode)
    {
        return new Wrapper((HttpStatusCode)statusCode);
    }
}

public class WrapperGeneric<T> : IWrapperGeneric<T>
{
    [JsonInclude] public Guid Id = Guid.NewGuid();

    public WrapperGeneric(Exception? exception, HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        Code = code;
        StackTrace = exception?.StackTrace;
    }

    public WrapperGeneric(T? content, HttpStatusCode code = HttpStatusCode.OK)
    {
        Content = content;
        Code = code;
    }


    public WrapperGeneric(HttpStatusCode code)
    {
        Code = code;
    }

    public WrapperGeneric(ModelStateDictionary modelState, Exception? exception = null)
    {
        Code = HttpStatusCode.BadRequest;
        ModelStateError = modelState
            .Where(x => x.Value?.ValidationState == ModelValidationState.Invalid)
            .Select(x => new ModelError
            {
                Key = x.Key,
                ErrorMessage = x.Value?.Errors?.FirstOrDefault()?.ErrorMessage
            }).ToList();

        var errorResponse = new WrapperGeneric<T>(exception);
        Error = errorResponse.Error;
        StackTrace = errorResponse.StackTrace;
    }


    public HttpStatusCode Code { get; init; } = HttpStatusCode.OK;
    public T? Content { get; init; }
    public string? Error { get; init; }
    public int? Total { get; set; }
    public List<ModelError>? ModelStateError { get; init; }
    [JsonIgnore] public string? StackTrace { get; init; }


    public static WrapperGeneric<T> ResultFromException(Exception exception,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new WrapperGeneric<T>(exception, statusCode);
    }

    public static WrapperGeneric<T> ResultFromModelState(ModelStateDictionary modelState,
        Exception? exception = null)
    {
        return new WrapperGeneric<T>(modelState, exception);
    }

    public static WrapperGeneric<T> ResultFromContent(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new WrapperGeneric<T>(content, statusCode);
    }

    public static implicit operator WrapperGeneric<T>(Exception exception)
    {
        return new WrapperGeneric<T>(exception);
    }

    public static implicit operator WrapperGeneric<T>((Exception exception, int statusCode) data)
    {
        return new WrapperGeneric<T>(data.exception, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator WrapperGeneric<T>((T? content, int statusCode) data)
    {
        return new WrapperGeneric<T>(data.content, (HttpStatusCode)data.statusCode);
    }

    public static implicit operator WrapperGeneric<T>(int statusCode)
    {
        return new WrapperGeneric<T>((HttpStatusCode)statusCode);
    }
}