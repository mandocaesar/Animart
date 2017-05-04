namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShipmentKiloQuantity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentCost", "KiloQuantity", c => c.Int(nullable: false));
            RenameColumn("dbo.ShipmentCost", "First5Kilo", "FirstKilo");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.ShipmentCost", "FirstKilo", "First5Kilo");
            DropColumn("dbo.ShipmentCost", "KiloQuantity");
        }
    }
}
