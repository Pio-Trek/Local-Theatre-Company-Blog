namespace Shop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reviews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reviews", "Product_Id", c => c.Int());
            CreateIndex("dbo.Reviews", "Product_Id");
            AddForeignKey("dbo.Reviews", "Product_Id", "dbo.Products", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "Product_Id", "dbo.Products");
            DropIndex("dbo.Reviews", new[] { "Product_Id" });
            DropColumn("dbo.Reviews", "Product_Id");
        }
    }
}
