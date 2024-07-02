using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Api.Contracts;

public class UserProfileResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Weight { get; set; } = 0;

    public decimal Height { get; set; } = 0;

    public DateTime BirthDate { get; set; } = DateTime.Now;

    public int Age => DateTime.Now.Year - BirthDate.Year;

    public decimal BMI => (this.Weight / ((this.Height * this.Height)/100))*100;
}