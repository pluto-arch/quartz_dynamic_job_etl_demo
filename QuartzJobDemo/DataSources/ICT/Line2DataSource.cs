using System.Data;
using Microsoft.Extensions.Logging;

namespace QuartzJobDemo.DataSources;

public class Line2DataSource: AbstractDataSource
{
    private readonly ILogger<Line1DataSource> _logger;

    public Line2DataSource(FetcherContextAccesor accesor,ILogger<Line1DataSource> logger):base(accesor)
    {
        _logger = logger;
    }


    /// <inheritdoc />
    public override Task<DataTable> FetchDataAsync()
    {
        var ctx= _accesor.CurrentFetcherContext;
        _logger.LogInformation("Line2DataSource 处理 {LineId} 的数据",ctx.LineId);


        var dt=new DataTable();
        dt.Columns.Add("ID", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Age", typeof(int));
        dt.Columns.Add("Time", typeof(DateTime));

        dt.Rows.Add(1, "Alice", 25,DateTime.Now);
        dt.Rows.Add(2, "Bob", 30,DateTime.Now);
        dt.Rows.Add(3, "Charlie", 22,DateTime.Now);

        return Task.FromResult(dt);

    }

    /// <inheritdoc />
    public override Task TransformData(DataTable data)
    {
        // 遍历DataTable并打印数据
        foreach (DataRow row in data.Rows)
        {
            _logger.LogInformation($"Age：{row["Age"]}");
        }
        return Task.CompletedTask;
    }
}