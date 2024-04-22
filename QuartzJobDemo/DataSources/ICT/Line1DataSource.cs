using System.Data;
using Microsoft.Extensions.Logging;
using Quartz.Logging;
using QuartzJobDemo.Dtos;

namespace QuartzJobDemo.DataSources;

public class Line1DataSource : IDataSource<List<DemoDto>>
{
    private readonly FetcherContextAccesor _accesor;
    private readonly ILogger<Line1DataSource> _logger;

    public Line1DataSource(FetcherContextAccesor accesor, ILogger<Line1DataSource> logger)
    {
        _accesor = accesor;
        _logger = logger;
    }


    /// <inheritdoc />
    public Task<List<DemoDto>> FetchDataAsync()
    {
        var data = new List<DemoDto>();
        foreach (var row in Enumerable.Range(1, 100000))
        {
            data.Add(new DemoDto
            {
                ID = row,
                Name = "Alice",
                Age = 22
            });
        }

        foreach (var row in Enumerable.Range(100000, 300000))
        {
            data.Add(new DemoDto
            {
                ID = row + 2,
                Name = "Alice",
                Age = 22
            });
        }
        _logger.LogInformation("数量：{Count}",data.Count);
        return Task.FromResult(data);
    }
}