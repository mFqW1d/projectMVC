
namespace arm_repairs_project.Models.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class DemandStatusMap: EntityTypeConfiguration<DemandStatus>
    {
        public DemandStatusMap()
        {
            ToTable("demand_status");
            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id");
            Property(x => x.Caption).HasColumnName("caption");
        }
    }
}