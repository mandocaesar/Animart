namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImageFlagforSupply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplyItem", "HasImage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupplyItem", "HasImage");
        }
    }
}
