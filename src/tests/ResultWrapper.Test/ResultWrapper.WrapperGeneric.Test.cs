using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library;
using ModelError = ResultWrapper.Library.Common.ModelError;

namespace ResultWrapper.Test;

public class ResultWrapperGenericTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Id_ShouldBeNewGuid_WhenAnyResultSet()
    {
        var wrapper = Wrapper<string>.FromSuccess("", HttpStatusCode.OK);
        
        Assert.IsInstanceOf<Guid>(wrapper.Id);
        Assert.That(wrapper.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void ResultContent_ShouldBeStringObject_WhenGiveOnlyStringObject()
    {
        string result = "test";

        Wrapper<string> wrapper = result;

        Assert.That(wrapper.Content, Is.EqualTo(result));
    }

    [Test]
    public void ResultContent_ShouldBeStringObject_And_StatusCode_ShouldBeSuccess_WhenGiveStringObjectWith200Code()
    {
        string result = "test";

        var wrapper = Wrapper<string>.FromSuccess(result, 200);

        Assert.That(wrapper.Content, Is.EqualTo(result));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public void Error_ShouldNotBeNull_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        var wrapper = Wrapper<object>.FromError(exception);

        Assert.NotNull(wrapper.Message);
    }

    [Test]
    public void Content_ShouldBeNull_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        var wrapper = Wrapper<object>.FromError(exception);

        Assert.Null(wrapper.Content);
    }

    [Test]
    public void Error_ShouldBeErrorMessage_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        var wrapper = Wrapper<object>.FromError(exception);

        Assert.That(wrapper.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    [TestCase(200, HttpStatusCode.OK)]
    [TestCase(400, HttpStatusCode.BadRequest)]
    [TestCase(404, HttpStatusCode.NotFound)]
    [TestCase(500, HttpStatusCode.InternalServerError)]
    [TestCase(600, 600)]
    public void Code_ShouldBeHttpStatusCode_WhenCodeGives(int code, HttpStatusCode statusCode)
    {
        var wrapper = Wrapper<object>.FromStatus(code);
        Assert.That(wrapper.Code, Is.EqualTo((int)statusCode));
    }

    [Test]
    public void ModelStateError_ShouldNotBeNullAndEmpty_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        var wrapper = Wrapper<object>.FromModelState(dictionary);

        Assert.That(wrapper.Content, Is.Not.Null);
        Assert.That(wrapper.Content, Is.AssignableTo(typeof(IReadOnlyCollection<ModelError>)));
        Assert.IsNotEmpty(wrapper.Content);
    }

    [Test]
    public void ModelStateError_ShouldBeCollectionOfBindingErrors_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        var wrapper = Wrapper<object>.FromModelState(dictionary);
        
        Assert.IsNotNull(wrapper.Content);

        var stateError = wrapper.Content!.First();

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
    ""message"": null
}")]
    public void Wrapper_MustBe_Deserializable(string rawData)
    {
        var wrapper = JsonSerializer.Deserialize<Wrapper<object>>(rawData);
        
        Assert.IsNotNull(wrapper);
        Assert.IsNotNull(wrapper.Content);
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.IsNull(wrapper.Message);
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
    ""message"": null
}")]
    public void WrapperGeneric_MustBe_Deserializable(string rawData)
    {
        var wrapper = JsonSerializer.Deserialize<Wrapper<object>>(rawData);
        
        Assert.IsNotNull(wrapper);
        Assert.IsNotNull(wrapper.Content);
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.IsNull(wrapper.Message);
        Assert.That(wrapper.Id, Is.EqualTo(Guid.Parse("463fadea-066e-4950-a733-5bc789a9ea94")));
    }
    
}