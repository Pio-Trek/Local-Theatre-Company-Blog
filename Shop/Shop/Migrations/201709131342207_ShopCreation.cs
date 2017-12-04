namespace Shop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShopCreation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Reviews", "Product_Id", "dbo.Products");
            DropIndex("dbo.Products", new[] { "Category_Id" });
            DropIndex("dbo.Reviews", new[] { "Product_Id" });
            DropColumn("dbo.Products", "Category_Id");
            DropColumn("dbo.Reviews", "Product_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reviews", "Product_Id", c => c.Int());
            AddColumn("dbo.Products", "Category_Id", c => c.Int());
            CreateIndex("dbo.Reviews", "Product_Id");
            CreateIndex("dbo.Products", "Category_Id");
            AddForeignKey("dbo.Reviews", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Products", "Category_Id", "dbo.Categories", "Id");
        }
    }
}
