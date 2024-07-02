using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitnessapp.Api.Entities
{
    public class RunActivity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        [Description("Distance in kilometers")]
        public int Distance { get; set; }
        [NotMapped]
        [Description("Duration in minutes")]
        public DateTime? Duration { get; set; }
        [NotMapped]
        [Description("Minutes per kilometer")]
        public double AveragePace { get; set; }
    }
}
