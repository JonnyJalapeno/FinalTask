namespace Tests.Sources
{
    public static class CredentialsSource
    {
        public static IEnumerable<object[]> InvalidCredentials =>
            new List<object[]>
            {
                new object[] { "ddd", "eee" }
            };

        public static IEnumerable<object[]> ValidCredentials =>
            new List<object[]>
            {
                new object[] { "standard_user", "secret_sauce" }
            };
    }
}

