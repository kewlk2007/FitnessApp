using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Api.Contracts;

public class RunActivityResponse
{
    public int Id { get; set; }
    public int UserProfileId { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Ended { get; set; }    
    public int Distance { get; set; }    
    public TimeSpan Duration 
    { 
        get {
            return Ended.Value.Subtract(Started.Value);
        } 
    }  
    public double AveragePace
    { 
        get {
            return this.Duration.TotalMinutes / Distance;
        } 
    }
}