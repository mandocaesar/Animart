namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImagesHolderForSupplies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplyItem", "IsPo", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupplyItem", "IsPo");
        }
    }
}
