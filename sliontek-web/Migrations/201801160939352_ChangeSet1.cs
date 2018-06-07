namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSet1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SysUser", "UserTest", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SysUser", "UserTest");
        }
    }
}
