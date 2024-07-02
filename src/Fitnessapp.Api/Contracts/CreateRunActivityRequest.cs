using System.ComponentModel;

namespace Fitnessapp.Api.Contracts;

public class CreateRunActivityRequest
{
    public int UserProfileId { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }
    public int Distance { get; set; }
}