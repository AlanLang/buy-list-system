namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SysRole",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        RoleCode = c.String(maxLength: 50),
                        RoleName = c.String(maxLength: 50),
                        RoleDesc = c.String(maxLength: 200),
                        RoleModified = c.DateTime(nullable: false),
                        RoleCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleID);
            
            CreateTable(
                "dbo.SysUserRole",
                c => new
                    {
                        UserRoleID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        UserRoleModified = c.DateTime(nullable: false),
                        UserRoleCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserRoleID)
                .ForeignKey("dbo.SysRole", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.SysUser", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
            CreateTable(
                "dbo.SysUser",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserCode = c.String(maxLength: 50),
                        UserName = c.String(maxLength: 50),
                        UserPwd = c.String(maxLength: 35),
                        UserMail = c.String(maxLength: 35),
                        UserModified = c.DateTime(nullable: false),
                        UserCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SysUserRole", "UserID", "dbo.SysUser");
            DropForeignKey("dbo.SysUserRole", "RoleID", "dbo.SysRole");
            DropIndex("dbo.SysUserRole", new[] { "RoleID" });
            DropIndex("dbo.SysUserRole", new[] { "UserID" });
            DropTable("dbo.SysUser");
            DropTable("dbo.SysUserRole");
            DropTable("dbo.SysRole");
        }
    }
}
