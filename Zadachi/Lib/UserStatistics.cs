using System.Collections.Concurrent;

namespace Zadachi.Lib
{
    public class UserStatistics
    {
        public ConcurrentQueue<string> Statistics { get; set; } = new();
    }
}
