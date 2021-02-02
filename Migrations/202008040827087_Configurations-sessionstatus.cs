namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Configurationssessionstatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Value = c.String(),
                        DefaultValue = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Sessions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Sessions", "ApproveDeclineBy", c => c.String());
            AddColumn("dbo.Sessions", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "CreatedBy");
            DropColumn("dbo.Sessions", "ApproveDeclineBy");
            DropColumn("dbo.Sessions", "Status");
            DropTable("dbo.Configurations");
        }
    }
}
