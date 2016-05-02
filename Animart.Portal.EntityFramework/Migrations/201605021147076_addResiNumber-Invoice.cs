namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addResiNumberInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "ResiNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "ResiNumber");
        }
    }
}
