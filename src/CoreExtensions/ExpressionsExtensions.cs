using System.Linq.Expressions;

namespace System
{
    public static class ExpressionsExtensions
    {
        public static string GetName<TObject, TMember>(this Expression<Func<TObject, TMember>> expression)
        {
            MemberExpression body = expression.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public static Expression<Func<TObject, object>> ToPropertyExpression<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertyExpression<TObject, object>(propertyName);
        }

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

        public static Func<TObject, object> ToPropertySelector<TObject>(this TObject obj, string propertyName)
        {
            return obj.ToPropertySelector<TObject, object>(propertyName);
        }

        public static Func<TObject, TProp> ToPropertySelector<TObject, TProp>(this TObject obj, string propertyName)
        {
            var exp = obj.ToPropertyExpression<TObject, TProp>(propertyName);

            return
            exp.Compile();
        }
    }
}