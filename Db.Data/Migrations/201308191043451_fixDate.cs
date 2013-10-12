namespace Db.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "PostDate", c => c.DateTime());
            AlterColumn("dbo.Comments", "PostDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "PostDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Posts", "PostDate", c => c.DateTime(nullable: false));
        }
    }
}
