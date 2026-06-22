using System.Text.Json.Serialization;

namespace ResultWrapper.Library.Common;

public class ModelError
{
    public string Key { get; set; }
    public string? ErrorMessage { get; set; }

    [JsonConstructor]
    public ModelError(string key, string? errorMessage)
    {
        Key = key;
        ErrorMessage = errorMessage;
    }

    public ModelError()
    {
        Key = string.Empty;
    }
}