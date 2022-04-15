namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_picture_id_allow_null : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Users", new[] { "PictureId" });
            AlterColumn("dbo.Users", "PictureId", c => c.Int());
            CreateIndex("dbo.Users", "PictureId");
            AddForeignKey("dbo.Users", "PictureId", "dbo.Pictures", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Users", new[] { "PictureId" });
            AlterColumn("dbo.Users", "PictureId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "PictureId");
            AddForeignKey("dbo.Users", "PictureId", "dbo.Pictures", "Id", cascadeDelete: true);
        }
    }
}
