namespace BTL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_birthday_table_user : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "Birthday");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Birthday", c => c.DateTime());
        }
    }
}
