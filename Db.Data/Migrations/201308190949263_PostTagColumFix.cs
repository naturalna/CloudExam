namespace Db.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostTagColumFix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagsPosts",
                c => new
                    {
                        Tags_Id = c.Int(nullable: false),
                        Posts_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tags_Id, t.Posts_Id })
                .ForeignKey("dbo.Tags", t => t.Tags_Id, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.Posts_Id, cascadeDelete: true)
                .Index(t => t.Tags_Id)
                .Index(t => t.Posts_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TagsPosts", new[] { "Posts_Id" });
            DropIndex("dbo.TagsPosts", new[] { "Tags_Id" });
            DropForeignKey("dbo.TagsPosts", "Posts_Id", "dbo.Posts");
            DropForeignKey("dbo.TagsPosts", "Tags_Id", "dbo.Tags");
            DropTable("dbo.TagsPosts");
        }
    }
}
