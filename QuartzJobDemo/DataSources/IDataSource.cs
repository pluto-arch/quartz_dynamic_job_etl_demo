using System.Data;

namespace QuartzJobDemo.DataSources;

public interface IDataSource<T>
{
    Task<T> FetchDataAsync();
    Task TransformData(DataTable data);
}