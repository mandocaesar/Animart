namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeOnPurchaseOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "Code");
        }
    }
}
