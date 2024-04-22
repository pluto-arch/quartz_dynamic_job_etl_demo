using System.Data;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using QuartzJobDemo.Dtos;

namespace QuartzJobDemo.DataSources;

public class Line2DataSource: IDataSource<List<DemoDto>> 
{
    private readonly FetcherContextAccesor _accesor;
    private readonly ILogger<Line2DataSource> _logger;

    public Line2DataSource(FetcherContextAccesor accesor,ILogger<Line2DataSource> logger)
    {
        _accesor = accesor;
        _logger = logger;
    }


    /// <inheritdoc />
    public Task<List<DemoDto>> FetchDataAsync()
    {
        var data= new List<DemoDto>();
        foreach (var row in Enumerable.Range(1, 100000))
        {
            data.Add(new DemoDto
            {
                ID=row,
                Name="Alice",
                Age=22
            });
        }

        foreach (var row in Enumerable.Range(100000, 300000))
        {
            data.Add(new DemoDto
            {
                ID=row+2,
                Name="Alice",
                Age=22
            });
        }
        _logger.LogInformation("Line2DataSource has been executed. 数量：{Count}",data.Count);
        return Task.FromResult(data);
    }
}