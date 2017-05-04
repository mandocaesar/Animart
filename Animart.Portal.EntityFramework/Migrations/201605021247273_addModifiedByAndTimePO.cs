namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addModifiedByAndTimePO : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.PurchaseOrder", "ModifiedBy_Id", c => c.Long());
            CreateIndex("dbo.PurchaseOrder", "ModifiedBy_Id");
            AddForeignKey("dbo.PurchaseOrder", "ModifiedBy_Id", "dbo.AbpUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseOrder", "ModifiedBy_Id", "dbo.AbpUsers");
            DropIndex("dbo.PurchaseOrder", new[] { "ModifiedBy_Id" });
            DropColumn("dbo.PurchaseOrder", "ModifiedBy_Id");
            DropColumn("dbo.PurchaseOrder", "ModifiedOn");
        }
    }
}
