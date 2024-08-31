using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Expressions extension methods.
    /// </summary>
    public static class ExpressionsExtensions
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TMember">The type of the member.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>the name of the given member.</returns>
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
        /// Converts to propertyexpression.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Expression<Func<TObject, object>> ToPropertyExpression<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertyExpression<TObject, object>(propertyName);
        }

        /// <summary>
        /// Converts to propertyexpression.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
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
        /// Converts to propertyselector.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Func<TObject, object> ToPropertySelector<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertySelector<TObject, object>(propertyName);
        }

        /// <summary>
        /// Converts to propertyselector.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static Func<TObject, TProp> ToPropertySelector<TObject, TProp>(this TObject obj, string propertyName)
        {
            var exp = obj.ToPropertyExpression<TObject, TProp>(propertyName);

            return
            exp.Compile();
        }
    }
}