namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPOExpeditionAdjustmentIsPreOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder", "ExpeditionAdjustment", c => c.String());
            AddColumn("dbo.PurchaseOrder", "IsPreOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "IsPreOrder");
            DropColumn("dbo.PurchaseOrder", "ExpeditionAdjustment");
        }
    }
}
