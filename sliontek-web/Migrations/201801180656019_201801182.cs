namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201801182 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SysRoleMenu", "MenuID", "dbo.SysMenu");
            DropForeignKey("dbo.SysRoleMenu", "RoleID", "dbo.SysRole");
            DropIndex("dbo.SysRoleMenu", new[] { "RoleID" });
            DropIndex("dbo.SysRoleMenu", new[] { "MenuID" });
            CreateTable(
                "dbo.SysRoleMenuLimit",
                c => new
                    {
                        RoleMenuLimitID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        MenuLimitID = c.Int(nullable: false),
                        RoleMenuModified = c.DateTime(nullable: false),
                        RoleMenuCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleMenuLimitID)
                .ForeignKey("dbo.SysMenuLimit", t => t.MenuLimitID, cascadeDelete: true)
                .ForeignKey("dbo.SysRole", t => t.RoleID, cascadeDelete: true)
                .Index(t => t.RoleID)
                .Index(t => t.MenuLimitID);
            
            CreateTable(
                "dbo.SysMenuLimit",
                c => new
                    {
                        MenuLimitID = c.Int(nullable: false, identity: true),
                        MenuID = c.Int(nullable: false),
                        MenuLimitModified = c.DateTime(nullable: false),
                        MenuLimitCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MenuLimitID)
                .ForeignKey("dbo.SysMenu", t => t.MenuID, cascadeDelete: true)
                .Index(t => t.MenuID);
            
            AddColumn("dbo.SysMenu", "MenuModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.SysMenu", "MenuCreate", c => c.DateTime(nullable: false));
            DropTable("dbo.SysRoleMenu");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SysRoleMenu",
                c => new
                    {
                        RoleMenuID = c.Int(nullable: false, identity: true),
                        RoleID = c.Int(nullable: false),
                        MenuID = c.Int(nullable: false),
                        Permission = c.Int(nullable: false),
                        RoleMenuModified = c.DateTime(nullable: false),
                        RoleMenuCreate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoleMenuID);
            
            DropForeignKey("dbo.SysRoleMenuLimit", "RoleID", "dbo.SysRole");
            DropForeignKey("dbo.SysRoleMenuLimit", "MenuLimitID", "dbo.SysMenuLimit");
            DropForeignKey("dbo.SysMenuLimit", "MenuID", "dbo.SysMenu");
            DropIndex("dbo.SysMenuLimit", new[] { "MenuID" });
            DropIndex("dbo.SysRoleMenuLimit", new[] { "MenuLimitID" });
            DropIndex("dbo.SysRoleMenuLimit", new[] { "RoleID" });
            DropColumn("dbo.SysMenu", "MenuCreate");
            DropColumn("dbo.SysMenu", "MenuModified");
            DropTable("dbo.SysMenuLimit");
            DropTable("dbo.SysRoleMenuLimit");
            CreateIndex("dbo.SysRoleMenu", "MenuID");
            CreateIndex("dbo.SysRoleMenu", "RoleID");
            AddForeignKey("dbo.SysRoleMenu", "RoleID", "dbo.SysRole", "RoleID", cascadeDelete: true);
            AddForeignKey("dbo.SysRoleMenu", "MenuID", "dbo.SysMenu", "MenuID", cascadeDelete: true);
        }
    }
}
