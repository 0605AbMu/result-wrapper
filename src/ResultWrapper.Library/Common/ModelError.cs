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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public ModelError()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        
    }
}