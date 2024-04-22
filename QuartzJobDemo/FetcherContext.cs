using System.Data;
using Microsoft.Extensions.DependencyInjection;
using QuartzJobDemo.DataSources;

namespace QuartzJobDemo;

public class FetcherContext:IDisposable
{

    public FetcherContext()
    {
        Paramters=new ();
    }

    public FetcherContext(string lineId,string sourceName):this()
    {
        SourceName=sourceName; 
        LineId=lineId;
    }

    public FetcherContext(string lineId,string sourceName, Dictionary<string, dynamic> paramters, IServiceScope scoped):this(lineId,sourceName)
    {
        Paramters=paramters;
        _scoped= scoped;
    }

    /// <summary>
    /// 产线Id
    /// </summary>
    public string LineId { get; set; }

    /// <summary>
    /// 数据源名称
    /// </summary>
    public string SourceName { get; set; }

    private readonly IServiceScope _scoped;
    public IServiceProvider ServiceProvider =>_scoped.ServiceProvider;

    /// <summary>
    /// 参数
    /// </summary>
    public Dictionary<string,dynamic> Paramters {get;set;}

    /// <summary>
    /// 添加参数信息
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddParamter(string key,dynamic value)
    {
        Paramters.Add(key,value);
    }


    public T Resolve<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    public IDataSource<T> ResolveDataSource<T>()
    {
        var dataSource = ServiceProvider.GetServices<IDataSource<T>>().FirstOrDefault(x=>x.GetType().Name==SourceName);
        if (dataSource is null)
        {
            throw new InvalidOperationException($"{LineId} 没有注册名为： [{SourceName}] 的处理器");
        }
        return dataSource;
    }

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _scoped?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}



public class FetcherContextAccesor
{
    private readonly IServiceProvider _service;
    private readonly AsyncLocal<FetcherContext> _currentContext = new AsyncLocal<FetcherContext>();

    public FetcherContext CurrentFetcherContext
    {
        get => _currentContext.Value;
        set => _currentContext.Value = value;
    }


    public FetcherContextAccesor(IServiceProvider service)
    {
        _service = service;
    }


    /// <summary>
    /// 开始一个新的数据抓取上下文
    /// </summary>
    /// <returns></returns>
    public IDisposable Begin(string lineId,string sourceName,Dictionary<string,dynamic> paramters)
    {
        var parentScope = CurrentFetcherContext;
        var context= new FetcherContext(lineId,sourceName,paramters,_service.CreateScope());
        CurrentFetcherContext = context;
        return new DisposeAction(() =>
        {
            CurrentFetcherContext = parentScope;
            context?.Dispose();
            context=null;
        });
    }
}