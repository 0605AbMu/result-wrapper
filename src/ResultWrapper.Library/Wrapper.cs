using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ResultWrapper.Library;

public partial class Wrapper : Wrapper<object?>
{
    #region With

    public override Wrapper WithId(string id)
    {
        this.Id = id;
        return this;
    }

    public override Wrapper WithCode(int code)
    {
        this.Code = code;
        return this;
    }

    public override Wrapper WithCode(HttpStatusCode code) => this.WithCode((int)code);

    /// <inheritdoc/>
    public override Wrapper WithMessage(string? message)
    {
        this.Message = message;
        return this;
    }

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

    public new static Wrapper FromError(Exception exception, int code)
    {
        return new Wrapper()
        {
            Message = exception.Message,
            Code = code
        };
    }

    public new static Wrapper FromError(Exception exception) =>
        FromError(exception, (int)HttpStatusCode.InternalServerError);

    public new static Wrapper FromError(Exception exception, HttpStatusCode code) =>
        FromError(exception, (int)code);

    public new static Wrapper FromError(string error) => FromError(error, (int)HttpStatusCode.InternalServerError);

    #endregion

    #region From model state error

    public new static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        string? error, int code)
    {
        return new Wrapper<IReadOnlyDictionary<string, string?>>()
        {
            Code = code,
            Message = error,
            Content = modelState.ToDictionary(x => x.Key, x => x.Value?.Errors.FirstOrDefault()?.ErrorMessage)
        };
    }

    public new static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState) =>
        FromModelState(modelState, null, (int)HttpStatusCode.BadRequest);

    public new static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        Exception? exception) => FromModelState(modelState, exception?.Message, (int)HttpStatusCode.BadRequest);

    public new static Wrapper<IReadOnlyDictionary<string, string?>> FromModelState(ModelStateDictionary modelState,
        string? error) =>
        FromModelState(modelState, error, (int)HttpStatusCode.BadRequest);

    #endregion

    #region From success result

    public new static Wrapper FromSuccess(object content, int code)
    {
        return new Wrapper()
        {
            Content = content,
            Code = code
        };
    }

    public new static Wrapper FromSuccess(object content) =>
        FromSuccess(content, (int)HttpStatusCode.OK);

    public new static Wrapper FromSuccess(object content, HttpStatusCode code) =>
        FromSuccess(content, (int)code);

    public new static Wrapper FromSuccessOrNotFound(object? content)
        => content is null ? FromStatus(HttpStatusCode.NotFound) : FromSuccess(content);

    #endregion

    #region From Status Code

    public new static Wrapper FromStatus(int code = 200)
    {
        return new Wrapper()
        {
            Code = code
        };
    }

    public new static Wrapper FromStatus(HttpStatusCode code = HttpStatusCode.OK) => FromStatus((int)code);

    #endregion
}