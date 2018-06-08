namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynewlog3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyNew", "BuyAuthor", c => c.String(maxLength: 30));
            AlterColumn("dbo.BuyNew", "BuyTime", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BuyNew", "BuyTime", c => c.String(maxLength: 20));
            DropColumn("dbo.BuyNew", "BuyAuthor");
        }
    }
}
