namespace WebApi.Entities;

public sealed class Todo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = default!;
    public bool Done { get; private set; }

    public Todo(string title) => Title = title;
    public void MarkDone() => Done = true;
}
