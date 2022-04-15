namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_display_order_product : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DisplayOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DisplayOrder");
        }
    }
}
