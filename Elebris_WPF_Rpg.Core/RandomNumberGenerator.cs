namespace Elebris_WPF_Rpg.Core
{
    public static class RandomNumberGenerator
    {
        private static readonly Random _simpleGenerator = new Random();
        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            return _simpleGenerator.Next(minimumValue, maximumValue + 1);
        }
    }
}
