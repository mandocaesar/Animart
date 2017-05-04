namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostalCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PurchaseOrder", "PostalCode", c => c.String(maxLength: 5));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "PostalCode");
        }
    }
}
