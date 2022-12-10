namespace System.Xml.Linq
{
    public static class XElementExtensions
    {
        public static string GetAttributeValue(this XElement xelement, string attributeName, string defaultValue = "")
        {
            attributeName.GuardAgainstNullOrEmpty(nameof(attributeName));

            return
            xelement
            .GuardAgainstNull(nameof(xelement))
            .Attribute(attributeName)?.Value ?? defaultValue;
        }

        public static bool AttributeEquals(this XElement element, string attributeName, string value)
        {
            return
            element?.Attribute(attributeName)?.Value == value;
        }

        public static bool AttributeContains(this XElement element, string attributeName, string value)
        {
            return
            element?.Attribute(attributeName)?.Value?.Contains(value) ?? false;
        }

        public static XElement WithName(this XElement element, string name)
        {
            element.GuardAgainstNull(nameof(element))
                   .Name = name.GuardAgainstNull(nameof(name));

            return element;
        }

        public static XElement WithValue(this XElement element, string value)
        {
            element.GuardAgainstNull(nameof(element))
                   .Value = value.GuardAgainstNull(nameof(value));

            return element;
        }

        public static bool LocalNameEquals(this XElement element, string value, bool ignoreCase = true)
        {
            element.GuardAgainstNull(nameof(element));

            return
            ignoreCase
                ? element.Name.LocalName.EqualsIgnoreCaseOrdinal(value)
                : element.Name.LocalName.Equals(value);
        }
    }
}