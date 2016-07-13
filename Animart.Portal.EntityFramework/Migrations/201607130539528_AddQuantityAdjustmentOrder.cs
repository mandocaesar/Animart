namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuantityAdjustmentOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "QuantityAdjustment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "QuantityAdjustment");
        }
    }
}
