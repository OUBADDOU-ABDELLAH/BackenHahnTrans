using HahnTransportAutomate.Services.Interfaces;

namespace HahnTransportAutomate.Services
{
    public class SimServer : BackgroundService
    {
        private readonly TimeSpan simTick;
        private ISimService simService;
        public SimServer(ISimService simService)
        {
            simTick = TimeSpan.Parse("00:00:10");
            this.simService = simService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tick = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var startTime = DateTime.Now;
                tick++;
                await simService.RunTick(tick);
                await Task.Run(() => WaitForTick(DateTime.Now - startTime), stoppingToken);
            }
        }
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    var tick = 0;
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var startTime = DateTime.Now;
        //        tick++;

        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var simService = scope.ServiceProvider.GetRequiredService<ISimService>();
        //            await simService.RunTick(tick);
        //        }

        //        await Task.Run(() => WaitForTick(DateTime.Now - startTime), stoppingToken);
        //    }
        //}

        private void WaitForTick(TimeSpan timePassed)
        {
            var timeToNextTick = simTick.Subtract(timePassed);
            var timeToNextTickInMilliseconds = (int)timeToNextTick.TotalMilliseconds;
            if (timeToNextTickInMilliseconds < 0)
            {
                timeToNextTickInMilliseconds = 0;
            }
            Thread.Sleep(timeToNextTickInMilliseconds);
        }
    }
}
