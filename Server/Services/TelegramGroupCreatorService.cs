using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WTelegram;
using TL;
using waPlanner.ModelViews.TelegramViews;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace waPlanner.Services
{
    public interface ITelegramGroupCreatorService
    {
        Task ActivateClient();
        Task SetSMSCode(string code);
        Task<long> CreateGroup(string orgName, string phoneNum);
    }


    public class TelegramGroupCreatorService : ITelegramGroupCreatorService, IDisposable
    {
        private readonly IConfiguration config;
        private readonly Client client;

        public TelegramGroupCreatorService(IConfiguration config)
        {
            this.config = config;
            this.client = new Client(Config);
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }

        private string Config(string what)
        {
            switch (what)
            {
                case "api_id": return config["api_id"];
                case "api_hash": return config["api_hash"];
                case "phone_number": return config["phone_number"];
                case "verification_code":
                case "password": return config["password"];                // if user has enabled 2FA
                default: return config[what];                  // let WTelegramClient decide the default config
            }
        }

        public async Task ActivateClient()
        {            
            await client.LoginUserIfNeeded();
        }

        public Task SetSMSCode(string code)
        {
            client.
        }

        public async Task<long> CreateGroup(string orgName, string phoneNum)
        {
            await client.LoginUserIfNeeded();

            Contacts_ResolvedPeer my_bot = await client.Contacts_ResolveUsername("clinic_test_uzbot");
            var new_user = await client.Contacts_ImportContacts(new[]
            {
                new InputPhoneContact
                {
                    phone = phoneNum,
                    first_name = orgName,
                    last_name = orgName
                }
            });

            var create_group = await client.Channels_CreateChannel(orgName, orgName, megagroup: true);
            var group = create_group.Chats.GetEnumerator();
            group.MoveNext();
            long group_id = group.Current.Key;

            await client.AddChatUser(create_group.Chats[group_id], my_bot.User);
            var get_chat = await client.GetFullChat(create_group.Chats[group_id]);
            var invite = (ChatInviteExported)get_chat.full_chat.ExportedInvite;

            await client.SendMessageAsync(new_user.users[new_user.imported[0].user_id],
                "Ссылка для вашей группы организации " + invite.link);

            return long.Parse($"-100{group_id}");
        }


    }
}
