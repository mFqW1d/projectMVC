namespace arm_repairs_project.Models.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class PriorityMap : EntityTypeConfiguration<Priority>
    {
        public PriorityMap()
        {
            ToTable("priority");
            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id");
            Property(x => x.Caption).HasColumnName("caption");
        }
    }
}