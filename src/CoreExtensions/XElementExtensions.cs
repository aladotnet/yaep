namespace System.Xml.Linq;

/// <summary>
/// Provides extension methods for <see cref="XElement"/> operations including attribute access, element traversal, and fluent modification.
/// </summary>
/// <remarks>
/// <para>
/// This class provides utilities for working with LINQ to XML <see cref="XElement"/> instances:
/// </para>
/// <list type="bullet">
/// <item><description>Getting attribute values with default fallbacks</description></item>
/// <item><description>Comparing attribute values (case-insensitive)</description></item>
/// <item><description>Fluent element modification (setting name and value)</description></item>
/// <item><description>Finding root parent elements</description></item>
/// <item><description>Comparing local names</description></item>
/// </list>
/// </remarks>
public static class XElementExtensions
{
    /// <summary>
    /// Gets the root ancestor of the specified element (the ancestor with no parent).
    /// </summary>
    /// <param name="element">The element to find the root parent for.</param>
    /// <returns>
    /// The root ancestor <see cref="XElement"/> that has no parent, or <c>null</c> if the element is null or has no ancestors.
    /// </returns>
    /// <remarks>
    /// The root parent is defined as the ancestor that does not have a parent element.
    /// If the element itself has no parent, it may return <c>null</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var xml = XElement.Parse("&lt;root&gt;&lt;parent&gt;&lt;child/&gt;&lt;/parent&gt;&lt;/root&gt;");
    /// var child = xml.Descendants("child").First();
    /// var root = child.RootParent(); // Returns the "root" element
    /// </code>
    /// </example>
    public static XElement? RootParent(this XElement element)
      => element?.Ancestors()
         ?.DefaultIfEmpty()
         ?.OfType<XElement>()
         ?.FirstOrDefault(x => x.Parent.IsNull());

    /// <summary>
    /// Gets the value of the specified attribute, or a default value if the attribute doesn't exist.
    /// </summary>
    /// <param name="xelement">The element to get the attribute from.</param>
    /// <param name="attributeName">The name of the attribute.</param>
    /// <param name="defaultValue">The value to return if the attribute doesn't exist. Defaults to an empty string.</param>
    /// <returns>
    /// The attribute value if the attribute exists; otherwise, <paramref name="defaultValue"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="xelement"/> or <paramref name="attributeName"/> is null or empty.</exception>
    /// <example>
    /// <code>
    /// var element = XElement.Parse("&lt;item id=\"123\" name=\"test\"/&gt;");
    /// var id = element.GetAttributeValue("id");           // "123"
    /// var missing = element.GetAttributeValue("foo", "N/A"); // "N/A"
    /// </code>
    /// </example>
    public static string GetAttributeValue(this XElement xelement, string attributeName, string defaultValue = "")
    {
        attributeName.GuardAgainstNullOrEmpty(nameof(attributeName));

        return
        xelement
        .GuardAgainstNull(nameof(xelement))
        .Attribute(attributeName)?.Value ?? defaultValue;
    }

    /// <summary>
    /// Determines whether the specified attribute's value equals the given value (case-insensitive, invariant culture).
    /// </summary>
    /// <param name="element">The element containing the attribute.</param>
    /// <param name="attributeName">The name of the attribute to compare.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>
    /// <c>true</c> if the attribute exists and its value equals <paramref name="value"/> (case-insensitive); otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Uses invariant culture for case-insensitive comparison. Returns <c>false</c> if the element is null
    /// or the attribute doesn't exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// var element = XElement.Parse("&lt;item status=\"Active\"/&gt;");
    /// bool isActive = element.AttributeValueEquals("status", "active"); // true (case-insensitive)
    /// </code>
    /// </example>
    public static bool AttributeValueEquals(this XElement element, string attributeName, string value)
    {
        return
        element?.Attribute(attributeName)?.Value?.EqualsIgnoreCaseInvariant(value) ?? false;
    }

    /// <summary>
    /// Determines whether the specified attribute's value contains the given substring.
    /// </summary>
    /// <param name="element">The element containing the attribute.</param>
    /// <param name="attributeName">The name of the attribute to check.</param>
    /// <param name="value">The substring to search for.</param>
    /// <returns>
    /// <c>true</c> if the attribute exists and its value contains <paramref name="value"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The comparison is case-sensitive. Returns <c>false</c> if the element is null or the attribute doesn't exist.
    /// </remarks>
    /// <example>
    /// <code>
    /// var element = XElement.Parse("&lt;item class=\"btn btn-primary\"/&gt;");
    /// bool hasBtn = element.AttributeValueContains("class", "btn"); // true
    /// </code>
    /// </example>
    public static bool AttributeValueContains(this XElement element, string attributeName, string value)
    {
        return
        element?.Attribute(attributeName)?.Value?.Contains(value) ?? false;
    }

    /// <summary>
    /// Sets the name of the element and returns the element for fluent chaining.
    /// </summary>
    /// <param name="element">The element to modify.</param>
    /// <param name="name">The new name for the element.</param>
    /// <returns>The same <see cref="XElement"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="name"/> is null.</exception>
    /// <example>
    /// <code>
    /// var element = new XElement("old")
    ///     .WithName("new")
    ///     .WithValue("content");
    /// </code>
    /// </example>
    public static XElement WithName(this XElement element, string name)
    {
        element.GuardAgainstNull(nameof(element))
               .Name = name.GuardAgainstNull(nameof(name));

        return element;
    }

    /// <summary>
    /// Sets the value of the element and returns the element for fluent chaining.
    /// </summary>
    /// <param name="element">The element to modify.</param>
    /// <param name="value">The new value for the element.</param>
    /// <returns>The same <see cref="XElement"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> or <paramref name="value"/> is null.</exception>
    /// <example>
    /// <code>
    /// var element = new XElement("item")
    ///     .WithValue("Hello World");
    /// </code>
    /// </example>
    public static XElement WithValue(this XElement element, string value)
    {
        element.GuardAgainstNull(nameof(element))
               .Value = value.GuardAgainstNull(nameof(value));

        return element;
    }

    /// <summary>
    /// Determines whether the element's local name (without namespace) equals the specified value.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <param name="value">The local name to compare against.</param>
    /// <param name="ignoreCase">If <c>true</c>, performs case-insensitive comparison; otherwise, case-sensitive. Defaults to <c>true</c>.</param>
    /// <returns>
    /// <c>true</c> if the element's local name equals <paramref name="value"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> is null.</exception>
    /// <remarks>
    /// The local name is the element name without the namespace prefix.
    /// </remarks>
    /// <example>
    /// <code>
    /// var element = XElement.Parse("&lt;ns:Item xmlns:ns=\"http://example.com\"/&gt;");
    /// bool isItem = element.LocalNameEquals("item");        // true (case-insensitive by default)
    /// bool exact = element.LocalNameEquals("Item", false);  // true (exact case)
    /// </code>
    /// </example>
    public static bool LocalNameEquals(this XElement element, string value, bool ignoreCase = true)
    {
        element.GuardAgainstNull(nameof(element));

        return
        ignoreCase
            ? element.Name.LocalName.EqualsIgnoreCaseOrdinal(value)
            : element.Name.LocalName.Equals(value);
    }
}
