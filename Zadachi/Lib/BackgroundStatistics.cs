using Microsoft.Extensions.Caching.Distributed;
using Prometheus;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Zadachi.Models;

namespace Zadachi.Lib
{
    public class BackgroundStatistics : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserStatistics _userStatistics;
        private readonly IDistributedCache _cache;
        private static readonly Counter _executionCounter = Metrics.CreateCounter("backgroundstatistics_executions_total", "Total number of background task executions");
        private static readonly Gauge _taskStatus = Metrics.CreateGauge("backgroundstatistics_status", "Current status of background task (1=running, 0=stopped)");
        private static readonly Histogram _executionDuration = Metrics.CreateHistogram("backgroundstatistics_duration_seconds", "Duration of task execution in seconds", new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(0.1, 0.1, 10)
            });
        public BackgroundStatistics(IServiceProvider serviceProvider, UserStatistics userStatistics, IDistributedCache cache)
        {
            _serviceProvider = serviceProvider;
            _userStatistics = userStatistics;
            _cache = cache;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int num = 0;
            string cache;
            _taskStatus.Set(1);
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope()) 
                using (_executionDuration.NewTimer())
                {
                    _executionCounter.Inc();
                    var scopedProvider = scope.ServiceProvider;
                    var context = scope.ServiceProvider.GetRequiredService<ZadachiDbContext>();
                    string result = $"There are {context.Activities.Count(a => a.User == null && a.IsCompleted)} completed no-user tasks";
                    var users = context.Users;
                    foreach (var user in users)
                    {
                        result += $"\nThere are {context.Activities.Count(a => a.User == user && a.IsCompleted)} completed {user.UserName} tasks";
                    }
                    _userStatistics.Statistics.Enqueue(result);
                    _cache.SetString($"BackgroundStatistics{num}", result);
                    //cache = _cache.GetString($"BackgroundStatistics{num}");
                    num++;
                }         
                Debug.WriteLine(_userStatistics.Statistics.Last());
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            _taskStatus.Set(0);
        }
    }
}
