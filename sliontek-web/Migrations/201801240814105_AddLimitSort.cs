namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLimitSort : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SysMenuLimit", "MenuLimitSort", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SysMenuLimit", "MenuLimitSort");
        }
    }
}
