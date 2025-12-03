namespace System
{
    /// <summary>
    /// Provides extension methods for <see cref="Guid"/> operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides utilities for working with <see cref="Guid"/> values,
    /// including checking for empty (default) GUIDs.
    /// </para>
    /// </remarks>
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="Guid"/> is empty (equal to <see cref="Guid.Empty"/>).
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> to check.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Guid"/> equals <see cref="Guid.Empty"/> (all zeros); otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// A <see cref="Guid"/> is considered empty when it equals <see cref="Guid.Empty"/>,
        /// which is the default value for a <see cref="Guid"/> (00000000-0000-0000-0000-000000000000).
        /// </para>
        /// <para>
        /// This is useful for validating that a GUID has been properly initialized or assigned.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var emptyGuid = Guid.Empty;
        /// var newGuid = Guid.NewGuid();
        ///
        /// emptyGuid.IsEmpty(); // true
        /// newGuid.IsEmpty();   // false
        ///
        /// // Common validation pattern
        /// if (userId.IsEmpty())
        /// {
        ///     throw new ArgumentException("User ID cannot be empty");
        /// }
        /// </code>
        /// </example>
        public static bool IsEmpty(this Guid value)
          => value == Guid.Empty;
    }
}