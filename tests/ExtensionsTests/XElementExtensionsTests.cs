using FluentAssertions;
using System.Xml.Linq;

namespace ExtensionsTests;

/// <summary>
/// Tests for XElementExtensions methods.
/// </summary>
public class XElementExtensionsTests
{
    #region RootParent

    /// <summary>
    /// Path: Element has ancestors -> traverses to root -> returns element with no parent.
    /// </summary>
    [Fact]
    public void RootParent_GivenElementWithAncestors_ReturnsRootElement()
    {
        var xml = @"<root><parent><child></child></parent></root>";
        var doc = XElement.Parse(xml);
        var child = doc.Descendants("child").Single();

        var result = child.RootParent();

        result!.Name.LocalName.Should().Be("root");
    }

    /// <summary>
    /// Path: Element has no ancestors (is root) -> no parent found -> returns null.
    /// </summary>
    [Fact]
    public void RootParent_GivenRootElement_ReturnsNull()
    {
        var xml = @"<root><parent><child></child></parent></root>";
        var doc = XElement.Parse(xml);

        var result = doc.RootParent();

        result.Should().BeNull();
    }

    /// <summary>
    /// Path: Element is deeply nested -> traverses multiple levels -> returns root.
    /// </summary>
    [Fact]
    public void RootParent_GivenDeeplyNestedElement_ReturnsRootElement()
    {
        var xml = @"<root><level1><level2><level3><target/></level3></level2></level1></root>";
        var doc = XElement.Parse(xml);
        var target = doc.Descendants("target").Single();

        var result = target.RootParent();

        result!.Name.LocalName.Should().Be("root");
    }

    #endregion

    #region GetAttributeValue

    /// <summary>
    /// Path: Attribute exists -> returns attribute value.
    /// </summary>
    [Fact]
    public void GetAttributeValue_GivenExistingAttribute_ReturnsValue()
    {
        var element = new XElement("test", new XAttribute("id", "123"));

        var result = element.GetAttributeValue("id");

        result.Should().Be("123");
    }

    /// <summary>
    /// Path: Attribute does not exist -> returns default value.
    /// </summary>
    [Fact]
    public void GetAttributeValue_GivenMissingAttribute_ReturnsDefaultValue()
    {
        var element = new XElement("test");

        var result = element.GetAttributeValue("missing", "default");

        result.Should().Be("default");
    }

    /// <summary>
    /// Path: Attribute does not exist, no default specified -> returns empty string.
    /// </summary>
    [Fact]
    public void GetAttributeValue_GivenMissingAttributeNoDefault_ReturnsEmptyString()
    {
        var element = new XElement("test");

        var result = element.GetAttributeValue("missing");

        result.Should().BeEmpty();
    }

