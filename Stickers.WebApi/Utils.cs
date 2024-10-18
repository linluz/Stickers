using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Stickers.WebApi;

public class Utils
{
    /// <summary>
    /// 获取 NewtonsoftJsonPatchInputFormatter
    /// </summary>
    /// <returns></returns>
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        => new ServiceCollection() // 创建一个新的 ServiceCollection 实例  
            .AddLogging() // 将日志服务添加到服务集合中  
            .AddMvc() // 向服务集合中添加 MVC 服务  
            .AddNewtonsoftJson() // 注册支持 Newtonsoft.Json 的 JSON 序列化和反序列化功能  
            .Services.BuildServiceProvider() // 从已配置的服务集合中构建服务提供者  
            .GetRequiredService<IOptions<MvcOptions>>() // 从服务提供者中获取 IOptions<MvcOptions> 服务  
            .Value // 获取实际的 MvcOptions 配置对象  
            .InputFormatters // 访问 MvcOptions 实例中的 InputFormatters 属性，这是一个输入格式化程序的集合  
            .OfType<NewtonsoftJsonPatchInputFormatter>() // 从 InputFormatters 中筛选出所有类型为 NewtonsoftJsonPatchInputFormatter 的格式化程序  
            .First(); 

}

