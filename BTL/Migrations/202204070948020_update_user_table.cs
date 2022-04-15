namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_user_table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "RoleId" });
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.Users", "Birthday", c => c.DateTime());
            AddColumn("dbo.Users", "CreateAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "LastLogin", c => c.DateTime());
            AddColumn("dbo.Users", "FailedLoginAttempt", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "CannotLoginUntil", c => c.DateTime());
            AddColumn("dbo.Users", "LastActivity", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "PictureId", c => c.Int(nullable: false));
            AddColumn("dbo.Roles", "Role_Id", c => c.Int());
            CreateIndex("dbo.Users", "PictureId");
            CreateIndex("dbo.Roles", "Role_Id");
            AddForeignKey("dbo.Users", "PictureId", "dbo.Pictures", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Roles", "Role_Id", "dbo.Roles", "Id");
            DropColumn("dbo.Users", "PictureName");
            DropColumn("dbo.Users", "RoleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "RoleId", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "PictureName", c => c.String());
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Roles", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Users", "PictureId", "dbo.Pictures");
            DropIndex("dbo.Roles", new[] { "Role_Id" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "PictureId" });
            DropColumn("dbo.Roles", "Role_Id");
            DropColumn("dbo.Users", "PictureId");
            DropColumn("dbo.Users", "LastActivity");
            DropColumn("dbo.Users", "CannotLoginUntil");
            DropColumn("dbo.Users", "FailedLoginAttempt");
            DropColumn("dbo.Users", "LastLogin");
            DropColumn("dbo.Users", "CreateAt");
            DropColumn("dbo.Users", "Birthday");
            DropTable("dbo.UserRoles");
            CreateIndex("dbo.Users", "RoleId");
            AddForeignKey("dbo.Users", "RoleId", "dbo.Roles", "Id", cascadeDelete: true);
        }
    }
}
