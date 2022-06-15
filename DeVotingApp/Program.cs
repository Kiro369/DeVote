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
           // Task.Factory.StartNew(DeVote.Program.Main);
            Application.Run(new VotingForm(new IDInfo() { Address = "عين شمس", FrontIDPath = @"C:\Users\Kiro\source\repos\DeVote\Recognition\front.jpg", ID = "30001130100113", LastName = "تادرس ذكري تادرس", Name = "كيرلس" }, new List<string>()));
        }
    }
} 