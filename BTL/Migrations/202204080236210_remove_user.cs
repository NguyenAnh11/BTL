namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_user : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "LastLogin");
            DropColumn("dbo.Users", "FailedLoginAttempt");
            DropColumn("dbo.Users", "CannotLoginUntil");
            DropColumn("dbo.Users", "LastActivity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "LastActivity", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "CannotLoginUntil", c => c.DateTime());
            AddColumn("dbo.Users", "FailedLoginAttempt", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "LastLogin", c => c.DateTime());
        }
    }
}
