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
using Microsoft.SemanticKernel.Services;
using System.Threading.Channels;
using System.Reflection;

namespace BrilliantComic.Services
{
    public class AIService
    {
        public Kernel kernel { get; set; } = new Kernel();
        public bool hasModel { get; set; } = false;

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

        public void ImportPlugins(Object plugin)
        {
            kernel.ImportPluginFromObject(plugin);
        }

        public void RemovePlugins()
        {
            if (kernel.Plugins.Any())
                kernel.Plugins.Remove(kernel.Plugins.First());
        }

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