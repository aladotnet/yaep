using FluentAssertions;
using System.Text;

namespace ExtensionsTests;

/// <summary>
/// Tests for StringBuilderExtensions methods.
/// </summary>
public class StringBuilderExtensionsTests
{
    #region AppendIf

    /// <summary>
    /// Path: Predicate returns true -> Append is called -> text is appended.
    /// </summary>
    [Fact]
    public void AppendIf_WhenPredicateTrue_AppendsText()
    {
        var sb = new StringBuilder("Hello");

        sb.AppendIf(() => true, " World");

        sb.ToString().Should().Be("Hello World");
    }

    /// <summary>
    /// Path: Predicate returns false -> Append is not called -> original text unchanged.
    /// </summary>
    [Fact]
    public void AppendIf_WhenPredicateFalse_DoesNotAppend()
    {
        var sb = new StringBuilder("Hello");

        sb.AppendIf(() => false, " World");

        sb.ToString().Should().Be("Hello");
    }

    /// <summary>
    /// Path: Null predicate -> guard fails -> throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void AppendIf_WhenPredicateNull_ThrowsArgumentNullException()
    {
        var sb = new StringBuilder("Hello");

        var act = () => sb.AppendIf(null!, "text");

        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region AppendLineIf with text

    /// <summary>
    /// Path: Predicate returns true -> AppendLine is called -> text with newline is appended.
    /// </summary>
    [Fact]
    public void AppendLineIf_WithText_WhenPredicateTrue_AppendsLineWithText()
    {
        var sb = new StringBuilder("Line1");

        sb.AppendLineIf(() => true, "Line2");

        sb.ToString().Should().Be("Line1Line2" + Environment.NewLine);
    }

    /// <summary>
    /// Path: Predicate returns false -> AppendLine is not called -> original text unchanged.
    /// </summary>
    [Fact]
    public void AppendLineIf_WithText_WhenPredicateFalse_DoesNotAppendLine()
    {
        var sb = new StringBuilder("Line1");

        sb.AppendLineIf(() => false, "Line2");

        sb.ToString().Should().Be("Line1");
    }

    #endregion

    #region AppendLineIf without text

    /// <summary>
    /// Path: Predicate returns true -> AppendLine is called -> newline is appended.
    /// </summary>
    [Fact]
    public void AppendLineIf_WithoutText_WhenPredicateTrue_AppendsNewLine()
    {
        var sb = new StringBuilder("Text");

        sb.AppendLineIf(() => true);

        sb.ToString().Should().Be("Text" + Environment.NewLine);
    }

    /// <summary>
    /// Path: Predicate returns false -> AppendLine is not called -> no newline added.
    /// </summary>
    [Fact]
    public void AppendLineIf_WithoutText_WhenPredicateFalse_DoesNotAppendNewLine()
    {
        var sb = new StringBuilder("Text");

        sb.AppendLineIf(() => false);

        sb.ToString().Should().Be("Text");
    }

    #endregion

    #region AppendJoinIf with char separator

    /// <summary>
    /// Path: Predicate returns true -> AppendJoin with char separator is called -> values joined.
    /// </summary>
    [Fact]
    public void AppendJoinIf_CharSeparator_WhenPredicateTrue_JoinsValues()
    {
        var sb = new StringBuilder();

        sb.AppendJoinIf(() => true, ',', new[] { "a", "b", "c" });

        sb.ToString().Should().Be("a,b,c");
    }

    /// <summary>
    /// Path: Predicate returns false -> AppendJoin is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void AppendJoinIf_CharSeparator_WhenPredicateFalse_DoesNotJoin()
    {
        var sb = new StringBuilder("prefix");

        sb.AppendJoinIf(() => false, ',', new[] { "a", "b", "c" });

        sb.ToString().Should().Be("prefix");
    }

    #endregion

    #region AppendJoinIf with string separator

    /// <summary>
    /// Path: Predicate returns true -> AppendJoin with string separator is called -> values joined.
    /// </summary>
    [Fact]
    public void AppendJoinIf_StringSeparator_WhenPredicateTrue_JoinsValues()
    {
        var sb = new StringBuilder();

        sb.AppendJoinIf(() => true, " - ", new[] { 1, 2, 3 });

        sb.ToString().Should().Be("1 - 2 - 3");
    }

    /// <summary>
    /// Path: Predicate returns false -> AppendJoin is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void AppendJoinIf_StringSeparator_WhenPredicateFalse_DoesNotJoin()
    {
        var sb = new StringBuilder();

        sb.AppendJoinIf(() => false, " - ", new[] { 1, 2, 3 });

        sb.ToString().Should().BeEmpty();
    }

    #endregion

    #region ClearIf

    /// <summary>
    /// Path: Predicate returns true -> Clear is called -> builder is emptied.
    /// </summary>
    [Fact]
    public void ClearIf_WhenPredicateTrue_ClearsBuilder()
    {
        var sb = new StringBuilder("Some content");

        sb.ClearIf(() => true);

        sb.ToString().Should().BeEmpty();
    }

    /// <summary>
    /// Path: Predicate returns false -> Clear is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void ClearIf_WhenPredicateFalse_DoesNotClear()
    {
        var sb = new StringBuilder("Some content");

        sb.ClearIf(() => false);

        sb.ToString().Should().Be("Some content");
    }

    #endregion

    #region RemoveIf

    /// <summary>
    /// Path: Predicate returns true -> Remove is called -> characters removed.
    /// </summary>
    [Fact]
    public void RemoveIf_WhenPredicateTrue_RemovesCharacters()
    {
        var sb = new StringBuilder("Hello World");

        sb.RemoveIf(() => true, 5, 6); // Remove " World"

        sb.ToString().Should().Be("Hello");
    }

    /// <summary>
    /// Path: Predicate returns false -> Remove is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void RemoveIf_WhenPredicateFalse_DoesNotRemove()
    {
        var sb = new StringBuilder("Hello World");

        sb.RemoveIf(() => false, 5, 6);

        sb.ToString().Should().Be("Hello World");
    }

    #endregion

    #region ReplaceIf without index

    /// <summary>
    /// Path: Predicate returns true -> Replace is called -> text replaced.
    /// </summary>
    [Fact]
    public void ReplaceIf_WhenPredicateTrue_ReplacesText()
    {
        var sb = new StringBuilder("Hello World");

        sb.ReplaceIf(() => true, "World", "Universe");

        sb.ToString().Should().Be("Hello Universe");
    }

    /// <summary>
    /// Path: Predicate returns false -> Replace is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void ReplaceIf_WhenPredicateFalse_DoesNotReplace()
    {
        var sb = new StringBuilder("Hello World");

        sb.ReplaceIf(() => false, "World", "Universe");

        sb.ToString().Should().Be("Hello World");
    }

    #endregion

    #region ReplaceIf with index

    /// <summary>
    /// Path: Predicate returns true -> Replace with range is called -> text replaced in range.
    /// </summary>
    [Fact]
    public void ReplaceIf_WithIndex_WhenPredicateTrue_ReplacesInRange()
    {
        var sb = new StringBuilder("Hello World World");

        sb.ReplaceIf(() => true, "World", "Universe", 0, 11); // Only first "World"

        sb.ToString().Should().Be("Hello Universe World");
    }

    /// <summary>
    /// Path: Predicate returns false -> Replace is not called -> builder unchanged.
    /// </summary>
    [Fact]
    public void ReplaceIf_WithIndex_WhenPredicateFalse_DoesNotReplace()
    {
        var sb = new StringBuilder("Hello World World");

        sb.ReplaceIf(() => false, "World", "Universe", 0, 11);

        sb.ToString().Should().Be("Hello World World");
    }

    #endregion

    #region ToStringBuilder

    /// <summary>
    /// Path: String value -> creates new StringBuilder -> returns StringBuilder with value.
    /// </summary>
    [Fact]
    public void ToStringBuilder_GivenString_ReturnsStringBuilderWithValue()
    {
        var value = "Hello World";

        var result = value.ToStringBuilder();

        result.Should().NotBeNull();
        result.ToString().Should().Be("Hello World");
    }

    /// <summary>
    /// Path: Empty string -> creates new StringBuilder -> returns empty StringBuilder.
    /// </summary>
    [Fact]
    public void ToStringBuilder_GivenEmptyString_ReturnsEmptyStringBuilder()
    {
        var value = "";

        var result = value.ToStringBuilder();

        result.Should().NotBeNull();
        result.ToString().Should().BeEmpty();
    }

    #endregion
}
