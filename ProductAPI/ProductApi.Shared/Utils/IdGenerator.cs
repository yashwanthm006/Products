using System;

namespace ProductApi.Shared.Utils
{
    public static class IdGenerator
    {
        private static readonly Random _random = new Random();
        public static string GenerateUniqueId()
        {
            return _random.Next(100000, 999999).ToString();
        }
    }
}
