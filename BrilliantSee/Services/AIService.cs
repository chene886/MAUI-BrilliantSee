using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BrilliantSee.Services
{
    public class AIService
    {
        public Kernel kernel { get; set; } = new Kernel();

        /// <summary>
        /// 是否已经初始化模型
        /// </summary>
        public bool hasModel { get; set; } = false;

        /// <summary>
        /// 初始化模型
        /// </summary>
        /// <param name="model">模型名</param>
        /// <param name="key">模型key</param>
        /// <param name="url">模型代理地址</param>
        public void InitKernel(string model, string key, string url)
        {
            var handler = new OpenAIHttpClentHandler();
            handler.url = url;
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: key,
                httpClient: new HttpClient(handler));
            kernel = builder.Build();
            hasModel = true;
        }

        /// <summary>
        /// 导入插件
        /// </summary>
        /// <param name="plugin"></param>
        public void ImportPlugins(Object plugin)
        {
            kernel.ImportPluginFromObject(plugin);
        }

        /// <summary>
        /// 移除插件
        /// </summary>
        public void RemovePlugins()
        {
            if (kernel.Plugins.Any())
                kernel.Plugins.Remove(kernel.Plugins.First());
        }

        /// <summary>
        /// 对输入的问题进行求解做出动作和回答
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<string> SolvePromptAsync(string msg)
        {
            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            };
            try
            {
                var result = await kernel.InvokePromptAsync(msg, new(settings));
                return result.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public class OpenAIHttpClentHandler : HttpClientHandler
    {
        public string url = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri!.LocalPath == "/v1/chat/completions")
            {
                UriBuilder builder = new UriBuilder(url);
                request.RequestUri = builder.Uri;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}