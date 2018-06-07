using sliontek_web.Migrations;
using sliontek_web.Model;
using sliontek_web.Model.Def;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace sliontek_web.Repositories
{
    public class EFContext : DbContext
    {
        public EFContext() : base("SqlConnectionString")
        {
            //禁止进行自动数据迁移
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<EFContext, Configuration>("SqlConnectionString"));
        }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysUserRole> SysUserRole { get; set; }
        public DbSet<SysMenu> SysMenu { get; set; }
        public DbSet<SysMenuLimit> SysMenuLimit { get; set; }
        public DbSet<SysRoleMenuLimit> SysRoleMenuLimit { get; set; }
        public DbSet<DefBuyType> DefBuyType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<SysMenuNavbar>(); // 忽略建表
            // 取消自动生成表名时变成复数形式
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
    }
}