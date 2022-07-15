using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TdLib
    ;
using TDLib.Bindings;
using waPlanner.Database;
using waPlanner.Extensions;
using waPlanner.Interfaces;
using waPlanner.ModelViews;
using waPlanner.TelegramBot;

namespace waPlanner.Services
{
    public interface ITelegramGroupCreatorService
    {
        Task<AnswerBasic> GetAuthenticationCode();
        Task<AnswerBasic> SetAuthenticationCode(string code, string password);
        Task<Answer<long[]>> CreateGroup(string PhoneNumber, string OrgName);
        ValueTask<Answer<IdValue>> SendRandomPassword(string PhoneNum);
    }

    public class TelegramGroupCreatorService : ITelegramGroupCreatorService, IAutoRegistrationSingletonLifetimeService
    {
        private readonly IConfiguration conf;
        private readonly ILogger<TelegramGroupCreatorService> logger;
        private readonly IServiceProvider provider;

        private readonly int ApiId;
        private readonly string ApiHash;
        private readonly string PhoneNumber;

        private readonly TdClient client;
        private static readonly ManualResetEventSlim ReadyToAuthenticate = new();

        private bool _authNeeded;
        private bool _passwordNeeded;

        public TelegramGroupCreatorService(IConfiguration conf, ILogger<TelegramGroupCreatorService> logger, IServiceProvider provider)
        {
            this.conf = conf;
            this.logger = logger;
            this.provider = provider;

            ApiId = Convert.ToInt32(conf["api_id"]);
            ApiHash = conf["api_hash"];
            PhoneNumber = conf["phone_number"];

            client = new TdClient();
            client.Bindings.SetLogVerbosityLevel(TdLogLevel.Fatal);

            client.UpdateReceived += async (_, update) => { await ProcessUpdates(update); };
        }

        private IHttpContextAccessorExtensions GetAccessor()
        {
            using (var scope = provider.CreateScope())
            {
                var accessor = scope.ServiceProvider.GetService<IHttpContextAccessorExtensions>();
                return accessor;
            }
        }

        public async Task<AnswerBasic> GetAuthenticationCode()
        {
            try
            {
                int role_id = GetAccessor().GetRoleId();
                if (role_id == (int)UserRoles.SUPER_ADMIN)
                {
                    ReadyToAuthenticate.Wait();
                    await client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber { PhoneNumber = PhoneNumber });
                    return new AnswerBasic(true, "");
                }
                return new AnswerBasic(false, "Только супер-Админ имеет эту привелегию");

            }
            catch (Exception e)
            {
                logger.LogError($"TelegramGroupCreatorService.GetAuthenticationCode Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }

        public async Task<AnswerBasic> SetAuthenticationCode(string code, string password)
        {
            try
            {
                int role_id = GetAccessor().GetRoleId();
                if (role_id == (int)UserRoles.SUPER_ADMIN)
                {
                    ReadyToAuthenticate.Wait();

                    await client.ExecuteAsync(new TdApi.CheckAuthenticationCode { Code = code });

                    if (!_passwordNeeded) { return new AnswerBasic(false, "Нужен двухфакторный пароль"); }

                    await client.ExecuteAsync(new TdApi.CheckAuthenticationPassword { Password = password });
                    return new AnswerBasic(true, "");
                }
                return new AnswerBasic(false, "Только супер-Админ имеет эту привелегию");
            }
            catch (Exception e)
            {
                logger.LogError($"TelegramGroupCreatorService.SetAuthenticationCode Error:{e.Message}");
                return new AnswerBasic(false, "Ошибка программы");
            }

        }


        public async Task<Answer<long[]>> CreateGroup(string PhoneNumber, string OrgName)
        {
            try
            {
                ReadyToAuthenticate.Wait();

                var new_group = await client.ExecuteAsync(new TdApi.CreateNewSupergroupChat { Title = OrgName, Description = "Planner" });
                var group_id = new_group.Id;
                var group = new_group.Type as TdApi.ChatType.ChatTypeSupergroup;
                var getGroupInfo = await client.GetSupergroupFullInfoAsync(group.SupergroupId);
                var bot = await client.SearchPublicChatAsync("tplanner_bot");
                await client.AddChatMemberAsync(group_id, bot.Id);

                var contact = await client.ImportContactsAsync(new TdApi.Contact[]
                {
                new TdApi.Contact
                {
                    FirstName = OrgName,
                    LastName = "Planner",
                    PhoneNumber = PhoneNumber
                }
                });

                var content = new TdApi.InputMessageContent.InputMessageText
                {
                    Text = new TdApi.FormattedText
                    {
                        Text = $"Ссылка для вашей группы организации {getGroupInfo.InviteLink.InviteLink}"
                    }
                };
                var chat = await client.CreatePrivateChatAsync(contact.UserIds[0]);
                await client.SendMessageAsync(chatId: chat.Id, inputMessageContent: content);

                return new Answer<long[]>(true, "", new long[] { group_id, chat.Id });
            }
            catch (Exception e)
            {
                logger.LogError($"TelegramGroupCreatorService.CreateGroup Error:{e.Message}");
                return new Answer<long[]>(false, $"Ошибка программы", null);
            }

        }

        public async ValueTask<Answer<IdValue>> SendRandomPassword(string PhoneNum)
        {
            try
            {
                using (var scope = provider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                    ReadyToAuthenticate.Wait();
                    var staff = await db.tbStaffs
                        .Include(x => x.Organization)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.PhoneNum == PhoneNum);

                    if (staff is null)
                        return new Answer<IdValue>(false, "", null);

                    string generatePassword = Utils.GeneratePassword.CreatePassword();
                    var text = await client.ParseTextEntitiesAsync($"Ваш новый пароль: 👉 <code>{generatePassword}</code>. 👈 Никому не передавайте!", new TdApi.TextParseMode.TextParseModeHTML());
                    var content = new TdApi.InputMessageContent.InputMessageText { Text = text };

                    var contact = await client.ImportContactsAsync(new TdApi.Contact[]
                    {
                        new TdApi.Contact
                        {
                            FirstName = staff.Organization.Name,
                            LastName = "Planner",
                            PhoneNumber = PhoneNum
                        }
                    });

                    var chat = await client.CreatePrivateChatAsync(contact.UserIds[0]);
                    await client.SendMessageAsync(chatId: chat.Id, inputMessageContent: content);

                    return new Answer<IdValue>(true, "", new IdValue { Id = staff.Id, Value = generatePassword });
                }
            }
            catch (Exception e)
            {
                logger.LogError($"TelegramGroupCreatorService.SendRandomPassword Error: {e.Message}");
                return new Answer<IdValue>(false, $"Ошибка программы", null);
            }
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
