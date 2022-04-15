namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_picture : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductPictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PictureId = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pictures", t => t.PictureId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.PictureId);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Title = c.String(),
                        Alt = c.String(),
                        MimeType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Products", "PictureName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "PictureName", c => c.String());
            DropForeignKey("dbo.ProductPictures", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductPictures", "PictureId", "dbo.Pictures");
            DropIndex("dbo.ProductPictures", new[] { "PictureId" });
            DropIndex("dbo.ProductPictures", new[] { "ProductId" });
            DropTable("dbo.Pictures");
            DropTable("dbo.ProductPictures");
        }
    }
}
