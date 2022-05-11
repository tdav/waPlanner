using System.ComponentModel.DataAnnotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;
using waPlanner.ModelViews;

namespace waPlanner.Database.Models
{
    public class spCategory : NameBaseModel
    {
        public int Id { get; set; }

        [StringLength(200)]
        public new string NameUz { get; set; }

        [StringLength(200)]
        public new string NameLt { get; set; }

        [StringLength(200)]
        public new string NameRu { get; set; }

        public int? OrganizationId { get; set; }
        public virtual spOrganization Organization { get; set; }
    }
}
