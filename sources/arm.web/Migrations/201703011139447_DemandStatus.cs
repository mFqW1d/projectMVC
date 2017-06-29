namespace arm_repairs_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DemandStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.demand_status",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        caption = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.demand_status");
        }
    }
}
