namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaperTest : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SysUser", "UserTest");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SysUser", "UserTest", c => c.String());
        }
    }
}
