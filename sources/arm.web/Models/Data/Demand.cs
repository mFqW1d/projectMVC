namespace arm_repairs_project.Models.Data
{
    using System;

    public class Demand
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string DescriptionIssue { get; set; }
        public string Phone { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Master { get; set; }
        public virtual ApplicationUser Manager { get; set; }
        public decimal DecisionHours { get; set; }
        public string DecisionDescription { get; set; }
        public string Equipment { get; set; }
        public virtual Priority Priority { get; set; }
        public virtual DemandStatus Status { get; set; }
    }
}