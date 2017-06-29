namespace arm_repairs_project.Models.Data.Mapping
{
    using System.Data.Entity.ModelConfiguration;

    public class DemandMap : EntityTypeConfiguration<Demand>
    {
        public DemandMap()
        {
            ToTable("demand");
            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id");
            Property(x => x.Date).HasColumnName("date");
            Property(x => x.DescriptionIssue).HasColumnName("description_issue");
            Property(x => x.Phone).HasColumnName("phone");
            Property(x => x.DecisionHours).HasColumnName("decision_hours");
            Property(x => x.DecisionDescription).HasColumnName("decision_description");
            Property(x => x.Equipment).HasColumnName("equipment");
            HasRequired(x => x.User)
                .WithMany()
                .Map(x => x.MapKey("user"));
            HasOptional(x => x.Master)
                .WithMany()
                .Map(x => x.MapKey("master"));
            HasOptional(x => x.Manager)
                .WithMany()
                .Map(x => x.MapKey("manager"));
            HasRequired(x => x.Priority)
                .WithMany()
                .Map(x => x.MapKey("priority_id"));
            HasRequired(x => x.Status)
                .WithMany()
                .Map(x => x.MapKey("status_id"));
        }
    }
}