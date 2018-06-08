namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynewlog4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyNew", "BuyDesc", c => c.String(maxLength: 90));
            DropColumn("dbo.BuyNew", "TypeDesc");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BuyNew", "TypeDesc", c => c.String(maxLength: 90));
            DropColumn("dbo.BuyNew", "BuyDesc");
        }
    }
}
