# Summary
This repo is a `Xamarin.Forms` iOS app with `Microsoft.EntityFrameworkCore.Sqlite` which demonstrates an issue when `ChangeTracker.HasChanges()` returns `true` unexpectedly when running on a physical device (`ARM64`). The issue does not appear when running on the simulator (`x86_64`).

&nbsp;

# Setup
Given the model:

```cs
public abstract class EntityBase {

    public Guid Id { get; set; }

    public Guid? GroupId { get; set; }

    public Guid? SubGroupId { get; set; }
}
```
```cs
public class User : EntityBase {

    public string Username { get; set; }

    public string EmailAddress { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public Guid? OrganizationId { get; set; }
}
```
We add a single entity:
```cs
using(ModelContext ctx = new ModelContext(connection))
{
    ctx.Database.EnsureCreated();

    ctx.Add(new User { Id = Guid.NewGuid() });
    ctx.SaveChanges();
}
```
Then we query and check for changes:
```cs
using(ModelContext ctx = new ModelContext(connection))
{
    stringBuilder.AppendLine($"Pre-query HasChanges() = {ctx.ChangeTracker.HasChanges()}");

    List<User> users = ctx.Set<User>().ToList();

    stringBuilder.AppendLine($"Query result count: {users.Count}");

    User user = ctx.Set<User>().FirstOrDefault();

    stringBuilder.AppendLine($"Post-query HasChanges() = {ctx.ChangeTracker.HasChanges()}");

    EntityEntry entry = ctx.Entry(user);

    List<PropertyEntry> modifiedProperties = entry.Properties.Where(p => p.IsModified).ToList();

    if(modifiedProperties.Count == 0)
    {
        stringBuilder.AppendLine("No modified properties found");
    }
    else
    {
        foreach(PropertyEntry prop in modifiedProperties)
        {
            stringBuilder.AppendLine($"Modified property \"{prop.Metadata.Name}\"");
            stringBuilder.AppendLine($"Original: {prop.OriginalValue}");
            stringBuilder.AppendLine($" Current: {prop.CurrentValue}");
        }
    }
}
```

&nbsp;

# Demonstration
The expected output is:
```
Pre-query HasChanges() = False
Query result count: 1
Post-query HasChanges() = False
No modified properties found
```
But the actual output is:
```
Pre-query HasChanges() = False
Query result count: 1
Post-query HasChanges() = True
Modified property "SubGroupId"
Original: 00000001-0000-0000-0000-0000e0eb1a6d
 Current: 
```

&nbsp;

# Alternatives
* ### Mono Interpreter
    The branch [no-issue-mono-interpreter](https://github.com/tstuts/xf-ef-sqlite-unexpected-changes/compare/no-issue-mono-interpreter) demonstrates that after enabling the Mono Interpreter (adding `<MtouchInterpreter>-all</MtouchInterpreter>` to the build configuration) the issue no longer appears.

* ### Model Modification
    The branch [no-issue-modified-model](https://github.com/tstuts/xf-ef-sqlite-unexpected-changes/compare/no-issue-modified-model) demonstrates that after making a modification to the model (eliminating the `LastName` property) the issue no longer appears.
