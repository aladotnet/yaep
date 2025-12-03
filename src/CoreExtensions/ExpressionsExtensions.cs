using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Provides extension methods for working with LINQ expressions and creating dynamic property accessors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides utilities for:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Extracting member names from lambda expressions</description></item>
    /// <item><description>Creating property expressions from property names at runtime</description></item>
    /// <item><description>Creating compiled property selectors for efficient property access</description></item>
    /// </list>
    /// <para>
    /// These methods are useful for dynamic property access scenarios, such as building dynamic LINQ queries,
    /// implementing generic data binding, or creating property-based sorting/filtering.
    /// </para>
    /// </remarks>
    public static class ExpressionsExtensions
    {
        /// <summary>
        /// Extracts the member name from a lambda expression.
        /// </summary>
        /// <typeparam name="TObject">The type of the object containing the member.</typeparam>
        /// <typeparam name="TMember">The type of the member being accessed.</typeparam>
        /// <param name="expression">A lambda expression that accesses a member (e.g., <c>x => x.PropertyName</c>).</param>
        /// <returns>The name of the member being accessed.</returns>
        /// <remarks>
        /// <para>
        /// This method handles both direct member access expressions and expressions wrapped in conversion operations
        /// (unary expressions), which commonly occur when the property type differs from the delegate's return type.
        /// </para>
        /// <para>
        /// This is useful for getting property names in a refactoring-safe way, avoiding magic strings.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Expression&lt;Func&lt;User, string&gt;&gt; expr = u => u.Name;
        /// string propName = expr.GetName(); // "Name"
        ///
        /// // Useful for creating property-based operations
        /// string sortColumn = ((Expression&lt;Func&lt;User, object&gt;&gt;)(u => u.CreatedDate)).GetName();
        /// </code>
        /// </example>
        public static string GetName<TObject, TMember>(this Expression<Func<TObject, TMember>> expression)
        {
            if (expression.Body is not MemberExpression body)
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = (MemberExpression)ubody.Operand;
            }

            return body.Member.Name;
        }

        /// <summary>
        /// Creates a property access expression from a property name at runtime.
        /// </summary>
        /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
        /// <param name="obj">An instance of the object type (used for type inference; the value is not used).</param>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>An expression that accesses the specified property and returns it as <see cref="object"/>.</returns>
        /// <remarks>
        /// This is a convenience overload that returns the property value boxed as <see cref="object"/>.
        /// For strongly-typed property access, use <see cref="ToPropertyExpression{TObject, TProp}"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User();
        /// var expression = user.ToPropertyExpression("Name");
        /// // expression is: u => (object)u.Name
        /// </code>
        /// </example>
        public static Expression<Func<TObject, object>> ToPropertyExpression<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertyExpression<TObject, object>(propertyName);
        }

        /// <summary>
        /// Creates a strongly-typed property access expression from a property name at runtime.
        /// </summary>
        /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
        /// <typeparam name="TProp">The type of the property being accessed.</typeparam>
        /// <param name="obj">An instance of the object type (used for type inference; the value is not used).</param>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>An expression that accesses the specified property.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null or empty.</exception>
        /// <remarks>
        /// <para>
        /// The returned expression can be used with LINQ providers (e.g., Entity Framework) for dynamic queries,
        /// or compiled into a delegate for efficient property access.
        /// </para>
        /// <para>
        /// The expression includes a type conversion to <typeparamref name="TProp"/>, allowing access to properties
        /// where the actual property type differs from the requested type.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User();
        /// var expression = user.ToPropertyExpression&lt;User, string&gt;("Name");
        /// // expression is: u => (string)u.Name
        ///
        /// // Use with Entity Framework
        /// var sortedUsers = context.Users.OrderBy(expression);
        /// </code>
        /// </example>
        public static Expression<Func<TObject, TProp>> ToPropertyExpression<TObject, TProp>(this TObject obj, string propertyName)
        {
            propertyName.GuardAgainstNullOrEmpty(nameof(propertyName));

            var arg = Expression.Parameter(typeof(TObject), "obj");
            var property = Expression.Property(arg, propertyName);
            //return the property as object
            var conv = Expression.Convert(property, typeof(TProp));
            var exp = Expression.Lambda<Func<TObject, TProp>>(conv, new ParameterExpression[] { arg });
            return exp;
        }

        /// <summary>
        /// Creates a compiled property selector delegate from a property name at runtime.
        /// </summary>
        /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
        /// <param name="obj">An instance of the object type (used for type inference; the value is not used).</param>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>A compiled delegate that retrieves the property value as <see cref="object"/>.</returns>
        /// <remarks>
        /// This is a convenience overload that returns the property value boxed as <see cref="object"/>.
        /// For strongly-typed property access, use <see cref="ToPropertySelector{TObject, TProp}"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User { Name = "John" };
        /// var selector = user.ToPropertySelector("Name");
        /// object name = selector(user); // "John"
        /// </code>
        /// </example>
        public static Func<TObject, object> ToPropertySelector<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertySelector<TObject, object>(propertyName);
        }

        /// <summary>
        /// Creates a strongly-typed compiled property selector delegate from a property name at runtime.
        /// </summary>
        /// <typeparam name="TObject">The type of the object containing the property.</typeparam>
        /// <typeparam name="TProp">The type of the property being accessed.</typeparam>
        /// <param name="obj">An instance of the object type (used for type inference; the value is not used).</param>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>A compiled delegate that retrieves the property value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null or empty.</exception>
        /// <remarks>
        /// <para>
        /// The delegate is compiled from an expression tree and provides efficient property access
        /// comparable to direct property access.
        /// </para>
        /// <para>
        /// For repeated access, consider caching the compiled delegate rather than calling this method each time.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var user = new User { Name = "John", Age = 30 };
        /// var nameSelector = user.ToPropertySelector&lt;User, string&gt;("Name");
        /// var ageSelector = user.ToPropertySelector&lt;User, int&gt;("Age");
        ///
        /// string name = nameSelector(user); // "John"
        /// int age = ageSelector(user);      // 30
        /// </code>
        /// </example>
        public static Func<TObject, TProp> ToPropertySelector<TObject, TProp>(this TObject obj, string propertyName)
        {
            var exp = obj.ToPropertyExpression<TObject, TProp>(propertyName);

            return
            exp.Compile();
        }
    }
}