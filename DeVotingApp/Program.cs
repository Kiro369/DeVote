namespace DeVotingApp
{
    /// <summary>
    /// TODO: Re-do this project using MAUI when it's released to maintain cross-platform/ability
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Task.Factory.StartNew(DeVote.Program.Main);
            Application.Run(new Main());
        }
    }
} 