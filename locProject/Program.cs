using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace EFTestApp
{
    public class Program
    {
        public static void Main()
        {
            using (var db = new BloggingContext())
            {
                var VSBlog = new Blog
                {
                    Url = "http://visualstudio.microsoft.com"
                };

                var PostOne = new Post
                {
                    Title = "Visual Studio Performance Profiler",
                    Content = "INSERT_CONTENT_HERE",
                    Blog = VSBlog
                };

                var PostTwo = new Post
                {
                    Title = ".NET Object Allocation Tool",
                    Content = "lorem ipsum something something",
                    Blog = VSBlog
                };

                db.Blogs.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                db.Posts.Add(PostOne);
                db.Posts.Add(PostTwo);
                db.Blogs.Add(VSBlog);
                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);

                Console.WriteLine("All blogs in database:");
                while (true)
                {
                    foreach (var blog in db.Blogs)
                    {
                        Console.WriteLine();
                        Console.WriteLine(blog.Url);
                        var blogPosts = db.Posts.Where(p => p.Blog.BlogId == blog.BlogId);
                        Console.WriteLine("Number of posts: {0}", blogPosts.Count());
                        foreach (var post in blogPosts)
                        {
                            Console.WriteLine("{0}: {1}", post.Title, post.Content);
                        }
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = "";
            string localPath = "../../../DatabaseSQLiteDb.db";
            string containerPath = "./DatabaseSQLiteDb.db";

            if (File.Exists(localPath))
            {
                path = localPath;
            }
            else if (File.Exists(containerPath))
            {
                path = containerPath;
            }
            else
            {
                throw new Exception("Unable to find database file");
            }

            optionsBuilder.UseSqlite($"Data Source={Path.GetFullPath(path)}");
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Blog Blog { get; set; }
    }
}