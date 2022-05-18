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

        public string Info { get; set; }

        [Required]
        public float  Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }       

        [StringLength(100)]
        public string Name { get; set; }

        public DateTime? BreakTimeStart { get; set; }
        public DateTime? BreakTimeEnd { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }

        [StringLength(150)]
        public string MessageUz { get; set; }

        [StringLength(150)]
        public string MessageRu { get; set; }
        [StringLength(150)]
        public string MessageLt { get; set; }

        public int OrderIndex { get; set; } = 1;

        public int? SpecializationId { get; set; }
        public virtual spSpecialization Specialization { get; set; }

        public virtual List< spCategory> Categories{ get; set; }

        public int? Rating { get; set; }
    }
}
