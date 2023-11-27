namespace Kursova
{
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
            Application.Run(new Form1());

            File.Create("C:\\Users\\PiwKi\\Desktop\\fs_file");
            const int sectorSize = 1024;//bytes
            const int bitmapSize = sectorSize * 2;

        }
    }
}