using coinbot.strategies.Contracts.Models;

namespace coin_bot.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = @"";
            var fileManager = new FileRepository.FileRepository();
            var config = fileManager.GetDataFromFile<Settings>(configPath);
        }
    }
}
