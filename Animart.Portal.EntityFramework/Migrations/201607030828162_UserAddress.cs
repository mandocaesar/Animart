namespace Animart.Portal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpUsers", "Address");
        }
    }
}
