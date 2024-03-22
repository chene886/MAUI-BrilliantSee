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
using Whisper.net;
using System.Threading.Channels;
using Plugin.Maui.Audio;
using System.Reflection;
using Whisper.net.Wave;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace BrilliantComic.Services
{
    public class AIService
    {
        public Kernel kernel { get; set; } = new Kernel();
        public WhisperProcessor? whisper { get; set; }
        private readonly Channel<string> channel = Channel.CreateUnbounded<string>();
        private readonly IAudioManager _audioManager;
        private readonly IAudioRecorder _audioRecorder;
        public bool hasModel { get; set; } = false;

        public AIService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _audioRecorder = _audioManager.CreateRecorder();
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
            hasModel = true;
            InitWhisper();
        }

        public async void InitWhisper()
        {
            if (whisper != null)
                return;
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BrilliantComic.Services.WhisperModels.ggml-base.bin");
            byte[] buffer = new byte[stream!.Length];
            await stream.ReadAsync(buffer, 0, (int)stream.Length);
            whisper = WhisperFactory
                .FromBuffer(buffer)
                .CreateBuilder()
                .WithLanguage("auto")
                .Build();
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

        public async Task BeingMessageAsync()
        {
            var path = FileSystem.AppDataDirectory;
            if (!File.Exists(Path.Combine(path, "recorde.wav")))
                using (File.Create(Path.Combine(path, "recorde.wav"))) { }
            if (!_audioRecorder.IsRecording)
                await _audioRecorder.StartAsync(Path.Combine(path, "recorde.wav"));
        }

        public async Task<string> StopMessageAsync()
        {
            if (_audioRecorder.IsRecording)
            {
                var record = await _audioRecorder.StopAsync();
                using var Stream = record.GetAudioStream();
                using var wavStream = new MemoryStream();
                await using var reader = new WaveFileReader(Stream);
                var resampler = new WdlResamplingSampleProvider(reader.ToSampleProvider(), 16000);
                WaveFileWriter.WriteWavFileToStream(wavStream, resampler.ToWaveProvider16());
                wavStream.Seek(0, SeekOrigin.Begin);
                var text = string.Empty;
                await foreach (var result in whisper!.ProcessAsync(wavStream))
                {
                    text += result.Text;
                }
                return text;
            }
            return "";
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