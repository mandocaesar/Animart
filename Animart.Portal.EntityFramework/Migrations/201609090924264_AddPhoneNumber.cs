namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "PhoneNumber", c => c.String());
            AddColumn("dbo.PurchaseOrder", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PurchaseOrder", "PhoneNumber");
            DropColumn("dbo.AbpUsers", "PhoneNumber");
        }
    }
}
