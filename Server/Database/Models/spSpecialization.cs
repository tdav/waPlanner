using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spSpecialization : NameBaseModel
    {
        public int Id { get; set; }

        [IndexColumn(IsUnique = true)]
        [StringLength(200)]
        public new string NameUz { get; set; }

        [IndexColumn(IsUnique = true)]
        [StringLength(200)]
        public new string NameLt { get; set; }

        [IndexColumn(IsUnique = true)]
        [StringLength(200)]
        public new string NameRu { get; set; }
        public virtual List<spOrganization> Organizations { get; set; }
    }
}
