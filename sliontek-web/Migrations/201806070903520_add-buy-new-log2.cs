namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynewlog2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyNewChangeLog",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BuyNewID = c.Int(nullable: false),
                        LogMsg = c.String(maxLength: 50),
                        Person = c.String(maxLength: 30),
                        Create = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BuyNewChangeLog");
        }
    }
}
