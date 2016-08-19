namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        ParentId = c.Guid(),
                        CreationTime = c.DateTime(nullable: false),
                        CreatorUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.CreatorUserId)
                .ForeignKey("dbo.Category", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.CreatorUserId);
            
            AddColumn("dbo.SupplyItem", "CategoryId", c => c.Guid());
            CreateIndex("dbo.SupplyItem", "CategoryId");
            AddForeignKey("dbo.SupplyItem", "CategoryId", "dbo.Category", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplyItem", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Category", "ParentId", "dbo.Category");
            DropForeignKey("dbo.Category", "CreatorUserId", "dbo.AbpUsers");
            DropIndex("dbo.SupplyItem", new[] { "CategoryId" });
            DropIndex("dbo.Category", new[] { "CreatorUserId" });
            DropIndex("dbo.Category", new[] { "ParentId" });
            DropColumn("dbo.SupplyItem", "CategoryId");
            DropTable("dbo.Category");
        }
    }
}
