using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Quartz.Spi;
using Quartz;

namespace QuartzJobDemo;

public class LineJobFactory : IJobFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LineJobFactory> _logger;

    public LineJobFactory(IServiceProvider serviceProvider, ILogger<LineJobFactory> logger = null)
    {
        _serviceProvider = serviceProvider;
        _logger = logger ?? NullLogger<LineJobFactory>.Instance;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        try
        {
            var jobDetail = bundle.JobDetail;
            var jobType = jobDetail.JobType;
            if (_serviceProvider.GetRequiredService(jobType) is not IJob jobToExecute)
            {
                _logger.LogError("Problem instantiating class '{JobClassName}'", bundle.JobDetail.JobType.FullName);
                throw new SchedulerException($"Problem instantiating class '{bundle.JobDetail.JobType.FullName}'");
            }
            return jobToExecute;
        }
        catch (Exception ex)
        {
            _logger.LogError("Problem instantiating class '{JobClassName}'", bundle.JobDetail.JobType.FullName);
            throw new SchedulerException($"Problem instantiating class '{bundle.JobDetail.JobType.FullName}'", ex);
        }
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}