namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynewlog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BuyNew", "BuyTime", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BuyNew", "BuyTime", c => c.String());
        }
    }
}
