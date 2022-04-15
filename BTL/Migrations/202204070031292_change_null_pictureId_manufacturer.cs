namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_null_pictureId_manufacturer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Manufacturers", new[] { "PictureId" });
            AlterColumn("dbo.Manufacturers", "PictureId", c => c.Int());
            CreateIndex("dbo.Manufacturers", "PictureId");
            AddForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Manufacturers", new[] { "PictureId" });
            AlterColumn("dbo.Manufacturers", "PictureId", c => c.Int(nullable: false));
            CreateIndex("dbo.Manufacturers", "PictureId");
            AddForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures", "Id", cascadeDelete: true);
        }
    }
}
