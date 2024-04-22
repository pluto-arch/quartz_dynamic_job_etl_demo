using System.Data;

namespace QuartzJobDemo.DataSources;

public abstract class AbstractDataSource<T> :IDataSource<T>
    //: IDataSource
{
    protected readonly FetcherContextAccesor _accesor;

    public AbstractDataSource(FetcherContextAccesor accesor)
    {
        _accesor = accesor;
    }

    public abstract Task<T> FetchDataAsync();

}