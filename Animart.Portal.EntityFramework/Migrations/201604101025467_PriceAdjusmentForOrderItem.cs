namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceAdjusmentForOrderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "PriceAdjustment", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "PriceAdjustment");
        }
    }
}
