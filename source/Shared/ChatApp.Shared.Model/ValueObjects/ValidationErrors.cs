namespace ChatApp.Shared.Model.ValueObjects;

public readonly struct ValidationErrors
{
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public ValidationErrors(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}
