namespace Annotations.API;

/// <summary>
/// This is a mock class that represents users. Later this will be replaced with a real implementation.
/// </summary>

public record UserData(string? UserNames, int Age, DateOnly Date, string? Occupation)
{
    //public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}