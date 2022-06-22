using System;
using Data.Entities;

namespace Service.DbInitializer
{
    /// <summary>
    /// Static class that holds initial test data to be seeded only if application
    /// running in development environment for development and testing purposes
    /// </summary>
    public static class TestData
    {
        public static readonly User[] Users =
        {
            new User
            {
                Id = new Guid("2b6f10f7-b177-4a64-85af-de55fff46ea2"),
                UserName = "alex_gavr812",
                Email = "gavrilenko@ukr.net",
                Name = "Alexander",
                RegistrationDate = new DateTime(2012, 11, 27, 17, 34, 12)
            },
            new User
            {
                Id = new Guid("dd7aeae4-98a1-45a4-8fc1-0a7f499e18bb"),
                UserName = "inna_36",
                Email = "horinna@example.com",
                Name = "Inna",
                RegistrationDate = new DateTime(2016, 3, 16, 5, 19, 59)
            },
            new User
            {
                Id = new Guid("986c459d-8132-4c1c-9815-a655d9833511"),
                UserName = "user",
                Email = "superboris@gmail.com",
                Name = "Boris",
                RegistrationDate = new DateTime(2007, 5, 2, 20, 29, 42)
            }
        };

        public const string UserPassword = "userpass";

        public static readonly User[] Moderators =
        {
            new User
            {
                Id = new Guid("cf91684e-984d-4acc-bad2-89362a05ae2e"),
                UserName = "moderator",
                Email = "ignach153@gmail.com",
                Name = "Oleg Ignachenko",
                RegistrationDate = new DateTime(2019, 9, 20, 22, 43, 8)
            },
            new User
            {
                Id = new Guid("64860ca0-a0dd-4b11-bef7-26a949b8720a"),
                UserName = "jorden23",
                Email = "niko_jorden43@mail.com",
                Name = "Nikola Jorden",
                RegistrationDate = new DateTime(2013, 4, 17, 15, 18, 28)
            }
        };

        public const string ModerPassword = "moderpass";

        public static readonly Thread[] Threads =
        {
            new Thread
            {
                Id = new Guid("14d6eceb-1f3e-40be-b7ed-14a4f84a345f"),
                Topic = "Cras elementum dignissim est eget malesuada?",
                Closed = false,
                AuthorId = Users[0].Id,
                CreationDate = new DateTime(2021, 8, 14, 3, 18, 20)
            },
            new Thread
            {
                Id = new Guid("5f3d8052-3912-4a78-a75a-aa1d2cbad226"),
                Topic = "Pellentesque venenatis pretium?",
                Closed = false,
                AuthorId = Users[1].Id,
                CreationDate = new DateTime(2017, 6, 9, 11, 40, 41)
            },
            new Thread // closed
            {
                Id = new Guid("d381d823-8f7a-4fc1-978f-ad322360910c"),
                Topic = "Duis ac nulla vitae metus posuere ultricies?",
                Closed = true,
                AuthorId = Users[1].Id,
                CreationDate = new DateTime(2020, 11, 29, 21, 15, 12)
            },
            new Thread // without author (if deleted)
            {
                Id = new Guid("e097cb7b-21ea-42e7-a6cc-05ba77791383"),
                Topic = "Sed metus leo, tincidunt at malesuada quis, vehicula nec nulla?",
                Closed = false,
                CreationDate = new DateTime(2017, 5, 15, 7, 55, 58)
            }
        };

        public static readonly Post[] Posts =
        {
            new Post
            {
                Id = new Guid("d4376327-f24d-423e-9226-8f85117fe117"),
                Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                ThreadId = Threads[0].Id,
                AuthorId = Users[0].Id,
                PublishDate = new DateTime(2021, 8, 14, 19, 25, 31)
            },
            new Post //without user (if deleted)
            {
                Id = new Guid("0db39598-3cf9-4e98-9d21-50a2c55cf5a1"),
                Content = "Nulla eget",
                ThreadId = Threads[0].Id,
                PublishDate = new DateTime(2021, 8, 15, 13, 5, 53)
            },
            new Post
            {
                Id = new Guid("574ae9ca-d72f-4600-801d-f948a5043de8"),
                Content = "Aliquam condimentum enim diam, id eleifend tortor vehicula it amet. " +
                          "Sed dignissim eget libero eget fermentum. Curabitur congue id eros ac " +
                          "tempus. Morbi semper sed lorem gravida congue. Fusce nec libero ut sapien " +
                          "cursus suscipit in a nunc.\nMauris volutpat nulla ut vehicula consequat. " +
                          "Cras orci nisl, vestibulum sed magna et, gravida vestibulum eros. Maecenas " +
                          "nisl massa, semper a tempor in, luctus et leo. Vestibulum pretium risus diam, " +
                          "non aliquet leo egestas ac.\nCras at nisl sodales, facilisis arcu non, elementum quam.",
                ThreadId = Threads[0].Id,
                AuthorId = Users[1].Id,
                PublishDate = new DateTime(2021, 8, 15, 17, 4, 12)
            },
            new Post
            {
                Id = new Guid("ec4cfba5-f75a-4bb4-8969-e29f3da6639b"),
                Content = "Vivamus ante sem, vehicula at euismod eu, luctus eleifend erat.",
                ThreadId = Threads[0].Id,
                AuthorId = Moderators[1].Id,
                PublishDate = new DateTime(2021, 8, 17, 4, 53, 48)
            },
            new Post
            {
                Id = new Guid("32f7f865-82db-4e74-8e80-24d8a8c9c7a6"),
                Content = "Donec mollis, ante placerat mollis porttitor, sapien mi pulvinar " +
                          "elit, ut commodo nisl turpis eget metus.",
                ThreadId = Threads[2].Id,
                AuthorId = Users[1].Id,
                PublishDate = new DateTime(2017, 5, 15, 12, 34, 8)
            }
        };
    }
}