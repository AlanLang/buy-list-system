namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsysmenulimit1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SysMenuLimit", "MenuLimitCode", c => c.String(maxLength: 20));
            AddColumn("dbo.SysMenuLimit", "MenuLimitName", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SysMenuLimit", "MenuLimitName");
            DropColumn("dbo.SysMenuLimit", "MenuLimitCode");
        }
    }
}
