namespace LocalTheatre.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocalTheatreInit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "PublishDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Comments", "PublisDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "PublisDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Comments", "PublishDate");
        }
    }
}
