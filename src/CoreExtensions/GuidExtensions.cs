namespace System
{
    /// <summary>
    /// Guid extensions.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this Guid value)
          => value == Guid.Empty;
        
    }
}