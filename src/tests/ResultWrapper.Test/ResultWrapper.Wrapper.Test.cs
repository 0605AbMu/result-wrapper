using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ResultWrapper.Library;
using ResultWrapper.Library.Extensions;

namespace ResultWrapper.Test;

public class ResultWrapperTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region Id

    [Test]
    public void Id_ShouldBeActivityId_WhenActivityExists()
    {
        if (Activity.Current == null)
            return;

        var wrapper = Wrapper.FromSuccess("", HttpStatusCode.OK);

        Assert.That(wrapper.Id, Is.EqualTo(Activity.Current?.Id));
    }

    [Test]
    public void Id_ShouldBeNewGuid_WhenAnyResultSet()
    {
        var wrapper = Wrapper.FromSuccess("", HttpStatusCode.OK);

        Assert.NotNull(wrapper.Id);
        Assert.True(Guid.TryParse(wrapper.Id, out _));
    }

    #endregion

    #region FromSuccess

    [Test]
    public void ResultContent_ShouldBeStringObject_WhenGiveOnlyStringObject()
    {
        string result = "test";

        var wrapper = Wrapper.FromSuccess(result);

        Assert.That(wrapper.Content, Is.EqualTo(result));
    }

    [Test]
    public void ResultContent_ShouldBeStringObject_And_StatusCode_ShouldBeSuccess_WhenGiveStringObjectWith200Code()
    {
        string result = "test";

        Wrapper wrapper = result;

        Assert.That(wrapper.Content, Is.EqualTo(result));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public void FromSuccess_WithHttpStatusCode_ShouldSetCorrectCode()
    {
        var wrapper = Wrapper.FromSuccess("data", HttpStatusCode.Created);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Created));
        Assert.That(wrapper.Content, Is.EqualTo("data"));
    }

    #endregion

    #region FromError

    [Test]
    public void Error_ShouldNotBeNull_WhenExceptionGives()
    {
        var exception = new Exception("Test exception");

        var wrapper = Wrapper.FromError(exception);

        Assert.NotNull(wrapper.Message);
    }

    [Test]
    public void Content_ShouldBeNull_WhenExceptionGives()
    {
        var exception = new Exception("Test exception");

        var wrapper = Wrapper.FromError(exception);

        Assert.Null(wrapper.Content);
    }

    [Test]
    public void Error_ShouldBeErrorMessage_WhenExceptionGives()
    {
        var exceptionMessage = "Test exception";
        var exception = new Exception(exceptionMessage);

        var wrapper = Wrapper.FromError(exception);

        Assert.That(wrapper.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    public void FromError_WithCode_ShouldSetCorrectCode()
    {
        var exception = new Exception("err");

        var wrapper = Wrapper.FromError(exception, HttpStatusCode.BadRequest);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(wrapper.Message, Is.EqualTo("err"));
    }

    [Test]
    public void FromError_WithString_ShouldSetMessage()
    {
        var wrapper = Wrapper.FromError("custom error message");

        Assert.That(wrapper.Message, Is.EqualTo("custom error message"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.Null(wrapper.Content);
    }

    #endregion

    #region FromStatus

    [Test]
    [TestCase(200, HttpStatusCode.OK)]
    [TestCase(400, HttpStatusCode.BadRequest)]
    [TestCase(404, HttpStatusCode.NotFound)]
    [TestCase(500, HttpStatusCode.InternalServerError)]
    [TestCase(600, 600)]
    public void Code_ShouldBeHttpStatusCode_WhenCodeGives(int code, HttpStatusCode statusCode)
    {
        var wrapper = Wrapper.FromStatus(code);
        Assert.That(wrapper.Code, Is.EqualTo((int)statusCode));
    }

    [Test]
    public void FromStatus_HttpStatusCode_ShouldSetCode()
    {
        var wrapper = Wrapper.FromStatus(HttpStatusCode.NotFound);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.Null(wrapper.Content);
        Assert.Null(wrapper.Message);
    }

    #endregion

    #region FromModelState

    [Test]
    public void ModelStateError_ShouldNotBeNullAndEmpty_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        var wrapper = Wrapper.FromModelState(dictionary);

        Assert.That(wrapper.Content, Is.Not.Null);
        Assert.That(wrapper.Content, Is.AssignableTo(typeof(IReadOnlyDictionary<string, string?>)));
        Assert.IsNotEmpty((IReadOnlyDictionary<string, string?>)wrapper.Content!);
    }

    [Test]
    public void ModelStateError_ShouldBeCollectionOfBindingErrors_WhenModelStateDictionaryGives()
    {
        ModelStateDictionary dictionary = new ModelStateDictionary();
        dictionary.AddModelError("username", "Min length must be 4");

        var wrapper = Wrapper.FromModelState(dictionary);

        Assert.IsNotNull(wrapper.Content);

        var modelStateDict = ((IReadOnlyDictionary<string, string?>)wrapper.Content)!;

        Assert.That(modelStateDict.Count, Is.EqualTo(1));

        var first = modelStateDict.First();

        Assert.That(first.Key, Is.EqualTo("username"));
        Assert.That(first.Value, Is.EqualTo("Min length must be 4"));
    }

    [Test]
    public void ModelStateError_WithMessage_ShouldSetMessage()
    {
        var dictionary = new ModelStateDictionary();
        dictionary.AddModelError("email", "Invalid format");

        var wrapper = Wrapper.FromModelState(dictionary, "Validation failed");

        Assert.That(wrapper.Message, Is.EqualTo("Validation failed"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public void ModelStateError_WithException_ShouldUseExceptionMessage()
    {
        var dictionary = new ModelStateDictionary();
        dictionary.AddModelError("name", "Required");
        var exception = new Exception("Binding error");

        var wrapper = Wrapper.FromModelState(dictionary, exception);

        Assert.That(wrapper.Message, Is.EqualTo("Binding error"));
    }

    [Test]
    public void ModelStateError_WithCustomCode_ShouldSetCode()
    {
        var dictionary = new ModelStateDictionary();
        dictionary.AddModelError("field", "Error");

        var wrapper = Wrapper.FromModelState(dictionary, "err", 422);

        Assert.That(wrapper.Code, Is.EqualTo(422));
    }

    #endregion

    #region Fluent methods

    [Test]
    public void WithId_ShouldOverrideId()
    {
        var wrapper = Wrapper.FromSuccess("data").WithId("custom-id");

        Assert.That(wrapper.Id, Is.EqualTo("custom-id"));
    }

    [Test]
    public void WithCode_Int_ShouldOverrideCode()
    {
        var wrapper = Wrapper.FromSuccess("data").WithCode(201);

        Assert.That(wrapper.Code, Is.EqualTo(201));
    }

    [Test]
    public void WithCode_HttpStatusCode_ShouldOverrideCode()
    {
        var wrapper = Wrapper.FromSuccess("data").WithCode(HttpStatusCode.Created);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Created));
    }

    [Test]
    public void WithMessage_ShouldOverrideMessage()
    {
        var wrapper = Wrapper.FromSuccess("data").WithMessage("created successfully");

        Assert.That(wrapper.Message, Is.EqualTo("created successfully"));
    }

    [Test]
    public void FluentChain_ShouldReturnSameInstance()
    {
        var wrapper = Wrapper.FromSuccess("data");
        var chained = wrapper.WithId("id").WithCode(201).WithMessage("msg");

        Assert.That(chained, Is.SameAs(wrapper));
    }

    #endregion

    #region Implicit casts

    [Test]
    public void ImplicitCast_FromException_ShouldCreateErrorWrapper()
    {
        var exception = new Exception("cast error");

        Wrapper wrapper = exception;

        Assert.That(wrapper.Message, Is.EqualTo("cast error"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void ImplicitCast_FromExceptionWithInt_ShouldSetCode()
    {
        var exception = new Exception("bad request");

        Wrapper wrapper = (exception, 400);

        Assert.That(wrapper.Message, Is.EqualTo("bad request"));
        Assert.That(wrapper.Code, Is.EqualTo(400));
    }

    [Test]
    public void ImplicitCast_FromExceptionWithHttpStatusCode_ShouldSetCode()
    {
        var exception = new Exception("not found");

        Wrapper wrapper = (exception, HttpStatusCode.NotFound);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public void ImplicitCast_FromInt_ShouldCreateStatusWrapper()
    {
        Wrapper wrapper = 204;

        Assert.That(wrapper.Code, Is.EqualTo(204));
        Assert.Null(wrapper.Content);
        Assert.Null(wrapper.Message);
    }

    [Test]
    public void ImplicitCast_FromHttpStatusCode_ShouldCreateStatusWrapper()
    {
        Wrapper wrapper = HttpStatusCode.NotFound;

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public void ImplicitCast_FromTupleWithInt_ShouldCreateSuccessWrapper()
    {
        Wrapper wrapper = ("payload", 201);

        Assert.That(wrapper.Content, Is.EqualTo("payload"));
        Assert.That(wrapper.Code, Is.EqualTo(201));
    }

    [Test]
    public void ImplicitCast_FromTupleWithHttpStatusCode_ShouldCreateSuccessWrapper()
    {
        Wrapper wrapper = ("payload", HttpStatusCode.Created);

        Assert.That(wrapper.Content, Is.EqualTo("payload"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Created));
    }

    #endregion

    #region FromSuccessOrNotFound

    [Test]
    public void FromSuccessOrNotFound_WhenNull_ShouldReturn404()
    {
        var wrapper = Wrapper.FromSuccessOrNotFound(null);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void FromSuccessOrNotFound_WhenNotNull_ShouldReturn200WithContent()
    {
        var wrapper = Wrapper.FromSuccessOrNotFound("data");

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(wrapper.Content, Is.EqualTo("data"));
    }

    #endregion

    #region ModelState extension

    [Test]
    public void ModelState_ToWrapper_ShouldReturn400WithErrors()
    {
        var dict = new ModelStateDictionary();
        dict.AddModelError("email", "Invalid format");

        var wrapper = dict.ToWrapper();

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.IsNotNull(wrapper.Content);
    }

    [Test]
    public void ModelState_ToWrapper_WithMessage_ShouldSetMessage()
    {
        var dict = new ModelStateDictionary();
        dict.AddModelError("field", "Error");

        var wrapper = dict.ToWrapper("Validation failed");

        Assert.That(wrapper.Message, Is.EqualTo("Validation failed"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public void ModelState_ToWrapper_WithException_ShouldUseExceptionMessage()
    {
        var dict = new ModelStateDictionary();
        dict.AddModelError("name", "Required");

        var wrapper = dict.ToWrapper(new Exception("Binding error"));

        Assert.That(wrapper.Message, Is.EqualTo("Binding error"));
    }

    [Test]
    public void ModelState_ToWrapper_WithCustomCode_ShouldSetCode()
    {
        var dict = new ModelStateDictionary();
        dict.AddModelError("field", "Error");

        var wrapper = dict.ToWrapper("err", 422);

        Assert.That(wrapper.Code, Is.EqualTo(422));
    }

    #endregion

    #region Deconstruct

    [Test]
    public void Deconstruct_ThreeParams_ShouldReturnAllFields()
    {
        var wrapper = Wrapper.FromError(new Exception("err"), HttpStatusCode.BadRequest);

        var (content, code, message) = wrapper;

        Assert.That(code, Is.EqualTo((int)HttpStatusCode.BadRequest));
        Assert.That(message, Is.EqualTo("err"));
        Assert.Null(content);
    }

    [Test]
    public void Deconstruct_TwoParams_ShouldReturnContentAndCode()
    {
        var wrapper = Wrapper.FromSuccess("data", HttpStatusCode.Created);

        var (content, code) = wrapper;

        Assert.That(code, Is.EqualTo((int)HttpStatusCode.Created));
        Assert.That(content, Is.EqualTo("data"));
    }

    #endregion

    #region Serialization

    [Test]
    public void Wrapper_MustBe_Serializable()
    {
        var wrapper = Wrapper.FromSuccess("content", HttpStatusCode.OK);

        var json = JsonSerializer.Serialize(wrapper);

        Assert.IsNotNull(json);
        Assert.That(json, Does.Contain("\"id\""));
        Assert.That(json, Does.Contain("\"code\""));
        Assert.That(json, Does.Contain("\"content\""));
        Assert.That(json, Does.Contain("\"message\""));
    }

    [Test]
    public void Wrapper_Serialized_ShouldRoundtrip()
    {
        var original = Wrapper.FromSuccess("hello").WithId("round-trip-id");

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Wrapper>(json);

        Assert.IsNotNull(deserialized);
        Assert.That(deserialized!.Id, Is.EqualTo("round-trip-id"));
        Assert.That(deserialized.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(deserialized.Content?.ToString(), Is.EqualTo("hello"));
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
        var wrapper = JsonSerializer.Deserialize<Wrapper>(rawData);

        Assert.IsNotNull(wrapper);
        Assert.IsNotNull(wrapper.Content);
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.IsNull(wrapper.Message);
        Assert.That(wrapper.Id, Is.EqualTo("463fadea-066e-4950-a733-5bc789a9ea94"));
    }

    #endregion
}
