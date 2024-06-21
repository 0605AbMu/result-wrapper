using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library;
using ResultWrapper.Library.Interfaces;

namespace ResultWrapper.Test;

public class ResultWrapperTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Id_ShouldBeNewGuid_WhenAnyResultSet()
    {
        Wrapper wrapper = ("test", 200);
        
        Assert.IsInstanceOf<Guid>(wrapper.Id);
        Assert.That(wrapper.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void ResultContent_ShouldBeStringObject_WhenGiveOnlyStringObject()
    {
        string result = "test";

        Wrapper wrapper = result;

        Assert.That(wrapper.Content, Is.EqualTo(result));
    }

    [Test]
    public void ResultContent_ShouldBeStringObject_And_StatusCode_ShouldBeSuccess_WhenGiveStringObjectWith200Code()
    {
        string result = "test";

        Wrapper wrapper = (result, 200);

        Assert.That(wrapper.Content, Is.EqualTo(result));
        Assert.That(wrapper.Code, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public void Error_ShouldNotBeNull_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        Wrapper wrapper = exception;

        Assert.NotNull(wrapper.Error);
    }

    [Test]
    public void Content_ShouldBeNull_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        Wrapper wrapper = exception;

        Assert.Null(wrapper.Content);
    }

    [Test]
    public void Error_ShouldBeErrorMessage_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        Wrapper wrapper = exception;

        Assert.That(wrapper.Error, Is.EqualTo(exceptionMessage));
    }

    [Test]
    [TestCase(200, HttpStatusCode.OK)]
    [TestCase(400, HttpStatusCode.BadRequest)]
    [TestCase(404, HttpStatusCode.NotFound)]
    [TestCase(500, HttpStatusCode.InternalServerError)]
    [TestCase(600, 600)]
    public void Code_ShouldBeHttpStatusCode_WhenCodeGives(int code, HttpStatusCode statusCode)
    {
        Wrapper wrapper = code;

        Assert.That(wrapper.Code, Is.EqualTo(statusCode));
    }

    [Test]
    public void ModelStateError_ShouldNotBeNullAndEmpty_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        IWrapperGeneric<object> wrapper = Wrapper.ResultFromModelState(dictionary);

        Assert.That(wrapper.ModelStateError, Is.Not.Null);
        Assert.IsNotEmpty(wrapper.ModelStateError!);
    }

    [Test]
    public void ModelStateError_ShouldBeCollectionOfBindingErrors_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        IWrapperGeneric<object> wrapper = Wrapper.ResultFromModelState(dictionary);
        
        Assert.IsNotNull(wrapper.ModelStateError);

        var stateError = wrapper.ModelStateError![0];

        Assert.That(stateError.Key, Is.EqualTo("username"));
        Assert.That(stateError.ErrorMessage, Is.EqualTo("Min length must be 4"));
    }

    [Test]
    [TestCase(@"{
    ""id"": ""463fadea-066e-4950-a733-5bc789a9ea94"",
    ""code"": 200,
    ""content"": {
        ""name"": {
            ""uz"": ""Test uz"",
            ""ru"": ""Test Ru"",
            ""eng"": ""Test eng"",
            ""cyrl"": ""Test cyrl""
        },
        ""thumb"": """",
        ""groupName"": null,
        ""isSystemDefined"": false,
        ""id"": 0
    },
    ""error"": null,
    ""total"": null,
    ""modelStateError"": null
}")]
    public void Wrapper_MustBe_Deserializable(string rawData)
    {
        var wrapper = JsonSerializer.Deserialize<Wrapper>(rawData);
        
        Assert.IsNotNull(wrapper);
        Assert.IsNotNull(wrapper.Content);
        Assert.That(wrapper.Code, Is.EqualTo(HttpStatusCode.OK));
        Assert.IsNull(wrapper.Error);
        Assert.That(wrapper.Id, Is.EqualTo(Guid.Parse("463fadea-066e-4950-a733-5bc789a9ea94")));
    }
    
    [Test]
    [TestCase(@"{
    ""id"": ""463fadea-066e-4950-a733-5bc789a9ea94"",
    ""code"": 200,
    ""content"": {
        ""name"": {
            ""uz"": ""Test uz"",
            ""ru"": ""Test Ru"",
            ""eng"": ""Test eng"",
            ""cyrl"": ""Test cyrl""
        },
        ""thumb"": """",
        ""groupName"": null,
        ""isSystemDefined"": false,
        ""id"": 0
    },
    ""error"": null,
    ""total"": null,
    ""modelStateError"": null
}")]
    public void WrapperGeneric_MustBe_Deserializable(string rawData)
    {
        var wrapper = JsonSerializer.Deserialize<Wrapper>(rawData);
        
        Assert.IsNotNull(wrapper);
        Assert.IsNotNull(wrapper.Content);
        Assert.That(wrapper.Code, Is.EqualTo(HttpStatusCode.OK));
        Assert.IsNull(wrapper.Error);
        Assert.That(wrapper.Id, Is.EqualTo(Guid.Parse("463fadea-066e-4950-a733-5bc789a9ea94")));
    }
    
}