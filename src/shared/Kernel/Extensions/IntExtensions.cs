namespace Kernel;

/// <summary>
/// Provides extension methods for converting integer values to time intervals represented by TimeSpan objects.
/// </summary>
/// <remarks>These methods enable more readable code when working with time intervals based on integer values. For
/// example, you can write '5.ToMinutes()' instead of 'TimeSpan.FromMinutes(5)'.</remarks>
public static class IntExtensions
{
    public static TimeSpan ToMinutes(this int value)
    {
        return TimeSpan.FromMinutes(value);
    }
    public static TimeSpan ToHours(this int value)
    {
        return TimeSpan.FromHours(value);
    }
    public static TimeSpan ToDays(this int value)
    {
        return TimeSpan.FromDays(value);
    }
}
