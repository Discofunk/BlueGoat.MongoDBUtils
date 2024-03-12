using System.Diagnostics;

namespace BlueGoat.MongoUtils.Test.Integration
{
    public class TimeAction
    {
        public static TimeSpan Execute(Action action)
        {
            var stopWatch = Stopwatch.StartNew();
            action();
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

        public static async Task<TimeSpan> ExecuteAsync(Func<Task> action)
        {
            var stopWatch = Stopwatch.StartNew();
            await action.Invoke();
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

        // public static async Task<TimeSpan> ExecuteAsync<T>(Func<Task<T>> action)
        // {
        //     var stopWatch = Stopwatch.StartNew();
        //     await action.Invoke();
        //     stopWatch.Stop();
        //     return stopWatch.Elapsed;
        // }
    }
}
