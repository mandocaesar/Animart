namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalWeightAndGrandTotalToInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "TotalWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Invoices", "GrandTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "GrandTotal");
            DropColumn("dbo.Invoices", "TotalWeight");
        }
    }
}