    /// <summary>
    /// Path: Null attribute name -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetAttributeValue_GivenNullAttributeName_ThrowsArgumentNullException()
    {
        var element = new XElement("test");

        var act = () => element.GetAttributeValue(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Path: Null element -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetAttributeValue_GivenNullElement_ThrowsArgumentNullException()
    {
        XElement? element = null;

        var act = () => element!.GetAttributeValue("attr");

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AttributeValueEquals

    /// <summary>
    /// Path: Attribute exists and value matches (case insensitive) -> returns true.
    /// </summary>
    [Fact]
    public void AttributeValueEquals_GivenMatchingValue_ReturnsTrue()
    {
        var element = new XElement("test", new XAttribute("type", "Active"));

        var result = element.AttributeValueEquals("type", "ACTIVE");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Attribute exists but value differs -> returns false.
    /// </summary>
    [Fact]
    public void AttributeValueEquals_GivenDifferentValue_ReturnsFalse()
    {
        var element = new XElement("test", new XAttribute("type", "Active"));

        var result = element.AttributeValueEquals("type", "Inactive");

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Attribute does not exist -> returns false.
    /// </summary>
    [Fact]
    public void AttributeValueEquals_GivenMissingAttribute_ReturnsFalse()
    {
        var element = new XElement("test");

        var result = element.AttributeValueEquals("missing", "value");

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Null element -> null propagation -> returns false.
    /// </summary>
    [Fact]
    public void AttributeValueEquals_GivenNullElement_ReturnsFalse()
    {
        XElement? element = null;

        var result = element.AttributeValueEquals("attr", "value");

        result.Should().BeFalse();
    }

    #endregion

    #region AttributeValueContains

    /// <summary>
    /// Path: Attribute value contains substring -> returns true.
    /// </summary>
    [Fact]
    public void AttributeValueContains_GivenMatchingSubstring_ReturnsTrue()
    {
        var element = new XElement("test", new XAttribute("description", "Hello World"));

        var result = element.AttributeValueContains("description", "World");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Attribute value does not contain substring -> returns false.
    /// </summary>
    [Fact]
    public void AttributeValueContains_GivenNonMatchingSubstring_ReturnsFalse()
    {
        var element = new XElement("test", new XAttribute("description", "Hello World"));

        var result = element.AttributeValueContains("description", "Universe");

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Attribute does not exist -> returns false.
    /// </summary>
    [Fact]
    public void AttributeValueContains_GivenMissingAttribute_ReturnsFalse()
    {
        var element = new XElement("test");

        var result = element.AttributeValueContains("missing", "value");

        result.Should().BeFalse();
    }

    #endregion

    #region WithName

    /// <summary>
    /// Path: Valid element and name -> sets name -> returns element with new name.
    /// </summary>
    [Fact]
    public void WithName_GivenValidName_SetsName()
    {
        var element = new XElement("original");

        var result = element.WithName("renamed");

        result.Name.LocalName.Should().Be("renamed");
        result.Should().BeSameAs(element);
    }

    /// <summary>
    /// Path: Null element -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void WithName_GivenNullElement_ThrowsArgumentNullException()
    {
        XElement? element = null;

        var act = () => element!.WithName("name");

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Path: Null name -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void WithName_GivenNullName_ThrowsArgumentNullException()
    {
        var element = new XElement("test");

        var act = () => element.WithName(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region WithValue

    /// <summary>
    /// Path: Valid element and value -> sets value -> returns element with new value.
    /// </summary>
    [Fact]
    public void WithValue_GivenValidValue_SetsValue()
    {
        var element = new XElement("test");

        var result = element.WithValue("new value");

        result.Value.Should().Be("new value");
        result.Should().BeSameAs(element);
    }

    /// <summary>
    /// Path: Null element -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void WithValue_GivenNullElement_ThrowsArgumentNullException()
    {
        XElement? element = null;

        var act = () => element!.WithValue("value");

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Path: Null value -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void WithValue_GivenNullValue_ThrowsArgumentNullException()
    {
        var element = new XElement("test");

        var act = () => element.WithValue(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region LocalNameEquals

    /// <summary>
    /// Path: LocalName matches (ignore case true, default) -> returns true.
    /// </summary>
    [Fact]
    public void LocalNameEquals_GivenMatchingNameIgnoreCase_ReturnsTrue()
    {
        var element = new XElement("TestElement");

        var result = element.LocalNameEquals("TESTELEMENT");

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: LocalName matches (ignore case false) -> exact match required -> returns true.
    /// </summary>
    [Fact]
    public void LocalNameEquals_GivenExactMatch_ReturnsTrue()
    {
        var element = new XElement("TestElement");

        var result = element.LocalNameEquals("TestElement", ignoreCase: false);

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: LocalName differs in case (ignore case false) -> returns false.
    /// </summary>
    [Fact]
    public void LocalNameEquals_GivenDifferentCaseNoIgnore_ReturnsFalse()
    {
        var element = new XElement("TestElement");

        var result = element.LocalNameEquals("TESTELEMENT", ignoreCase: false);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: LocalName does not match -> returns false.
    /// </summary>
    [Fact]
    public void LocalNameEquals_GivenDifferentName_ReturnsFalse()
    {
        var element = new XElement("TestElement");

        var result = element.LocalNameEquals("OtherElement");

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Null element -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void LocalNameEquals_GivenNullElement_ThrowsArgumentNullException()
    {
        XElement? element = null;

        var act = () => element!.LocalNameEquals("name");

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
