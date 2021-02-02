namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class homeaboutcontrols : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AboutControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstHeading = c.String(),
                        FirstHeadingText = c.String(),
                        FirstHeadingImage = c.String(),
                        SecondHeading = c.String(),
                        SecondHeadingText = c.String(),
                        SecondHeadingImage = c.String(),
                        ThirdHeading = c.String(),
                        ThirdHeadingText = c.String(),
                        ThirdHeadingImage = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HomepageControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstSliderHeading = c.String(),
                        FirstSliderHeadingText = c.String(),
                        FirstSliderHeadingImage = c.String(),
                        SecondSliderHeading = c.String(),
                        SecondSliderHeadingText = c.String(),
                        SecondSliderHeadingImage = c.String(),
                        ThirdSliderHeading = c.String(),
                        ThirdSliderHeadingText = c.String(),
                        FacebookLink = c.String(),
                        TwitterLink = c.String(),
                        InstagramLink = c.String(),
                        YoutubeLink = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "Hobbies", c => c.String());
            AddColumn("dbo.AspNetUsers", "StudentGrade", c => c.String());
            AddColumn("dbo.AspNetUsers", "StudentSchool", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "StudentSchool");
            DropColumn("dbo.AspNetUsers", "StudentGrade");
            DropColumn("dbo.AspNetUsers", "Hobbies");
            DropTable("dbo.HomepageControls");
            DropTable("dbo.AboutControls");
        }
    }
}
