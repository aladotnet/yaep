using FluentAssertions;

namespace ExtensionsTests;

/// <summary>
/// Tests for GuidExtensions methods.
/// </summary>
public class GuidExtensionsTests
{
    #region IsEmpty

    /// <summary>
    /// Path: Guid.Empty value -> equals Guid.Empty -> returns true.
    /// </summary>
    [Fact]
    public void IsEmpty_GivenEmptyGuid_ReturnsTrue()
    {
        var guid = Guid.Empty;

        var result = guid.IsEmpty();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Non-empty Guid value -> not equal to Guid.Empty -> returns false.
    /// </summary>
    [Fact]
    public void IsEmpty_GivenNonEmptyGuid_ReturnsFalse()
    {
        var guid = Guid.NewGuid();

        var result = guid.IsEmpty();

        result.Should().BeFalse();
    }

    /// <summary>
    /// Path: Default Guid (which is Guid.Empty) -> equals Guid.Empty -> returns true.
    /// </summary>
    [Fact]
    public void IsEmpty_GivenDefaultGuid_ReturnsTrue()
    {
        Guid guid = default;

        var result = guid.IsEmpty();

        result.Should().BeTrue();
    }

    /// <summary>
    /// Path: Specific non-empty Guid -> not equal to Guid.Empty -> returns false.
    /// </summary>
    [Fact]
    public void IsEmpty_GivenSpecificGuid_ReturnsFalse()
    {
        var guid = new Guid("12345678-1234-1234-1234-123456789012");

        var result = guid.IsEmpty();

        result.Should().BeFalse();
    }

    #endregion
}
