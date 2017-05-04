namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReceiptNumberField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder", "ReceiptNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "ReceiptNumber");
        }
    }
}
