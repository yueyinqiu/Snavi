namespace Snavi.UserInterfaces;

public interface IPickableCommand
{
    string Template { get; }
    
    string Description { get; }
}