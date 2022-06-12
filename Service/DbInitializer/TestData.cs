using System;
using Data.Entities;

namespace Service.DbInitializer
{
    public static class TestData
    {
        public static readonly User[] Users =
        {
            new User
            {
                Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                UserName = "alex_gavr812",
                Email = "gavrilenko@ukr.net",
                Name = "Alexander"
            },
            new User
            {
                Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                UserName = "inna_82",
                Email = "horinna@example.com",
                Name = "Inna"
            },
            new User
            {
                Id = new Guid("986c459d-8132-4c1c-9815-a655d9833511"),
                UserName = "superboris43",
                Email = "superboris@gmail.com",
                Name = "Boris"
            }
        };

        public const string UserPassword = "userpassword123";

        public static readonly User[] Moderators =
        {
            new User
            {
                Id = new Guid("cf91684e-984d-4acc-bad2-89362a05ae2e"),
                UserName = "oleg_ignachenko",
                Email = "ignach153@gmail.com",
                Name = "Oleg"
            },
            new User
            {
                Id = new Guid("64860ca0-a0dd-4b11-bef7-26a949b8720a"),
                UserName = "johare43",
                Email = "johare43@mail.com",
                Name = "Nikola"
            }
        };

        public const string ModerPassword = "moderpassword123";

        public static readonly Thread[] Threads =
        {
            new Thread
            {
                Id = new Guid("14d6eceb-1f3e-40be-b7ed-14a4f84a345f"),
                Topic = "Cras elementum dignissim est eget malesuada?",
                Closed = false,
                AuthorId = Users[0].Id
            },
            new Thread
            {
                Id = new Guid("5f3d8052-3912-4a78-a75a-aa1d2cbad226"),
                Topic = "Pellentesque venenatis pretium?",
                Closed = false,
                AuthorId = Users[1].Id
            },
            new Thread // closed
            {
                Id = new Guid("d381d823-8f7a-4fc1-978f-ad322360910c"),
                Topic = "Duis ac nulla vitae metus posuere ultricies?",
                Closed = true,
                AuthorId = Users[1].Id
            },
            new Thread // without author (if deleted)
            {
                Id = new Guid("e097cb7b-21ea-42e7-a6cc-05ba77791383"),
                Topic = "Sed metus leo, tincidunt at malesuada quis, vehicula nec nulla?",
                Closed = false
            }
        };

        public static readonly Post[] Posts =
        {
            new Post
            {
                Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                ThreadId = Threads[0].Id,
                UserId = Users[0].Id
            },
            new Post
            {
                Id = new Guid("32f7f865-82db-4e74-8e80-24d8a8c9c7a6"),
                Content = "Donec mollis, ante placerat mollis porttitor, sapien mi pulvinar " +
                          "elit, ut commodo nisl turpis eget metus.",
                ThreadId = Threads[0].Id,
                UserId = Users[1].Id
            },
            new Post
            {
                Id = new Guid("574ae9ca-d72f-4600-801d-f948a5043de8"),
                Content = "Aliquam condimentum enim diam, id eleifend tortor vehicula it amet. " +
                          "Sed dignissim eget libero eget fermentum. Curabitur congue id eros ac " +
                          "tempus. Morbi semper sed lorem gravida congue. Fusce nec libero ut sapien " +
                          "cursus suscipit in a nunc. Mauris volutpat nulla ut vehicula consequat. " +
                          "Cras orci nisl, vestibulum sed magna et, gravida vestibulum eros. Maecenas " +
                          "nisl massa, semper a tempor in, luctus et leo. Vestibulum pretium risus diam, " +
                          "non aliquet leo egestas ac. Cras at nisl sodales, facilisis arcu non, elementum quam.",
                ThreadId = Threads[0].Id,
                UserId = Users[0].Id
            },
            new Post
            {
                Id = new Guid("ec4cfba5-f75a-4bb4-8969-e29f3da6639b"),
                Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                ThreadId = Threads[2].Id,
                UserId = Users[0].Id
            },
            new Post //without user (if deleted)
            {
                Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                Content = "Nulla eget",
                ThreadId = Threads[0].Id
            }
        };
    }
}