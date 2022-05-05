using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TdLib;
using TDLib.Bindings;


namespace waPlanner.Services
{
    public interface ITelegramGroupCreatorService
    {
        Task GetAuthenticationCode();
        Task SetAuthenticationCode(string code, string password);
        Task<long> CreateGroup(string PhoneNumber, string OrgName);
    }

    public class TelegramGroupCreatorService : ITelegramGroupCreatorService
    {
        private readonly IConfiguration conf;
        private readonly ILogger<TelegramGroupCreatorService> logger;

        private readonly int ApiId;
        private readonly string ApiHash;
        private readonly string PhoneNumber;

        private readonly TdClient client;
        private static readonly ManualResetEventSlim ReadyToAuthenticate = new();

        private bool _authNeeded;
        private bool _passwordNeeded;

        public Guid Id = Guid.NewGuid();


        public TelegramGroupCreatorService(IConfiguration conf, ILogger<TelegramGroupCreatorService> logger)
        {
            this.conf = conf;
            this.logger = logger;

            ApiId = Convert.ToInt32(conf["api_id"]);
            ApiHash = conf["api_hash"];
            PhoneNumber = conf["phone_number"];

            client = new TdClient();
            client.Bindings.SetLogVerbosityLevel(TdLogLevel.Fatal);

            client.UpdateReceived += async (_, update) => { await ProcessUpdates(update); };
        }


        public async Task GetAuthenticationCode()
        {
            ReadyToAuthenticate.Wait();

            await client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber { PhoneNumber = PhoneNumber, });
        }

        public async Task SetAuthenticationCode(string code, string password)
        {
            ReadyToAuthenticate.Wait();

            await client.ExecuteAsync(new TdApi.CheckAuthenticationCode { Code = code });

            if (!_passwordNeeded) { return; }

            await client.ExecuteAsync(new TdApi.CheckAuthenticationPassword { Password = password });
        }


        public async Task<long> CreateGroup(string PhoneNumber, string OrgName)
        {
            ReadyToAuthenticate.Wait();

            var new_group = await client.ExecuteAsync(new TdApi.CreateNewSupergroupChat { Title = OrgName, Description = "Planner", });
            var group_id = new_group.Id;

            var users = await client.GetChatsAsync(limit: 341);
            var bot = await client.GetUserAsync(users.ChatIds[2]);

            TdApi.Contact new_contact = new TdApi.Contact { FirstName = OrgName, LastName = "Planner", PhoneNumber = PhoneNumber};
            
            //await client.AddContactAsync(new_contact);

            //await client.AddChatMembersAsync(group_id, new long[] {bot.Id, new_contact.UserId});
            
            return group_id;
        }

        private async Task ProcessUpdates(TdApi.Update update)
        {
            // Since Tdlib was made to be used in GUI application we need to struggle a bit and catch required events to determine our state.
            // Below you can find example of simple authentication handling.
            // Please note that AuthorizationStateWaitOtherDeviceConfirmation is not implemented.

            switch (update)
            {
                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters }:
                    // TdLib creates database in the current directory.
                    // so create separate directory and switch to that dir.
                    var filesLocation = Path.Combine(AppContext.BaseDirectory, "db");
                    await client.ExecuteAsync(new TdApi.SetTdlibParameters
                    {
                        Parameters = new TdApi.TdlibParameters
                        {
                            ApiId = ApiId,
                            ApiHash = ApiHash,
                            DeviceModel = "PC",
                            SystemLanguageCode = "ru",
                            ApplicationVersion = "1.0.1",
                            DatabaseDirectory = filesLocation,
                            FilesDirectory = filesLocation,
                            // More parameters available!
                        }
                    });
                    break;

                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitEncryptionKey }:
                    await client.ExecuteAsync(new TdApi.CheckDatabaseEncryptionKey());
                    break;

                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber }:
                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitCode }:
                    _authNeeded = true;
                    ReadyToAuthenticate.Set();
                    break;

                case TdApi.Update.UpdateAuthorizationState { AuthorizationState: TdApi.AuthorizationState.AuthorizationStateWaitPassword }:
                    _authNeeded = true;
                    _passwordNeeded = true;
                    ReadyToAuthenticate.Set();
                    break;

                case TdApi.Update.UpdateUser:
                    ReadyToAuthenticate.Set();
                    break;

                case TdApi.Update.UpdateConnectionState { State: TdApi.ConnectionState.ConnectionStateReady }:
                    // You may trigger additional event on connection state change
                    break;

                default:
                    // ReSharper disable once EmptyStatement
                    ;
                    // Add a breakpoint here to see other events
                    break;
            }
        }

    }
}
