using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace waPlanner.ModelViews
{
    public interface IBaseModel
    {
        int Status { get; set; }
        int CreateUser { get; set; }
        DateTime CreateDate { get; set; }
        DateTime? UpdateDate { get; set; }
        int? UpdateUser { get; set; }
    }

    public class BaseModel : IBaseModel
    {
        [DefaultValue(1)]
        public int Status { get; set; }
        public int CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public interface INameBaseModel
    {
        string NameUz { get; set; }
        string NameLt { get; set; }
        string NameRu { get; set; }
    }

    public class NameBaseModel : IBaseModel, INameBaseModel
    {
        [DefaultValue(1)]
        public int Status { get; set; }
        public int CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUser { get; set; }

        [StringLength(200)]
        public string NameUz { get; set; }

        [StringLength(200)]
        public string NameLt { get; set; }

        [StringLength(200)]
        public string NameRu { get; set; }
    }
}
