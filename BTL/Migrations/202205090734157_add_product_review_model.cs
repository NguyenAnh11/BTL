namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_product_review_model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductReviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreateAt = c.DateTime(nullable: false),
                        ProductId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Content = c.String(),
                        ParentId = c.Int(),
                        ProductReview_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.ProductReviews", t => t.ProductReview_Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.UserId)
                .Index(t => t.ProductReview_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductReviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.ProductReviews", "ProductReview_Id", "dbo.ProductReviews");
            DropForeignKey("dbo.ProductReviews", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductReviews", new[] { "ProductReview_Id" });
            DropIndex("dbo.ProductReviews", new[] { "UserId" });
            DropIndex("dbo.ProductReviews", new[] { "ProductId" });
            DropTable("dbo.ProductReviews");
        }
    }
}
