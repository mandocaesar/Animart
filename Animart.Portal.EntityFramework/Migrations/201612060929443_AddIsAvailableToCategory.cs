namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsAvailableToCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "IsAvailable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Category", "IsAvailable");
        }
    }
}
