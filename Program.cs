namespace PackLogger
{
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var packLogger = new PackLogger();
            Console.WriteLine("Starting watching packs");
            await packLogger.Start();
        }
    }
}
