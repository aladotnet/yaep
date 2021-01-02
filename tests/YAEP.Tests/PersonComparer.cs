using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YAEP.Tests
{
    public class PersonComparer : IEqualityComparer<Person>
    {

        public bool Equals([AllowNull] Person x, [AllowNull] Person y)
        {
            if (y == null && x == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.FirstName == y.FirstName && x.LastName == y.LastName)
                return true;
            else
                return false;
        }

        public int GetHashCode([DisallowNull] Person obj)
        {
            int hash = 13;
            hash = (hash * 7) + obj.FirstName.GetHashCode();
            hash = (hash * 7) + obj.LastName.GetHashCode();           
            return hash;
        }
    }
}
