using System;
using System.Collections.Generic;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Tests
{
    public static class UnitTestHelper
    {
        public static DbContextOptions<ForumContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ForumContext(options);
            
            SeedTestData(context);

            return options;
        }
        
        private static void SeedTestData(ForumContext context)
        {
            context.Users.AddRange(UserList);
            context.Threads.AddRange(ThreadList);
            context.Posts.AddRange(PostList);
            
            context.SaveChanges();
        }

        private static IEnumerable<User> UserList =>
            new List<User>
            {
                new User
                {
                    Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    UserName = "username1",
                    Email = "email1@example.com",
                    Name = "name1",
                    RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
                },
                new User
                {
                    Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    UserName = "username2",
                    Email = "email2@example.com",
                    Name = "name2",
                    RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
                },
                new User
                {
                    Id = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    UserName = "username3",
                    Email = "email3@example.com",
                    Name = "name3",
                    RegistrationDate = new DateTime(2005, 6, 3, 9, 12, 11)
                }
            };
        
        private static IEnumerable<Thread> ThreadList =>
            new List<Thread>
            {
                new Thread
                {
                    Id = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    Topic = "Thread topic 1",
                    Closed = false,
                    CreationDate = new DateTime(2012, 12, 8, 5, 16, 30),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2")
                },
                new Thread
                {
                    Id = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    Topic = "Thread topic 2",
                    Closed = true,
                    CreationDate = new DateTime(2017, 3, 3, 5, 12, 51),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb")
                },
                new Thread
                {
                    Id = new Guid("5891e6dc-09ec-4883-9040-80c38c0318ab"),
                    Topic = "Thread topic 3",
                    Closed = false,
                    CreationDate = new DateTime(2016, 9, 24, 9, 18, 11),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb")
                }
            };
        
        private static IEnumerable<Post> PostList =>
            new List<Post>
            {
                new Post
                {
                    Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                    Content = "Post content 1",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2013, 1, 5, 19, 25, 31)
                },
                new Post
                {
                    Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                    Content = "Post content 2",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                    PublishDate = new DateTime(2012, 12, 10, 13, 5, 53)
                },
                new Post
                {
                    Id = new Guid("074b6e15-965b-4a06-add1-302014c4e589"),
                    Content = "Post content 3",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2017, 3, 4, 9, 46, 9)
                },
                new Post
                {
                    Id = new Guid("7e845814-1b72-45ca-852e-01311adab752"),
                    Content = "Post content 4",
                    ThreadId = new Guid("10ceb8e3-b160-4b28-b237-1ecd448a52d3"),
                    AuthorId = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                    PublishDate = new DateTime(2013, 1, 8, 2, 20, 3)
                },
                new Post
                {
                    Id = new Guid("61b61787-3488-48c1-bf3c-e76b1731f77f"),
                    Content = "Post content 5",
                    ThreadId = new Guid("0a793cc1-0f4f-4766-86e3-2d1f30e03a85"),
                    AuthorId = new Guid("6bc56cad-0687-427a-a836-435d25af8575"),
                    PublishDate = new DateTime(2017, 3, 3, 5, 18, 44)
                }
            };
    }
}