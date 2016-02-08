using System.ComponentModel.DataAnnotations.Schema;

namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    [Table("Shipment")]
    public partial class Shipment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.City",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId)
                .Index(t => t.CreatorUserId);
            
            CreateTable(
                "dbo.Expedition",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId)
                .Index(t => t.CreatorUserId);
            
            CreateTable(
                "dbo.ShipmentCost",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Type = c.String(),
                        First5Kilo = c.Int(nullable: false),
                        NextKilo = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                        City_Id = c.Guid(),
                        Expedition_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.City", t => t.City_Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.Expedition", t => t.Expedition_Id)
                .Index(t => t.CreatorUserId)
                .Index(t => t.City_Id)
                .Index(t => t.Expedition_Id);
           
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShipmentCost", "Expedition_Id", "dbo.Expedition");
            DropForeignKey("dbo.ShipmentCost", "CreatorUserId", "dbo.AbpUsers");
            DropForeignKey("dbo.ShipmentCost", "City_Id", "dbo.City");
            DropForeignKey("dbo.Expedition", "CreatorUserId", "dbo.AbpUsers");
            DropForeignKey("dbo.City", "CreatorUserId", "dbo.AbpUsers");
            DropIndex("dbo.ShipmentCost", new[] { "Expedition_Id" });
            DropIndex("dbo.ShipmentCost", new[] { "City_Id" });
            DropIndex("dbo.ShipmentCost", new[] { "CreatorUserId" });
            DropIndex("dbo.Expedition", new[] { "CreatorUserId" });
            DropIndex("dbo.City", new[] { "CreatorUserId" });
            DropColumn("dbo.SupplyItem", "Weight");
            DropTable("dbo.ShipmentCost");
            DropTable("dbo.Expedition");
            DropTable("dbo.City");
        }
    }
}
