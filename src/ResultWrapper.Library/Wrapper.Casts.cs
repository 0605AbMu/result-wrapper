using System.Net;

namespace ResultWrapper.Library;

public partial class Wrapper
{
    public static implicit operator Wrapper(string data) => FromSuccess(data);
    public static implicit operator Wrapper((object content, int code) data) => FromSuccess(data.content, data.code);

    public static implicit operator Wrapper((object content, HttpStatusCode code) data) =>
        FromSuccess(data.content, data.code);

    public static implicit operator Wrapper(int code) => FromStatus(code);
    public static implicit operator Wrapper(HttpStatusCode code) => FromStatus(code);
    public static implicit operator Wrapper(Exception e) => FromError(e);
    public static implicit operator Wrapper((Exception e, int code) data) => FromError(data.e, data.code);
    public static implicit operator Wrapper((Exception e, HttpStatusCode code) data) => FromError(data.e, data.code);
}