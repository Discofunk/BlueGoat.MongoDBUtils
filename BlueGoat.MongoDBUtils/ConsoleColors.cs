namespace BlueGoat.MongoDBUtils
{
    public class ConsoleEx : IConsole
    {
        public void WriteLine(string? message)
        {
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public void WriteLineOk(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteLineInfo(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteLineError(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void WriteLineWarn(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Write(string? message)
        {
            Console.ResetColor();
            Console.Write(message);
        }

        public void WriteOk(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }

        public void WriteInfo(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
            Console.ResetColor();
        }

        public void WriteError(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }

        public void WriteWarn(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }
    }

}
