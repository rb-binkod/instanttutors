namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sessionupd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "Description", c => c.String());
            AddColumn("dbo.Sessions", "Concerns", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "Concerns");
            DropColumn("dbo.Sessions", "Description");
        }
    }
}
