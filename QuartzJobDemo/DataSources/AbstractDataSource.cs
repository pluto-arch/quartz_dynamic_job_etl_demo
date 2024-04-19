using System.Data;

namespace QuartzJobDemo.DataSources;

public abstract class AbstractDataSource : IDataSource<DataTable>
{
    protected readonly FetcherContextAccesor _accesor;

    public AbstractDataSource(FetcherContextAccesor accesor)
    {
        _accesor = accesor;
    }

    /// <inheritdoc />
    public abstract Task<DataTable> FetchDataAsync();
    

    /// <inheritdoc />
    public abstract Task TransformData(DataTable data);
}