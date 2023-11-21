using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace YAEPTests
{
    public class CollectionExtensionsTests
    {

        [Fact]
        public void TryAdd_throwes_AgrgumentNullException()
        {
            List<int> list = null;

            Assert.Throws<ArgumentNullException>(() => list.TryAdd(4));
        }


        [Fact]
        public void TryAdd_Adds_new_entry_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3","Last3")
            };

            var added = list.TryAdd(new Person("first4", "Last4"));

            added.Should().BeTrue();
            list.Count.Should().Be(4);
            list.SingleOrDefault(p => p.FirstName == "first4").Should().NotBeNull();
        }

        [Fact]
        public void TryAdd_given_an_existing_entry_ignores_it_returns_false()
        {
            var existingEntry = new Person("first3", "Last3");
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                existingEntry
            };

            var added = list.TryAdd(existingEntry);

            added.Should().BeFalse();
            list.Count.Should().Be(3);
        }

        [Fact]
        public void TryAdd_with_comparer_Adds_new_entry_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3","Last3")
            };

            var added = list.TryAdd(new Person("first4", "Last4"), new PersonComparer());

            added.Should().BeTrue();
            list.Count.Should().Be(4);
            list.SingleOrDefault(p => p.FirstName == "first4").Should().NotBeNull();
        }

        [Fact]
        public void TryAdd_with_comparer_given_an_existing_entry_ignores_it_returns_false()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var existingEntry = new Person("first3", "Last3");
            var added = list.TryAdd(existingEntry, new PersonComparer());

            added.Should().BeFalse();
            list.Count.Should().Be(3);
        }

        [Fact]
        public void TryAdd_with_comparer_Func_Adds_new_entry_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3","Last3")
            };

            var added = list.TryAdd(new Person("first4", "Last4"), (p1, p2) => p1.FirstName == p2.FirstName
                                                            && p1.LastName == p2.LastName);

            added.Should().BeTrue();
            list.Count.Should().Be(4);
            list.SingleOrDefault(p => p.FirstName == "first4").Should().NotBeNull();
        }

        [Fact]
        public void TryAdd_with_comparer_Func_given_an_existing_entry_ignores_it_returns_false()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var existingEntry = new Person("first3", "Last3");
            var added = list.TryAdd(existingEntry, (p1, p2) => p1.FirstName == p2.FirstName
                                                            && p1.LastName == p2.LastName);

            added.Should().BeFalse();
            list.Count.Should().Be(3);
        }

        [Fact]
        public void TryAdd_with_comparer_Func_given_an_null_for_the_predicate_throwes_ArgumentNullException()
        {
            var list = new List<string>();
            Func<string, string, bool> func = null;

            Assert.Throws<ArgumentNullException>(() => list.TryAdd("Test", func));
        }

        [Fact]
        public void AddIf_given_a_true_predicate_adds_the_given_item_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var newItem = new Person("first4", "Last2");

            var added = list.AddIf(newItem, v => v.LastName == "Last2");

            added.Should().BeTrue();
            list.Count.Should().Be(4);
            list.SingleOrDefault(item => item.FirstName == "first4" && item.LastName == "Last2").Should().NotBeNull();

        }


        [Fact]
        public void AddIf_given_a_false_predicate_ignores_the_given_item_returns_false()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var newItem = new Person("first4", "Last2");

            var added = list.AddIf(newItem, v => v.LastName == "Last3");

            added.Should().BeFalse();
            list.Count.Should().Be(3);
            list.SingleOrDefault(item => item.FirstName == "first4" && item.LastName == "Last2").Should().BeNull();

        }

        [Fact]
        public void AddIfNot_given_a_true_predicate_ignores_the_given_item_returns_false()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var newItem = new Person("first4", "Last2");

            var added = list.AddIfNot(newItem, v => v.LastName == "Last2");

            added.Should().BeFalse();
            list.Count.Should().Be(3);
            list.SingleOrDefault(item => item.FirstName == "first4" && item.LastName == "Last2").Should().BeNull();

        }


        [Fact]
        public void AddIfNot_given_a_false_predicate_adds_the_given_item_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last3")
            };

            var newItem = new Person("first4", "Last2");

            var added = list.AddIfNot(newItem, v => v.LastName == "Last3");

            added.Should().BeTrue();
            list.Count.Should().Be(4);
            list.SingleOrDefault(item => item.FirstName == "first4" && item.LastName == "Last2").Should().NotBeNull();

        }

        [Fact]
        public void ReplaceWhere_given_true_predicate_replaces_all_the_corresponding_entries_with_the_given_item_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2")
            };

            var newItem = new Person("first4", "Last4");

            var replaced = list.ReplaceWhere(newItem, v => v.LastName == "Last2");

            replaced.Should().BeTrue();
            list.Where(v => v.FirstName == "first4" && v.LastName == "Last4").Count().Should().Be(2);
        }

        [Fact]
        public void ReplaceWhere_given_false_predicate_returns_flase()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2")
            };

            var newItem = new Person("first4", "Last4");

            var replaced = list.ReplaceWhere(newItem, v => v.LastName == "Last100");

            replaced.Should().BeFalse();
            list.Where(v => v.FirstName == "first4" && v.LastName == "Last4").Count().Should().Be(0);
        }

        [Fact]
        public void ReplaceSingle_given_true_predicate_throwes_an_exception_for_multiple_corresponding_entries()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2")
            };

            var newItem = new Person("first4", "Last4");

            Assert.Throws<InvalidOperationException>(() => list.ReplaceSingle(newItem, v => v.LastName == "Last2"));
        }

        [Fact]
        public void ReplaceSingle_given_true_predicate_replaces_the_corresponding_entry_with_the_given_item_returns_true()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2")
            };

            var newItem = new Person("first4", "Last4");

            var replaced = list.ReplaceSingle(newItem, v => v.LastName == "Last1");

            replaced.Should().BeTrue();
            list.SingleOrDefault(v => v.FirstName == "first4" && v.LastName == "Last4").Should().NotBeNull();
        }

        [Fact]
        public void ReplaceSingle_given_sequence_that_do_not_satisfy_the_condition_returns_flase()
        {
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2")
            };

            var newItem = new Person("first4", "Last4");

            var replaced = list.ReplaceSingle(newItem, v => v.LastName == "Last100");

            replaced.Should().BeFalse();
            list.SingleOrDefault(v => v.FirstName == "first4" && v.LastName == "Last4").Should().BeNull();
        }


        [Fact]
        public void RemoveWhere_given_true_predicate_removes_the_corresponding_entries_returns_true()
        {
            var newItem = new Person("first4", "Last4");
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2"),
                newItem
            };



            var removed = list.RemoveWhere(v => v.LastName == "Last4");

            removed.Should().BeTrue();
            list.Count.Should().Be(3);
            list.SingleOrDefault(v => v.FirstName == "first4" && v.LastName == "Last4").Should().BeNull();
        }

        [Fact]
        public void RemoveWhere_given_false_predicate_returns_false()
        {
            var newItem = new Person("first4", "Last4");
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2"),
                newItem
            };



            var removed = list.RemoveWhere(v => v.LastName == "Last100");

            removed.Should().BeFalse();
            list.Count.Should().Be(4);
        }

        [Fact]
        public void RemoveWhereNot_given_false_predicate_removes_the_corresponding_entries_returns_true()
        {
            var newItem = new Person("first4", "Last4");
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2"),
                newItem
            };



            var removed = list.RemoveWhereNot(v => v.LastName == "Last4");

            removed.Should().BeTrue();
            list.Count.Should().Be(1);
            list.SingleOrDefault(v => v.FirstName == "first4" && v.LastName == "Last4").Should().NotBeNull();
        }

        [Fact]
        public void RemoveWhereNot_given_true_predicate_returns_false()
        {
            var newItem = new Person("first4", "Last4");
            var list = new List<Person>
            {
                new Person("first1","Last1"),
                new Person("first2","Last2"),
                new Person("first3", "Last2"),
                newItem
            };



            var removed = list.RemoveWhereNot(v => v.LastName != "");

            removed.Should().BeFalse();
            list.Count.Should().Be(4);
        }
    }
}
