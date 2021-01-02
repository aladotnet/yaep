namespace YAEP.Tests
{
    public class Person
    {
        public string FirstName { get; set; }

        public Person(string firstName, string lastName )
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string  LastName { get; set; }
    }
}
