namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvoiceToOrderItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Invoices", "PurchaseOrder_Id", "dbo.PurchaseOrder");
            DropIndex("dbo.Invoices", new[] { "PurchaseOrder_Id" });
            AddColumn("dbo.OrderItems", "InvoiceId", c => c.Guid());
            AddColumn("dbo.Invoices", "Expedition", c => c.String());
            CreateIndex("dbo.OrderItems", "InvoiceId");
            AddForeignKey("dbo.OrderItems", "InvoiceId", "dbo.Invoices", "Id");
            DropColumn("dbo.Invoices", "PurchaseOrder_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "PurchaseOrder_Id", c => c.Guid());
            DropForeignKey("dbo.OrderItems", "InvoiceId", "dbo.Invoices");
            DropIndex("dbo.OrderItems", new[] { "InvoiceId" });
            DropColumn("dbo.Invoices", "Expedition");
            DropColumn("dbo.OrderItems", "InvoiceId");
            CreateIndex("dbo.Invoices", "PurchaseOrder_Id");
            AddForeignKey("dbo.Invoices", "PurchaseOrder_Id", "dbo.PurchaseOrder", "Id");
        }
    }
}
