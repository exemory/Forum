using System.Collections.Generic;
using Bogus;
using Bogus.Extensions;
using Data.Entities;

namespace Service.DbInitializer
{
    /// <summary>
    /// Static class that contains initial test data for development and testing purposes
    /// </summary>
    public static class TestData
    {
        public const string UserPassword = "userpass";

        public static IEnumerable<User> Users { get; private set; }
        public static IEnumerable<Thread> Threads { get; private set; }
        public static IEnumerable<Post> Posts { get; private set; }
        public static string RandomRole => new Faker().PickRandom("User", "Moderator");

        /// <summary>
        /// Static constructor for initializing <see cref="TestData"/> class
        /// </summary>
        static TestData()
        {
            InitializeData();
        }

        /// <summary>
        /// Initializes testing data
        /// </summary>
        private static void InitializeData()
        {
            var posts = new List<Post>();

            var userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserName, f => f.Person.UserName.ClampLength(max: 15))
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Name,
                    f => f.PickRandom(f.Person.FirstName, f.Person.FullName).ClampLength(max: 20).OrNull(f, 0.2f))
                .RuleFor(u => u.RegistrationDate, f => f.Date.Past(15));

            var postsFaker = new Faker<Post>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Content, f => f.Lorem.Text())
                .RuleFor(t => t.AuthorId, f => f.PickRandom(Users).Id.OrNull(f, 0.1f))
                .RuleFor(t => t.PublishDate, f => f.Date.Past(15));

            var threadsFaker = new Faker<Thread>()
                .RuleFor(t => t.Id, f => f.Random.Guid())
                .RuleFor(t => t.Topic, f => f.Lorem.Sentences(f.Random.Int(1, 2), " "))
                .RuleFor(t => t.Closed, f => f.Random.Bool(0.2f))
                .RuleFor(t => t.AuthorId, f => f.PickRandom(Users).Id.OrNull(f, 0.1f))
                .RuleFor(t => t.CreationDate, f => f.Date.Past(15))
                .RuleFor(t => t.Posts, (f, t) =>
                {
                    postsFaker.RuleFor(p => p.ThreadId, _ => t.Id);
                    var threadPosts = postsFaker.GenerateBetween(0, 6);
                    posts.AddRange(threadPosts);
                    return threadPosts;
                });

            Users = userFaker.Generate(29);
            Threads = threadsFaker.Generate(10);
            Posts = posts;
        }
    }
}