using CommunityToolkit.Maui.Alerts;
using System.Collections.Concurrent;

namespace BrilliantSee.Services
{
    public class MessageService
    {
        //定义一个队列，用于存储消息
        //private ConcurrentQueue<string> _messages = new();

        //public MessageService()
        //{
        //    // 启动一个后台任务来监听队列
        //    Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            if (_messages.Count > 0)
        //            {
        //                await ReadMessage();
        //            }
        //            else
        //            {
        //                await Task.Delay(100);
        //            }
        //        }
        //    });
        //}

        //实现一个写入消息的方法
        public void WriteMessage(string message)
        {
            //_messages.Enqueue(message);
            _ = MainThread.InvokeOnMainThreadAsync(() =>
            {
                _ = Toast.Make(message).Show();
            });
        }

        //public async Task ReadMessage()
        //{
        //    var message = string.Empty;
        //    var success = _messages.TryDequeue(out message);
        //    if (success)
        //    {
        //        _ = MainThread.InvokeOnMainThreadAsync(() =>
        //        {
        //            _ = Toast.Make(message!).Show();
        //        });
        //        await Task.Delay(2000);
        //    }
        //}
    }
}