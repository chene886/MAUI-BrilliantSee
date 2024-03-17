using Azure.AI.OpenAI;
using BrilliantComic.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using BrilliantComic.Models.Comics;
using BrilliantComic.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;

namespace BrilliantComic.Services
{
    public class AIService
    {
        private readonly DBService _db;
        private readonly SourceService _sourceService;
        public Kernel kernel { get; set; } = new Kernel();

        public AIService(DBService db, SourceService sourceService)
        {
            _db = db;
            _sourceService = sourceService;
        }

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
            kernel.ImportPluginFromObject(new Plugins.ComicPlugin(_db, _sourceService));
        }

        public async Task<string> SolvePromptAsync(string msg)
        {
            OpenAIPromptExecutionSettings settings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            };
            var result = await kernel.InvokePromptAsync(msg, new(settings));
            return result.ToString();
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