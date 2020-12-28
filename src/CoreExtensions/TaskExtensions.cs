namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static Task<T> AsTaskFromResult<T>(this T value)
        {
            return Task.FromResult(value);
        }
    }
}
