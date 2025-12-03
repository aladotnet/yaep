![YAEP](https://github.com/aladotnet/yaep/blob/dev/Images/YAEP.png)

# YAEP - Yet Another Extensions Package

A high-performance, allocation-optimized collection of C# extension methods designed to increase code readability and reduce boilerplate. YAEP provides fluent APIs for common operations on strings, collections, tasks, enums, and more.

## Features

- **Allocation-Optimized**: Methods use `Span<T>`, manual iteration, and caching to minimize heap allocations
- **Nullable-Aware**: Uses `[NotNullWhen]` and `[NotNull]` attributes for proper compiler null-state tracking
- **Fluent APIs**: All methods return values to enable method chaining
- **Zero Dependencies**: Pure .NET library with no external dependencies
- **Namespace-Matching**: Extension methods are in the same namespace as the types they extend (no extra `using` statements needed)

## Installation

```bash
dotnet add package YAEP
```

Or via Package Manager:

```powershell
Install-Package YAEP
```

## Quick Start

```csharp
// String operations
string? name = GetName();
if (!name.IsNullOrEmpty())
{
    // name is guaranteed non-null here (compiler knows this!)
    Console.WriteLine(name.Length);
}

// Collection operations
var items = GetItems();
if (items.IsNotEmpty())
{
    items.ForEach(item => Process(item));
}

// Object null checks
User? user = GetUser();
var displayName = user.DefaultIfNull(defaultUser).Name;

// Task utilities
var result = someValue.AsTaskFromResult(); // Returns cached Task for common values
await httpClient.GetAsync(url).DetachedAwait(); // ConfigureAwait(false)
```

## Extension Classes

### StringExtensions

String operations including null/empty checks, case-insensitive comparisons, and encoding utilities.

```csharp
// Null/Empty checks (with nullable flow analysis)
string? value = GetValue();
if (!value.IsNullOrEmpty())
{
    // value is guaranteed non-null here
}

// Case-insensitive comparisons
"Hello".EqualsIgnoreCaseOrdinal("HELLO");      // true - fastest, for identifiers
"Hello".EqualsIgnoreCaseInvariant("HELLO");    // true - culture-insensitive
"Straße".EqualsIgnoreCaseCurrent("STRASSE");   // depends on culture

// Default values
string? input = null;
string result = input.DefaultIfNull("N/A");     // "N/A"
string empty = input.EmptyIfNull();             // ""

// Byte conversion (with allocation-free overload)
byte[] bytes = "Hello".ToByteArray();           // UTF-8 by default
byte[] ascii = "Hello".ToByteArray(Encoding.ASCII);

// Allocation-free version using Span
Span<byte> buffer = stackalloc byte["Hello".GetByteCount()];
int written = "Hello".ToByteArray(buffer);

// String concatenation (uses StringBuilder internally)
string result = "Hello".Concat(new[] { " ", "World", "!" }); // "Hello World!"

// Validation
"12345".IsNumeric();                            // true (digits only)
"550e8400-e29b-41d4-a716-446655440000".IsGuidValue(); // true
```

### CollectionsUtils

Collection operations with null-safety and performance optimizations.

```csharp
// Null/Empty checks (optimized for ICollection/IReadOnlyCollection)
int[]? numbers = GetNumbers();
if (!numbers.IsNullOrEmpty())
{
    // numbers is guaranteed non-null here
    Console.WriteLine(numbers[0]);
}

IEnumerable<string>? names = GetNames();
if (names.IsNotEmpty())
{
    // Safe to enumerate
}

// Safe iteration (no null checks needed)
foreach (var item in items.EmptyIfNull())
{
    Process(item);
}

// Conditional operations
var list = new List<string>();
list.TryAdd("item");                            // Adds only if not present
list.TryAdd("item", StringComparer.OrdinalIgnoreCase); // With custom comparer
list.AddIf(value, x => x.Length > 0);           // Add if predicate is true
list.AddIfNot(value, string.IsNullOrEmpty);     // Add if predicate is false

// Replace and remove operations
var numbers = new List<int> { 1, 2, 3, 2, 4 };
numbers.ReplaceWhere(0, x => x == 2);           // { 1, 0, 3, 0, 4 }
numbers.ReplaceSingle(99, x => x == 3);         // Throws if multiple matches
numbers.RemoveWhere(x => x % 2 == 0);           // Remove all even numbers

// Conditional filtering
var filtered = products.WhereIf(
    p => p.Category == category,
    () => filterByCategory);

// ForEach extension
items.ForEach(item => Console.WriteLine(item));
```

### ObjectExtensions

Fluent null checking with nullable reference type support.

```csharp
User? user = GetUser();

// Null checks with flow analysis
if (!user.IsNull())
{
    // user is guaranteed non-null here
    Console.WriteLine(user.Name);
}

if (user.IsNotNull())
{
    // user is guaranteed non-null here
    ProcessUser(user);
}

// Default values (alternative to ?? operator)
var displayName = user.DefaultIfNull(defaultUser).Name;
```

### ExceptionExtensions (Guard Clauses)

Fluent validation with exception throwing.

```csharp
// Guard against null
public void ProcessUser(User? user)
{
    user.GuardAgainstNull(nameof(user));
    // user is now guaranteed non-null
    Console.WriteLine(user.Name);
}

// Guard against null with custom exception
var user = repository.Find(id)
    .GuardAgainstNull(new EntityNotFoundException($"User {id} not found"));

// Guard against null or empty string
public void SetName(string? name)
{
    Name = name.GuardAgainstNullOrEmpty(nameof(name), "Name is required");
}

// Guard against custom conditions
public void SetAge(int age)
{
    age.GuardAgainst(a => a < 0, "Age cannot be negative");
    Age = age;
}

// Fluent chaining
var user = GetUser()
    .GuardAgainstNull(nameof(user))
    .GuardAgainst(u => u.IsDisabled, "User is disabled");
```

### TaskExtensions

Async utilities with caching for common values.

```csharp
// Detached await (ConfigureAwait(false))
var data = await httpClient.GetStringAsync(url).DetachedAwait();
// Also works with ValueTask
var result = await valueTask.DetachedAwait();

// Convert values to completed tasks (cached for common values)
Task<bool> trueTask = true.AsTaskFromResult();   // Returns cached task
Task<int> zeroTask = 0.AsTaskFromResult();       // Returns cached task
Task<string> emptyTask = "".AsTaskFromResult();  // Returns cached task
Task<int> customTask = 42.AsTaskFromResult();    // Creates new task

// ValueTask conversion
ValueTask<int> valueTask = value.AsValueTaskFromResult();

// Synchronous wait (use sparingly!)
var result = asyncOperation.GetAwaiterResult();

// Task to ValueTask conversion
ValueTask<string> vt = task.AsValueTask();
```

### EnumExtensions

Enum utilities with caching for performance.

```csharp
// Parse with default value (never throws)
var status = "Active".ToEnum(Status.Unknown);
var invalid = "invalid".ToEnum(Status.Unknown);  // Returns Unknown

// Get all values (cached)
var values = DayOfWeek.Monday.GetValues();

// Get all names (cached)
var names = DayOfWeek.Monday.GetNames();

// Get name-value pairs (cached, useful for dropdowns)
var pairs = Status.Active.ToNameValuePares();
foreach (var (name, value) in pairs)
{
    Console.WriteLine($"{name} = {value}");
}

// Look up by name (case-insensitive)
int value = DayOfWeek.Monday.GetValue("Friday");  // 5

// Look up by value
string name = DayOfWeek.Monday.GetName(5);        // "Friday"
```

### StringBuilderExtensions

Conditional StringBuilder operations for fluent string building.

```csharp
var sb = new StringBuilder();

// Conditional appending
sb.AppendIf(() => includeHeader, "Header: ")
  .AppendLineIf(() => showTitle, "Title")
  .AppendJoinIf(() => items.Any(), ", ", items)
  .ReplaceIf(() => shouldReplace, "old", "new")
  .ClearIf(() => shouldReset);

// Create StringBuilder from string
var builder = "Hello".ToStringBuilder()
    .Append(" World")
    .AppendLine("!");
```

### GuidExtensions

```csharp
Guid id = GetId();
if (id.IsEmpty())
{
    throw new ArgumentException("ID cannot be empty");
}
```

### ReflectionExtensions

Reflection utilities with caching.

```csharp
// Check for generic base type
typeof(StringList).IsSubClassOfGenericBase(typeof(List<>)); // true

// Filter types from assemblies
var controllers = assemblies.TypesWhere(t =>
    t.IsClass && t.Name.EndsWith("Controller"));

// Check for attributes
typeof(MyClass).HasCustomAttribute<SerializableAttribute>();

// Check if type is static
typeof(StaticHelper).IsStatic(); // true

// Load referenced assemblies (cached)
var references = assembly.LoadReferencedAssemblies();
```

### XElementExtensions

LINQ to XML utilities.

```csharp
var element = XElement.Parse("<item id='123' status='Active'/>");

// Get attribute with default
var id = element.GetAttributeValue("id");              // "123"
var missing = element.GetAttributeValue("foo", "N/A"); // "N/A"

// Compare attribute values (case-insensitive)
element.AttributeValueEquals("status", "active");      // true
element.AttributeValueContains("status", "Act");       // true

// Fluent modification
var newElement = new XElement("item")
    .WithName("product")
    .WithValue("Widget");

// Find root parent
var root = deepElement.RootParent();

// Compare local name
element.LocalNameEquals("item");                       // true (case-insensitive)
```

### JsonSerializationExtensions

JSON serialization utilities.

```csharp
// Deserialize with optional configuration
var user = jsonString.JsonDeserialize<User>();
var user = jsonString.JsonDeserialize<User>(options =>
{
    options.PropertyNameCaseInsensitive = true;
});

// Serialize with optional configuration
var json = user.ToJson();
var json = user.ToJson(options =>
{
    options.WriteIndented = true;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
```

### ExpressionsExtensions

LINQ expression utilities for dynamic property access.

```csharp
// Get property name from expression (refactoring-safe)
Expression<Func<User, string>> expr = u => u.Name;
string propName = expr.GetName(); // "Name"

// Create property expression at runtime
var expression = user.ToPropertyExpression("Name");
// Use with EF: context.Users.OrderBy(expression)

// Create compiled property selector
var selector = user.ToPropertySelector<User, string>("Name");
string name = selector(user); // Efficient property access
```

## Performance Considerations

YAEP is designed with performance in mind:

1. **Caching**: Enum values/names and assembly references are cached using `ConcurrentDictionary`
2. **Allocation-Free**: Methods use `Span<T>` and manual iteration where possible
3. **Optimized Collection Checks**: `IsNullOrEmpty` checks for `ICollection<T>` and `IReadOnlyCollection<T>` use `Count` property instead of enumeration
4. **Cached Tasks**: Common task results (`true`, `false`, `0`, `1`, empty string, `null`) are cached

## Requirements

- .NET 10.0+


## License

MIT License - see LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
