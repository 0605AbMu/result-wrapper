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
    }
}