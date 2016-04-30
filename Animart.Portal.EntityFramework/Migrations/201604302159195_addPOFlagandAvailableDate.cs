namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPOFlagandAvailableDate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SupplyImage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SupplyItemId = c.Guid(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId)
                .Index(t => t.CreatorUserId);
            
            AddColumn("dbo.SupplyItem", "AvailableUntil", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplyImage", "CreatorUserId", "dbo.AbpUsers");
            DropIndex("dbo.SupplyImage", new[] { "CreatorUserId" });
            DropColumn("dbo.SupplyItem", "AvailableUntil");
            DropTable("dbo.SupplyImage");
        }
    }
}
