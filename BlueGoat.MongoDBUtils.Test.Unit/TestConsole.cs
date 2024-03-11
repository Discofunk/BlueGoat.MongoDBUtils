using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace BlueGoat.MongoDBUtils.Test.Unit
{
    public class TestConsole : IConsole
    {
        private readonly ITestOutputHelper output;
        private readonly Queue<string> outputs = new();
        private readonly Queue<string> inputs = new();

        public IReadOnlyList<string> Outputs => outputs.ToList().AsReadOnly();

        public TestConsole(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void AddNextInput(string input)
        {
            inputs.Enqueue(input);
        }

        public void WriteLine(string message)
        {
            outputs.Enqueue(message);
            output.WriteLine(message);
        }

        public void WriteLineOk(string message)
        {
            WriteLine(message);
        }

        public void WriteLineInfo(string message)
        {
            WriteLine(message);
        }

        public void WriteLineError(string message)
        {
            WriteLine(message);
        }

        public void WriteLineWarn(string message)
        {
            WriteLine(message);
        }

        public void Write(string message)
        {
            WriteLine(message);
        }

        public void WriteOk(string message)
        {
            WriteLine(message);
        }

        public void WriteInfo(string message)
        {
            WriteLine(message);
        }

        public void WriteError(string message)
        {
            WriteLine(message);
        }

        public void WriteWarn(string message)
        {
            WriteLine(message);
        }

        public string ReadLine()
        {
            var nextInput = inputs.Dequeue();
            output.WriteLine(nextInput);
            return nextInput;
        }
    }
}
