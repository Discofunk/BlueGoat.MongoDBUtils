namespace BlueGoat.MongoDBUtils
{
    public static class ConsoleEx
    {

        public static void WriteLine(string? message)
        {
            Console.ResetColor();
            Console.WriteLine(message);
        }

        public static void WriteLineOk(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteLineInfo(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteLineError(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteLineWarn(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Write(string? message)
        {
            Console.ResetColor();
            Console.Write(message);
        }

        public static void WriteOk(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteInfo(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteError(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void WriteWarn(string? message)
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();
        }
    }

}
