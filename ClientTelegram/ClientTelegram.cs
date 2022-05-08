using WTelegram;
using TL;


namespace ClientTelegram
{
    public class ClientTelegram
    {
        public static void Main(string[] args)
        {

        }

        public static async Task<long> CreateClientGroup(string orgName, string phoneNum)
        {
            using var client = new Client(Config);
            await client.LoginUserIfNeeded();

            var cha = client.Messages_GetAllChats();
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
                "—сылка дл€ вашей группы организации " + invite.link);

            return long.Parse($"-100{group_id}");
        }

        private static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return "14819555";
                case "api_hash": return "447c1b18db01d8baea52ab0d3dc77ec3";
                case "phone_number": return "+998339900979";
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "password": return "";     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }
    }
}




