namespace sliontek_web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbuynew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyNew",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BuyName = c.String(maxLength: 30),
                        BuyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BuyUrl = c.String(maxLength: 50),
                        BuyTypeName = c.String(maxLength: 30),
                        BuyLevel = c.String(maxLength: 30),
                        BuyCheckPerson = c.String(maxLength: 90),
                        BuyTime = c.String(),
                        TypeDesc = c.String(maxLength: 90),
                        Modified = c.DateTime(nullable: false),
                        Create = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BuyNew");
        }
    }
}
