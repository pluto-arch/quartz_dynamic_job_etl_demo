using System.Collections.Concurrent;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzJobDemo.Jobs;

namespace QuartzJobDemo;

public class JobRunner
{
    private readonly IConfiguration _configuration;
    private readonly ISchedulerFactory _jobSchedularFactory;
    private readonly IJobFactory _jobFactory;
    public JobRunner(IConfiguration configuration, ISchedulerFactory jobSchedularFactory, IJobFactory jobFactory)
    {
        _configuration = configuration;
        _jobSchedularFactory = jobSchedularFactory;
        _jobFactory = jobFactory;
    }


    public async Task StartAsync()
    {
        IScheduler scheduler=await _jobSchedularFactory.GetScheduler();
        scheduler.JobFactory=_jobFactory;
        await CreateJob(scheduler);
    }

    private async Task CreateJob(IScheduler scheduler)
    {
        var jobs= _configuration.GetSection("Jobs").Get<List<JobOptions>>()??new ();
        foreach (var item in jobs)
        {
            if (!item.Enabled)
            {
                continue;
            }
            
            var jobData=new JobDataMap();
            jobData.Add("LineId",item.LineId);
            jobData.Add("DataSourceName",item.DataSourceName);
            IJobDetail job=JobBuilder.Create<LineDataJob>()
                .WithIdentity(item.LineId!)
                .SetJobData(jobData)
                .Build();

            ITrigger trigger=TriggerBuilder.Create()
                .WithIdentity(item.LineId)
                .WithCronSchedule(item.Corn)
                .Build();

           
            await scheduler.ScheduleJob(job,trigger);
            await scheduler.Start();
        }
    }
}