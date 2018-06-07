namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuytype : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DefBuyType",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(maxLength: 30),
                        TypeDesc = c.String(maxLength: 50),
                        Modified = c.DateTime(nullable: false),
                        Create = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DefBuyType");
        }
    }
}
