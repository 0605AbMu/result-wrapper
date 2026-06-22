using System.Net;
using System.Text.Json;
using ResultWrapper.Library;
using ResultWrapper.Library.Interfaces;

namespace ResultWrapper.Test;

public class ResultWrapperGenericTest
{
    [SetUp]
    public void Setup()
    {
    }

    #region FromSuccess

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
    public void FromSuccess_WithHttpStatusCode_ShouldSetCorrectCode()
    {
        var wrapper = Wrapper<int>.FromSuccess(42, HttpStatusCode.Created);

        Assert.That(wrapper.Content, Is.EqualTo(42));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Created));
    }

    #endregion

    #region FromError

    [Test]
    public void FromError_ShouldSetMessage_AndNullContent()
    {
        var exception = new Exception("generic error");

        var wrapper = Wrapper<string>.FromError(exception);

        Assert.That(wrapper.Message, Is.EqualTo("generic error"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void FromError_WithCode_ShouldSetCorrectCode()
    {
        var exception = new Exception("bad");

        var wrapper = Wrapper<string>.FromError(exception, HttpStatusCode.BadRequest);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public void FromError_WithString_ShouldSetMessage()
    {
        var wrapper = Wrapper<string>.FromError("something went wrong");

        Assert.That(wrapper.Message, Is.EqualTo("something went wrong"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    #endregion

    #region Fluent methods

    [Test]
    public void WithId_ShouldOverrideId()
    {
        var wrapper = Wrapper<string>.FromSuccess("data").WithId("my-id");

        Assert.That(wrapper.Id, Is.EqualTo("my-id"));
    }

    [Test]
    public void WithCode_Int_ShouldOverrideCode()
    {
        var wrapper = Wrapper<string>.FromSuccess("data").WithCode(202);

        Assert.That(wrapper.Code, Is.EqualTo(202));
    }

    [Test]
    public void WithCode_HttpStatusCode_ShouldOverrideCode()
    {
        var wrapper = Wrapper<string>.FromSuccess("data").WithCode(HttpStatusCode.Accepted);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Accepted));
    }

    [Test]
    public void WithMessage_ShouldOverrideMessage()
    {
        var wrapper = Wrapper<string>.FromSuccess("data").WithMessage("all good");

        Assert.That(wrapper.Message, Is.EqualTo("all good"));
    }

    [Test]
    public void FluentChain_ShouldReturnSameInstance()
    {
        var wrapper = Wrapper<string>.FromSuccess("data");
        var chained = wrapper.WithId("id").WithCode(201).WithMessage("msg");

        Assert.That(chained, Is.SameAs(wrapper));
    }

    #endregion

    #region Implicit casts

    [Test]
    public void ImplicitCast_FromException_ShouldCreateErrorWrapper()
    {
        var exception = new Exception("oops");

        Wrapper<string> wrapper = exception;

        Assert.That(wrapper.Message, Is.EqualTo("oops"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void ImplicitCast_FromExceptionWithInt_ShouldSetCode()
    {
        var exception = new Exception("bad");

        Wrapper<string> wrapper = (exception, 400);

        Assert.That(wrapper.Message, Is.EqualTo("bad"));
        Assert.That(wrapper.Code, Is.EqualTo(400));
    }

    [Test]
    public void ImplicitCast_FromExceptionWithHttpStatusCode_ShouldSetCode()
    {
        var exception = new Exception("unauth");

        Wrapper<string> wrapper = (exception, HttpStatusCode.Unauthorized);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Unauthorized));
    }

    [Test]
    public void ImplicitCast_FromInt_ShouldCreateStatusWrapper()
    {
        Wrapper<string> wrapper = 204;

        Assert.That(wrapper.Code, Is.EqualTo(204));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void ImplicitCast_FromHttpStatusCode_ShouldCreateStatusWrapper()
    {
        Wrapper<string> wrapper = HttpStatusCode.Forbidden;

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Forbidden));
    }

    [Test]
    public void ImplicitCast_FromTupleWithInt_ShouldCreateSuccessWrapper()
    {
        Wrapper<string> wrapper = ("result", 201);

        Assert.That(wrapper.Content, Is.EqualTo("result"));
        Assert.That(wrapper.Code, Is.EqualTo(201));
    }

    [Test]
    public void ImplicitCast_FromTupleWithHttpStatusCode_ShouldCreateSuccessWrapper()
    {
        Wrapper<string> wrapper = ("result", HttpStatusCode.Created);

        Assert.That(wrapper.Content, Is.EqualTo("result"));
        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.Created));
    }

    #endregion

    #region Cast to non-generic Wrapper

    [Test]
    public void WrapperGeneric_MustChangeable_WithItsInterface()
    {
        Wrapper<string> wrapper = "test";

        Assert.IsInstanceOf<Wrapper<string>>(wrapper);
    }

    [Test]
    public void WrapperGeneric_MustChangeable_WithItsNonGenericType()
    {
        Wrapper<string> wrapper = "test";

        Wrapper casted = wrapper;

        Assert.That(casted.Id, Is.EqualTo(wrapper.Id));
        Assert.That(casted.Content, Is.EqualTo(wrapper.Content));
        Assert.That(casted.Code, Is.EqualTo(wrapper.Code));
        Assert.That(casted.Message, Is.EqualTo(wrapper.Message));
    }

    [Test]
    public void CastToNonGeneric_ShouldPreserveAllFields()
    {
        var original = Wrapper<int>.FromSuccess(99, HttpStatusCode.Created)
            .WithId("preserve-id")
            .WithMessage("preserve-msg");

        Wrapper casted = original;

        Assert.That(casted.Id, Is.EqualTo("preserve-id"));
        Assert.That(casted.Code, Is.EqualTo((int)HttpStatusCode.Created));
        Assert.That(casted.Message, Is.EqualTo("preserve-msg"));
        Assert.That(casted.Content, Is.EqualTo(99));
    }

    #endregion

    #region FromSuccessOrNotFound

    [Test]
    public void FromSuccessOrNotFound_WhenNull_ShouldReturn404()
    {
        var wrapper = Wrapper<string>.FromSuccessOrNotFound(null);

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.NotFound));
        Assert.Null(wrapper.Content);
    }

    [Test]
    public void FromSuccessOrNotFound_WhenNotNull_ShouldReturn200WithContent()
    {
        var wrapper = Wrapper<string>.FromSuccessOrNotFound("found");

        Assert.That(wrapper.Code, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(wrapper.Content, Is.EqualTo("found"));
    }

    #endregion

    #region Deconstruct

    [Test]
    public void Deconstruct_ThreeParams_ShouldReturnAllFields()
    {
        var wrapper = Wrapper<string>.FromSuccess("hello", HttpStatusCode.Created)
            .WithMessage("ok");

        var (content, code, message) = wrapper;

        Assert.That(content, Is.EqualTo("hello"));
        Assert.That(code, Is.EqualTo((int)HttpStatusCode.Created));
        Assert.That(message, Is.EqualTo("ok"));
    }

    [Test]
    public void Deconstruct_TwoParams_ShouldReturnContentAndCode()
    {
        var wrapper = Wrapper<string>.FromSuccess("hello");

        var (content, code) = wrapper;

        Assert.That(content, Is.EqualTo("hello"));
        Assert.That(code, Is.EqualTo((int)HttpStatusCode.OK));
    }

    #endregion

    #region Serialization

    [Test]
    public void WrapperGeneric_MustBe_Serializable()
    {
        var wrapper = Wrapper<string>.FromSuccess("hello");

        var json = JsonSerializer.Serialize(wrapper);

        Assert.IsNotNull(json);
        Assert.That(json, Does.Contain("\"id\""));
        Assert.That(json, Does.Contain("\"code\""));
        Assert.That(json, Does.Contain("\"content\""));
        Assert.That(json, Does.Contain("\"message\""));
    }

    [Test]
    [TestCase(@"{
    ""id"": ""463fadea-066e-4950-a733-5bc789a9ea94"",
    ""code"": 200,
    ""content"": {
        ""name"": ""test"",
        ""thumb"": """",
        ""groupName"": null,
        ""isSystemDefined"": false,
        ""id"": 10
    },
    ""message"": null
}")]
    public void WrapperGeneric_MustBe_Deserializable(string rawData)
    {
        var wrapper = JsonSerializer.Deserialize<Wrapper<JsonElement>>(rawData);

        Assert.IsNotNull(wrapper);

        var content = wrapper.Content;
        Assert.True(content.ValueKind == JsonValueKind.Object);

        Assert.True(content.TryGetProperty("id", out var id));
        Assert.True(id.ValueKind == JsonValueKind.Number);
        Assert.That(id.GetInt64(), Is.EqualTo(10));

        Assert.True(content.TryGetProperty("name", out var name));
        Assert.True(name.ValueKind == JsonValueKind.String);
        Assert.That(name.GetString(), Is.EqualTo("test"));
    }

    #endregion
}
