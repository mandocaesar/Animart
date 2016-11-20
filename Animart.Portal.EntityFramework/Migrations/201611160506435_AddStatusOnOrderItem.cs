namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatusOnOrderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "Status");
        }
    }
}
