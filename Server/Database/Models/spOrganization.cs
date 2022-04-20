using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spOrganization: BaseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public long ChatId { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        [Required]
        public float  Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }       

        [StringLength(150)]
        public string Name { get; set; }

        public DateTime? DinnerTimeStart { get; set; } = DateTime.Now;
        public DateTime? DinnerTimeEnd { get; set; } = DateTime.Now;

        public int? SpecializationId { get; set; }
        public virtual spSpecialization Specialization { get; set; }

        public virtual List< spCategory> Categories{ get; set; }
    }
}
