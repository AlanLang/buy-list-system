namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynew1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyNew", "BuyState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuyNew", "BuyState");
        }
    }
}
