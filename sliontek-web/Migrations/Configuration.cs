namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<sliontek_web.Repositories.EFContext>
    {
        public Configuration()
        {
            // 采用手动数据库迁移
            //1. Enable-Migrations
            //2. Add-Migration  thename
            //3. Update-Database
            AutomaticMigrationsEnabled = false;
            ContextKey = "sliontek_web.Repositories.EFContext";
        }

        protected override void Seed(sliontek_web.Repositories.EFContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
