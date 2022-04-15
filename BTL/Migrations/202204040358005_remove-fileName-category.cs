namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removefileNamecategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "PictureId", c => c.Int());
            CreateIndex("dbo.Categories", "PictureId");
            AddForeignKey("dbo.Categories", "PictureId", "dbo.Pictures", "Id");
            DropColumn("dbo.Categories", "PictureName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Categories", "PictureName", c => c.String());
            DropForeignKey("dbo.Categories", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Categories", new[] { "PictureId" });
            DropColumn("dbo.Categories", "PictureId");
        }
    }
}
