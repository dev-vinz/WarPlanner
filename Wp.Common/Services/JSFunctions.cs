using Timer = System.Timers.Timer;

namespace Wp.Common.Services
{
    public static class JSFunctions
    {
        /// <summary>
        /// Clears a timer interval
        /// </summary>
        /// <param name="timer">The timer to clear</param>
        public static void ClearInterval(ref Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// Sets a timer where an action is executed after a specific interval
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="timeout">The timeout after each tick the action is executed</param>
        /// <returns>A timer containing the action beeing executed after each timeout</returns>
        public static Timer SetInterval(Action action, TimeSpan timeout)
        {
            Timer timer = new(timeout.TotalMilliseconds);

            //timer.Elapsed += (s, e) =>
            //{
            //    timer.Enabled = false;
            //    action();
            //    timer.Enabled = true;
            //};

            timer.Elapsed += (_, _) => action();
            timer.AutoReset = true;
            timer.Start();

            return timer;
        }

        /// <summary>
        /// Sets a timeout before an action is executed
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="timeout">The timeout after the action is executed</param>
        public static async Task SetTimeoutAsync(Action action, TimeSpan timeout)
        {
            await Task
                .Delay(timeout)
                .ConfigureAwait(false);

            action();
        }
    }
}
