namespace ChatApp.Shared.Tests.Setup
{
    public static class TestConfig
    {
        // Logged-in user: david@example.com
        public static Guid LoggedUserId = Guid.Parse("98778b84-6108-45c0-b4b9-a7ac71059ce5");

        public static class UserAlice
        {
            public static Guid Id = Guid.Parse("a6e58f6e-fef3-458c-a26c-19cfc14d329e");
        }

        public static class UserBob
        {
            public static Guid Id = Guid.Parse("9151458c-3010-4463-8d40-c3722f061704");
        }

        public static class UserCharlie
        {
            public static Guid Id = Guid.Parse("67edf34c-2203-4052-9c0b-9eb400d9d6e2");
        }

        public static class UserEve
        {
            public static Guid Id = Guid.Parse("ff32d4d0-86ca-41be-9157-9a2ce3d5bcd4");
        }

        public static class StudyGroupChat
        {
            public static Guid Id = Guid.Parse("C653AF32-7980-4480-ACF2-708A0653ECBA");

            public static Guid DavidMessageId = Guid.Parse("cf1038bd-3680-41dd-9cb4-ed4a7644f8b8");
        }

        public static class AlicePrivateChat
        {
            public static Guid Id = Guid.Parse("4398407C-7AAB-40AA-A88A-618B7E4F5701");
        }
    }
}