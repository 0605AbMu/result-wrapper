using System.Net;

namespace ResultWrapper.Library;

public partial class Wrapper<T>
{
    public static implicit operator Wrapper<T>(T data) => FromSuccess(data);
    public static implicit operator Wrapper<T>((T content, int code) data) => FromSuccess(data.content, data.code);

    public static implicit operator Wrapper<T>((T content, HttpStatusCode code) data) =>
        FromSuccess(data.content, data.code);

    public static implicit operator Wrapper<T>(int code) => FromStatus(code);
    public static implicit operator Wrapper<T>(HttpStatusCode code) => FromStatus(code);
    public static implicit operator Wrapper<T>(Exception e) => FromError(e);
    public static implicit operator Wrapper<T>((Exception e, int code) data) => FromError(data.e, data.code);
    public static implicit operator Wrapper<T>((Exception e, HttpStatusCode code) data) => FromError(data.e, data.code);
}