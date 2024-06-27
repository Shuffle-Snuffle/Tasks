using AvaloniaApplicationPracticeOne.Models;

namespace AvaloniaApplicationPracticeOne;

public class Service
{
    private static ProgramContext? _db;

    public static ProgramContext GetDbContext()
    {
        if (_db == null)
        {
            _db = new ProgramContext();
        }
        return _db;
    }
}