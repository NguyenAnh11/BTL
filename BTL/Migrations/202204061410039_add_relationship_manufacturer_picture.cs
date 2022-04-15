namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_relationship_manufacturer_picture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manufacturers", "PictureId", c => c.Int(nullable: false));
            CreateIndex("dbo.Manufacturers", "PictureId");
            AddForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures", "Id", cascadeDelete: true);
            DropColumn("dbo.Manufacturers", "PictureName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Manufacturers", "PictureName", c => c.String());
            DropForeignKey("dbo.Manufacturers", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Manufacturers", new[] { "PictureId" });
            DropColumn("dbo.Manufacturers", "PictureId");
        }
    }
}
