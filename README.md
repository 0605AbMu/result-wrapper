[![Publish package to Nuget](https://github.com/0605AbMu/result-wrapper/actions/workflows/pack.yml/badge.svg)](https://github.com/0605AbMu/result-wrapper/actions/workflows/pack.yml)

# A Simple Result wrapper for ASP.NET API projects
You can wrap all ASP.NET API response data via this package.

## Features
 - **Simple**
 - **Easy to use**
 - **Support both strong and dynamic types**
 - **Support auto type casting**

## Installation
You can install via nuget. [Link](https://www.nuget.org/packages/ResultWrapper.Library)
```
dotnet add package ResultWrapper.Library
```

## Usage
### Status result
```csharp
[HttpGet]
public Wrapper OnlyStatus()
{
    return Wrapper.FromStatus(HttpStatusCode.OK);
}

[HttpGet]
public Wrapper OnlyStatus()
{
    return HttpStatusCode.OK;
}

[HttpGet]
public Wrapper OnlyStatus()
{
    return 200;
}
```
### Success result
```csharp
[HttpGet]
public Wrapper Add(int a, int b)
{
    return Wrapper.FromSuccess(a + b);
}

[HttpGet]
public Wrapper TextResult(int a, int b)
{
    return "Well Done!";
}

[HttpGet]
public Wrapper ResultWithCustomCode(int a, int b)
{
    return (new
    {
        Sum = a + b
    }, 200);
}
```
### Error result
```csharp

[HttpGet]
public Wrapper Error()
{
    try
    {
        throw new InvalidOperationException();
    }
    catch (Exception e)
    {
        return Wrapper.FromError(e, HttpStatusCode.BadRequest);
    }
}
```
### Model state error
```csharp
[HttpGet]
public Wrapper Error([FromBody] SampleModel model)
{
    if (!ModelState.IsValid)
        return Wrapper.FromModelState(ModelState);

    return Wrapper.FromStatus(HttpStatusCode.OK);
}
```

> And you can also use **Strong Typed Wrapper(Wrapper<T>)** with such methods.
```csharp
[HttpGet]
public Wrapper<string> ResultWithCustomCode()
{
    return Wrapper<string>.FromSuccess("Well Done!", HttpStatusCode.OK);
}
```
or
```csharp
[HttpGet]
public Wrapper<string> ResultWithCustomCode()
{
    return "Well Done!";
}
```

**Thanks for your attention!**