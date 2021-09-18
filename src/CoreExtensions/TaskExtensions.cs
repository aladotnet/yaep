namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task<T> AsTaskFromResult<T>(this T value)
        {
            return Task.FromResult(value);
        }

        /// <summary>
        /// gets the result from a Task calling GetAwaiter().GetResult()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T GetAwaiterResult<T>(this Task<T> task)
        {            
            return task.GetAwaiter().GetResult();
        }
    }
}
