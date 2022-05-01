using System;
using WTelegram;
using TL;


namespace ClientTelegram
{
    public class Program
    {
        async static Task Main(string[] args)
        {
            await TClient();
        }

        public async static Task TClient()
        {

            using var client = new Client();
            await client.LoginUserIfNeeded();

            Contacts_ResolvedPeer my_bot = await client.Contacts_ResolveUsername("clinic_test_uzbot");
            var new_user = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = "+998900090250", 
                first_name = "Organization", last_name = "User_Name"} });

            var create_group = await client.Channels_CreateChannel("TEST", "TEST", megagroup: true);
            var group = create_group.Chats.GetEnumerator();
            group.MoveNext();
            long group_id = group.Current.Key;

            await client.AddChatUser(create_group.Chats[group_id], my_bot.User);
            var get_chat = await client.GetFullChat(create_group.Chats[group_id]);
            var invite = (ChatInviteExported)get_chat.full_chat.ExportedInvite;
            await client.SendMessageAsync(new_user.users[new_user.imported[0].user_id], 
                "—сылка дл€ вашей группы организации " + invite.link);
        }

        private static string Config(string what)
        {
            
        } 
    }
}




