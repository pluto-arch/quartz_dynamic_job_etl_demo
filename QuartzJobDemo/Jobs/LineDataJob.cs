using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzJobDemo.DataSources;
using QuartzJobDemo.Dtos;

namespace QuartzJobDemo.Jobs;



[DisallowConcurrentExecution]
public class LineDataJob:IJob
{
    private readonly ILogger<LineDataJob> _logger;
    private readonly FetcherContextAccesor _accesor;

    public LineDataJob(ILogger<LineDataJob> logger,FetcherContextAccesor accesor)
    {
        _logger = logger;
        _accesor = accesor;
    }


    /// <inheritdoc />
    public async Task Execute(IJobExecutionContext context)
    {
        var lineId=context.JobDetail.JobDataMap.Get("LineId") as string;
        var dataSourceName= context.JobDetail.JobDataMap.Get("DataSourceName") as string;
        _logger.LogInformation("{lineId} has been executed",lineId);
        using (_accesor.Begin(lineId,dataSourceName,null))
        {
            var dataSource= _accesor.CurrentFetcherContext.ResolveDataSource<List<DemoDto>>();
             var data = await dataSource.FetchDataAsync();
        }
    }
}