using System.Linq;

namespace System
{
    public static class EnumExtensions
    {
        //todo: should be tested
        internal static TEnum ToEnum<TEnum>(this string value)
            where TEnum : struct
        {
            value.GuardAgainstNullOrEmpty(nameof(value));
            return 
            (TEnum)Enum.Parse(typeof(TEnum), value, true);
        }

        public static TEnum ToEnum<TEnum>(this string value,TEnum defaultValue)
            where TEnum : struct
        {
            return 
            Enum.TryParse(value,true, out TEnum result )
            ? result
            : defaultValue;
        }

        public static Array GetValues<TEnum>(this TEnum value)
            where TEnum : struct
        {
            value.GuardAgainst(v => !v.GetType().IsEnum, $"the given value [{value}] is not an enum");
            
            return
            Enum.GetValues(typeof(TEnum));
        }

        public static string[] GetNames<TEnum>(this TEnum value)
        where TEnum : struct
        {
            value.GuardAgainst(v => !v.GetType().IsEnum, $"the given value [{value}] is not an enum");
            
            return
            Enum.GetNames(typeof(TEnum));
        }

        public static (string Name, int Value)[] ToNameValuePaires<TEnum>(this TEnum value)
        where TEnum : struct
        {            
            var names = value.GetNames();
            
            return
            value.GetValues().Cast<int>()
               .Select((v,index)=> (names[index], v))
               .ToArray();
        }


        public static int GetValue<TEnum>(this TEnum value, string name)
        where TEnum : struct
        {
            value.GuardAgainst(v => !v.GetType().IsEnum, $"the given value [{value}] is not an enum");
            name.GuardAgainstNullOrEmpty(nameof(name));

            var valueNames = value.ToNameValuePaires();

            return
                valueNames
                .Single(nv => nv.Name.EqualsIgnoreCaseOrdinal(name))
                .Value;
        }

        public static string GetName<TEnum>(this TEnum @this, int value)
        where TEnum : struct
        {
            @this.GuardAgainst(v => !v.GetType().IsEnum, $"the given value [{value}] is not an enum");

            var valueNames = value.ToNameValuePaires();

            return
                valueNames
                .Single(nv => nv.Value.Equals(value))
                .Name;
        }


    }
}
