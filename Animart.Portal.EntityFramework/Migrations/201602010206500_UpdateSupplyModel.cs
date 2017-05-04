namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSupplyModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplyItem", "Weight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplyItem", "Weigth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SupplyItem", "Weigth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SupplyItem", "Weight");
        }
    }
}
