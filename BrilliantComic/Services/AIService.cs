using Azure.AI.OpenAI;
using BrilliantComic.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrilliantComic.Services
{
    public class AIService
    {
        private readonly DBService _db;
        public Kernel kernel { get; set; } = new Kernel();
        private KernelPlugin? kernelPlugins { get; set; }
        private string pluginDirectory { get; set; } = string.Empty;
        public bool hasModel { get; set; } = false;
        private List<SettingItem> modelConfigs { get; set; } = new List<SettingItem>();

        public AIService(DBService db)
        {
            _db = db;
            _ = InitModelAsync();
        }

        public async Task InitModelAsync()
        {
            modelConfigs = await _db.GetSettingItemsAsync("AIModel");
            var modelId = modelConfigs.Where(s => s.Name == "ModelId").First().Value;
            var apiKey = modelConfigs.Where(s => s.Name == "ApiKey").First().Value;
            var apiUrl = modelConfigs.Where(s => s.Name == "ApiUrl").First().Value;
            if (modelId != "" && apiKey != "" && apiUrl != "")
            {
                UpdateModel(modelId, apiKey, apiUrl);
                hasModel = true;
            }
        }

        public void UpdateModel(string model, string key, string url)
        {
            InitKernel(model, key, url);
            foreach (var item in modelConfigs)
            {
                switch (item.Name)
                {
                    case "ModelId":
                        item.Value = model;
                        break;

                    case "ApiKey":
                        item.Value = key;
                        break;

                    case "ApiUrl":
                        item.Value = url;
                        break;
                }
                _ = _db.UpdateSettingItemAsync(item);
            }
        }

        private void InitKernel(string model, string key, string url)
        {
            var handler = new OpenAIHttpClentHandler();
            handler.url = url;
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: key,
                httpClient: new HttpClient(handler));
            kernel = builder.Build();

            pluginDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            kernelPlugins = kernel.ImportPluginFromPromptDirectory(pluginDirectory);
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