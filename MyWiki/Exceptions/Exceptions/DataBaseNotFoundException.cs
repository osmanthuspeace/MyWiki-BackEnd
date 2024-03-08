namespace MyWiki.Exceptions.Exceptions;

public sealed class DataBaseNotFoundException(int id, string entryName) : Exception
{
    public override string Message => $"Couldn't find [{entryName}] with id [{id}].";
}
