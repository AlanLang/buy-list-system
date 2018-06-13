namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class buycheck01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyNewChangeLog", "LogStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuyNewChangeLog", "LogStatus");
        }
    }
}
