namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removemimeTypePicture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pictures", "FileName", c => c.String());
            DropColumn("dbo.Pictures", "Name");
            DropColumn("dbo.Pictures", "MimeType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pictures", "MimeType", c => c.String());
            AddColumn("dbo.Pictures", "Name", c => c.String());
            DropColumn("dbo.Pictures", "FileName");
        }
    }
}
