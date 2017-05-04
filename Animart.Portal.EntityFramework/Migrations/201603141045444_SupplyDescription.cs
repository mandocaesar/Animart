namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SupplyDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplyItem", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupplyItem", "Description");
        }
    }
}
