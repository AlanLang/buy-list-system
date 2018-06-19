namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserwx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SysUser", "UserWx", c => c.String(maxLength: 35));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SysUser", "UserWx");
        }
    }
}
