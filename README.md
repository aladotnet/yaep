# yaep

yet another extensions package is a collection of extension methods to increase code readability. 

### Install 

```markdown
Install-Package YAEP
```
### Usage

```markdown
s.IsNullOrEmpty();
s.EqualsIgnoreCaseCurrent() //CurrentCulture
s.EqualsIgnoreCaseInvariant() 
s.EqualsIgnoreCaseOrdinal()
s.Concat(new []{"1","2"})

var list = new List<Todo>();
list.IsNullOrEmpty();
list.IsNotEmpty()
list.AsReadOnly();
list.EmptyIfNull()

todo.IsNotNull();
todo.IsNull();
todo.DefaultIfNull(new Todo(Guid.Empty.ToString(), "--"));

obj.AsTaskFromResult() => returns Task<objType>
```



> **Note:** the namespaces maches the namespace of the extended Type so you don't have to change your usings