namespace System.Xml.Linq;

/// <summary>
/// XElement xml linq extensions.
/// </summary>
public static class XElementExtensions
{
    /// <summary>
    /// Gets the root parnet of the given element.
    /// root parent is an ancestor witch does not have a parent.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// XElement
    /// </returns>
    public static XElement? RootParent(this XElement element)
      => element?.Ancestors()
         ?.DefaultIfEmpty()
         ?.OfType<XElement>()
         ?.FirstOrDefault(x=> x.Parent.IsNull());
    
    /// <summary>
    /// Gets the attribute value.
    /// </summary>
    /// <param name="xelement">The xelement.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>returns the attribute value if not null otherwise returns the given defaulValue.</returns>
    public static string GetAttributeValue(this XElement xelement, string attributeName, string defaultValue = "")
    {
        attributeName.GuardAgainstNullOrEmpty(nameof(attributeName));

        return
        xelement
        .GuardAgainstNull(nameof(xelement))
        .Attribute(attributeName)?.Value ?? defaultValue;
    }

    /// <summary>
    /// Gets whether the Attribute value equals the given value or not.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if the values are equal and false if not.</returns>
    public static bool AttributeValueEquals(this XElement element, string attributeName, string value)
    {
        return
        element?.Attribute(attributeName)?.Value == value;
    }

    /// <summary>
    /// Gets whether the attribute value contains the given value otr not.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static bool AttributeValueContains(this XElement element, string attributeName, string value)
    {
        return
        element?.Attribute(attributeName)?.Value?.Contains(value) ?? false;
    }

    /// <summary>
    /// sets the element Name.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public static XElement WithName(this XElement element, string name)
    {
        element.GuardAgainstNull(nameof(element))
               .Name = name.GuardAgainstNull(nameof(name));

        return element;
    }

    /// <summary>
    /// sets the element value.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static XElement WithValue(this XElement element, string value)
    {
        element.GuardAgainstNull(nameof(element))
               .Value = value.GuardAgainstNull(nameof(value));

        return element;
    }

    /// <summary>
    /// Gets whether the LocalName equals the given value or not.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="value">The value.</param>
    /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
    /// <returns>true if equals, false if not.</returns>
    public static bool LocalNameEquals(this XElement element, string value, bool ignoreCase = true)
    {
        element.GuardAgainstNull(nameof(element));

        return
        ignoreCase
            ? element.Name.LocalName.EqualsIgnoreCaseOrdinal(value)
            : element.Name.LocalName.Equals(value);
    }
}