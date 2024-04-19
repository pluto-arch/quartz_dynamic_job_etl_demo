// See https://aka.ms/new-console-template for more information
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzJobDemo;
using QuartzJobDemo.DataSources;
using QuartzJobDemo.Jobs;


var config=new ConfigurationBuilder()
    .AddJsonFile("jobsettings.json",false,reloadOnChange:true)
    .Build();


var hostBuilder=Host.CreateDefaultBuilder()
    .ConfigureServices((_,services)=>
    {
        services.AddSingleton<IConfiguration>(config);
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<IJobFactory,LineJobFactory>();
        services.AddSingleton<JobRunner>();
        services.AddTransient<LineDataJob>();
        services.AddSingleton<FetcherContextAccesor>();


        var assembly=Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataSource<>)))
            .ToList();

        foreach (var item in types)
        {
            var entitTypies = item.GetInterfaces()
                .FirstOrDefault(x=>x.IsGenericType&&x.Name.StartsWith("IDataSource"))?.GetGenericArguments()[0];
            if (entitTypies != null)
            {
                var interfaceType = typeof(IDataSource<>).MakeGenericType(entitTypies);
                services.AddTransient(interfaceType,item);
            }
        }


    })
    .ConfigureLogging(options=>options.AddSimpleConsole(c =>
    {
        c.TimestampFormat = "[HH:mm:ss] ";
        c.SingleLine=true;
    }))
    .UseConsoleLifetime();


var app=hostBuilder.Build();
var runner = app.Services.GetRequiredService<JobRunner>();
await runner.StartAsync();
await app.RunAsync();
