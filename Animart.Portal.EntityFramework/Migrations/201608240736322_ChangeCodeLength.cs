namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCodeLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SupplyItem", "Code", c => c.String(nullable: false, maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SupplyItem", "Code", c => c.String(nullable: false, maxLength: 10));
        }
    }
}
