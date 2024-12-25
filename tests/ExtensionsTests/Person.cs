namespace ExtensionsTests
{
    public class Person
    {
        public int ID;
        public string FirstName { get; set; }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string LastName { get; set; }
    }
}
