namespace Db.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 30),
                        DisplayName = c.String(),
                        AuthCode = c.String(maxLength: 40),
                        SessionKey = c.String(maxLength: 50, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        PostDate = c.Int(nullable: false),
                        Text = c.String(nullable: false, storeType: "ntext"),
                        PostedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.PostedBy_Id)
                .Index(t => t.PostedBy_Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false, storeType: "ntext"),
                        PostDate = c.DateTime(nullable: false),
                        Post_Id = c.Int(),
                        CommentedBy_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.Post_Id)
                .ForeignKey("dbo.Users", t => t.CommentedBy_Id)
                .Index(t => t.Post_Id)
                .Index(t => t.CommentedBy_Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Comments", new[] { "CommentedBy_Id" });
            DropIndex("dbo.Comments", new[] { "Post_Id" });
            DropIndex("dbo.Posts", new[] { "PostedBy_Id" });
            DropForeignKey("dbo.Comments", "CommentedBy_Id", "dbo.Users");
            DropForeignKey("dbo.Comments", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Posts", "PostedBy_Id", "dbo.Users");
            DropTable("dbo.Tags");
            DropTable("dbo.Comments");
            DropTable("dbo.Posts");
            DropTable("dbo.Users");
        }
    }
}
