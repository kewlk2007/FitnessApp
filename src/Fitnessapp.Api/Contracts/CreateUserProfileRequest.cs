using System.ComponentModel;

namespace Fitnessapp.Api.Contracts;

public class CreateUserProfileRequest
{
    public string Name { get; set; }

    public decimal Weight { get; set; } = 0;

    public decimal Height { get; set; } = 0;

    public DateTime BirthDate { get; set; } = DateTime.Now;
}