using Serilog;
using Serilog.Formatting.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new JsonFormatter()) // 核心：以JSON格式输出到控制台
    .CreateBootstrapLogger(); // 创建启动引导记录器

try
{
    Log.Information("Starting web application...");

    var builder = WebApplication.CreateBuilder(args);

    // ========== 1. 配置 Serilog 作为主要的日志框架 ==========
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) // 从appsettings.json读取配置
        .ReadFrom.Services(services) // 从依赖注入容器读取服务
        .Enrich.FromLogContext() // 允许动态丰富日志上下文（如添加请求ID）
        .WriteTo.Console(new JsonFormatter()) // 确保最终输出为JSON
    );

    // ========== 2. 原有的服务配置 (保持不变) ==========
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // ========== 3. 配置HTTP请求管道 (可选择性添加请求日志中间件) ==========
    // app.UseSerilogRequestLogging(); // (可选) 为每个HTTP请求自动生成摘要日志

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // 捕获并记录启动过程中发生的致命异常
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // 确保在应用程序关闭时，所有缓冲的日志都被刷新
    Log.CloseAndFlush();
}