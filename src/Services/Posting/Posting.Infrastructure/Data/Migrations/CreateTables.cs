// using FluentMigrator;
//
// namespace Posting.Infrastructure.Data.Migrations
// {
//     [Migration(20200105)]
//     public class CreateTables : AutoReversingMigration
//     {
//         public override void Up()
//         {
//             Create.Table("Posts")
//                 .WithColumn("Id").AsString(36).PrimaryKey()
//                 .WithColumn("UserId").AsString(36).NotNullable().Indexed()
//                 .WithColumn("BlogId").AsString(36).NotNullable().Indexed()
//                 .WithColumn("Title").AsString(200).NotNullable()
//                 .WithColumn("Content").AsString(50000).NotNullable()
//                 .WithColumn("CreatedAt").AsDateTime2().NotNullable()
//                 .WithColumn("UpdatedAt").AsDateTime2()
//                 .WithColumn("Url").AsString(100).NotNullable();
//
//             Create.Table("Reactions")
//                 .WithColumn("Id").AsString(36).PrimaryKey()
//                 .WithColumn("UserId").AsString(36).NotNullable().Indexed()
//                 .WithColumn("Type").AsInt16().NotNullable()
//                 .WithColumn("CreatedAt").AsDateTime2().NotNullable()
//                 .WithColumn("PostId").AsString(36).ForeignKey("FK_Reactions_Posts_PostId", "Posts", "Id").Indexed();
//
//             Create.Table("Comments")
//                 .WithColumn("Id").AsString(36).PrimaryKey()
//                 .WithColumn("UserId").AsString(36).NotNullable().Indexed()
//                 .WithColumn("Content").AsString(2500).NotNullable()
//                 .WithColumn("CreatedAt").AsDateTime2().NotNullable()
//                 .WithColumn("UpdatedAt").AsDateTime2()
//                 .WithColumn("PostId").AsString(36).ForeignKey("FK_Comments_Posts_PostId", "Posts", "Id").Indexed();
//             
//             
//             
//         }
//     }
// }

