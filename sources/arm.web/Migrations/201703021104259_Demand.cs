namespace arm_repairs_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Demand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.demand",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        description_issue = c.String(),
                        phone = c.String(),
                        decision_hours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        decision_description = c.String(),
                        equipment = c.String(),
                        manager = c.String(maxLength: 128),
                        master = c.String(maxLength: 128),
                        priority_id = c.Int(nullable: false),
                        status_id = c.Int(nullable: false),
                        user = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AspNetUsers", t => t.manager)
                .ForeignKey("dbo.AspNetUsers", t => t.master)
                .ForeignKey("dbo.priority", t => t.priority_id, cascadeDelete: true)
                .ForeignKey("dbo.demand_status", t => t.status_id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.user, cascadeDelete: true)
                .Index(t => t.manager)
                .Index(t => t.master)
                .Index(t => t.priority_id)
                .Index(t => t.status_id)
                .Index(t => t.user);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.demand", "user", "dbo.AspNetUsers");
            DropForeignKey("dbo.demand", "status_id", "dbo.demand_status");
            DropForeignKey("dbo.demand", "priority_id", "dbo.priority");
            DropForeignKey("dbo.demand", "master", "dbo.AspNetUsers");
            DropForeignKey("dbo.demand", "manager", "dbo.AspNetUsers");
            DropIndex("dbo.demand", new[] { "user" });
            DropIndex("dbo.demand", new[] { "status_id" });
            DropIndex("dbo.demand", new[] { "priority_id" });
            DropIndex("dbo.demand", new[] { "master" });
            DropIndex("dbo.demand", new[] { "manager" });
            DropTable("dbo.demand");
        }
    }
}
