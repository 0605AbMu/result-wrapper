using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ResultWrapper.Library.Extensions;

public static class ModelStateDictionaryExtensions
{
    public static Wrapper<IReadOnlyDictionary<string, string?>> ToWrapper(
        this ModelStateDictionary modelState) =>
        Wrapper.FromModelState(modelState);

    public static Wrapper<IReadOnlyDictionary<string, string?>> ToWrapper(
        this ModelStateDictionary modelState, string? message) =>
        Wrapper.FromModelState(modelState, message);

    public static Wrapper<IReadOnlyDictionary<string, string?>> ToWrapper(
        this ModelStateDictionary modelState, Exception? exception) =>
        Wrapper.FromModelState(modelState, exception);

    public static Wrapper<IReadOnlyDictionary<string, string?>> ToWrapper(
        this ModelStateDictionary modelState, string? message, int code) =>
        Wrapper.FromModelState(modelState, message, code);
}
