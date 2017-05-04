namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterShipment : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShipmentCost", "Expedition_Id", "dbo.Expedition");
            DropIndex("dbo.ShipmentCost", new[] { "Expedition_Id" });
            AddColumn("dbo.ShipmentCost", "Expedition", c => c.String());
            DropColumn("dbo.ShipmentCost", "Expedition_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentCost", "Expedition_Id", c => c.Guid());
            DropColumn("dbo.ShipmentCost", "Expedition");
            CreateIndex("dbo.ShipmentCost", "Expedition_Id");
            AddForeignKey("dbo.ShipmentCost", "Expedition_Id", "dbo.Expedition", "Id");
        }
    }
}
