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
            int apiId = 0;
            string apiHash = "hash";

            using var client = new Client();
            var me = await client.LoginUserIfNeeded();
            Contacts_ResolvedPeer my_bot = await client.Contacts_ResolveUsername("clinic_test_uzbot");
            var new_user = await client.Contacts_ImportContacts(new[] { new InputPhoneContact { phone = "+998900090250" } });

            var create_group = await client.Channels_CreateChannel("TEST", "TEST", megagroup: true);
            var group = create_group.Chats.GetEnumerator();
            group.MoveNext();
            long group_id = group.Current.Key;

            await client.AddChatUser(create_group.Chats[group_id], my_bot.User);
            var mcf = await client.GetFullChat(create_group.Chats[group_id]);
            var invite = (ChatInviteExported)mcf.full_chat.ExportedInvite;
            await client.SendMessageAsync(new_user.users[new_user.imported[0].user_id], "Join to this group " + invite.link);
        }
    }
}




