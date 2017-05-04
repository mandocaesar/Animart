namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameAndPriceForOrderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "Name", c => c.String());
            AddColumn("dbo.OrderItems", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "Price");
            DropColumn("dbo.OrderItems", "Name");
        }
    }
}
