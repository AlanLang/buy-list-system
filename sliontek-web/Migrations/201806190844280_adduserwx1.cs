namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserwx1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyNewChangeLog", "ChangeFrom", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuyNewChangeLog", "ChangeFrom");
        }
    }
}
