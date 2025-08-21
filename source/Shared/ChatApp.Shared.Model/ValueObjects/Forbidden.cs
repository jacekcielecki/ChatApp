namespace ChatApp.Shared.Model.ValueObjects;

public struct Forbidden
{
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public Forbidden(IDictionary<string, string[]> errors)
    {
        Errors = errors;
    }
}