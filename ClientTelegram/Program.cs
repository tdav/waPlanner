using System;
using TeleSharp.TL.Messages;
using TeleSharp.TL.Channels;
using TeleSharp.TL;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace ClientTelegram
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.
            //builder.Services.AddRazorPages();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //}
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.MapRazorPages();

            //app.Run();

           await Client();

        }

        public async static Task<TLUpdates> Client()
        {
            int apiId = 12546946;
            string apiHash = "47bd4f1d270a84059e0c0a75cc648ba1";

            var client = new TelegramClient(apiId, apiHash);
            await client.ConnectAsync();

            //var passwordSettings = await client.GetPasswordSetting();

            //var user = await client.MakeAuthWithPasswordAsync(passwordSettings, "s8064025");

            var req = new TLRequestCreateChannel();
            req.Megagroup = true;
            req.Title = "AAA11";
            req.About = "AAAd11";

            var dialogs = (TLDialogsSlice)await client.GetUserDialogsAsync();
            //var chats = dialogs.Chats
            //    .Where(x => x.GetType() == typeof(TLBotInfo))

            var res = await client.SendRequestAsync<TLUpdates>(req);
            var group = res.Chats[0] as TLChannel;
            await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = group.Id, AccessHash = group.AccessHash.Value }, "MESSAGE");
            //var add_bot = new TLRequestInviteToChannel();
            //add_bot.Users = 

            return res;

        }
    }
}




