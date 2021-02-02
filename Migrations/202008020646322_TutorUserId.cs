namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TutorUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "TutorUserId", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "TutorUserId");
        }
    }
}
