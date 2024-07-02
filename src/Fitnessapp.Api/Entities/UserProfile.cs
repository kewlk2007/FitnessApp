using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitnessapp.Api.Entities
{
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [Description("Weight in kilograms")]
        public decimal Weight { get; set; } = 0;

        [Description("Height in centimeters")]
        public decimal Height { get; set; } = 0;
        public DateTime BirthDate { get; set; } = DateTime.Now;

        [NotMapped]
        public int Age => DateTime.Now.Year - BirthDate.Year;

        [NotMapped]
        public decimal BMI => (this.Weight / ((this.Height * this.Height)/100))*100;
    }
}
