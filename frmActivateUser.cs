using System;
using TLSharp.Core;
using Apteka.UtilsUI;
using Apteka.Utils;

namespace Apteka.RepExportSprav
{
    public partial class frmActivateUser : DevExpress.XtraEditors.XtraForm
    {
        private string seshash;

        //apiId и apiHash из получите  https://my.telegram.org/apps 
        private int apiId = 812998;
        private string apiHash = "2ff5988a31cc2fd22ec3547d51fced09";
        private TelegramClient client;
        private string TelNum = "";

        public frmActivateUser()
        {
            InitializeComponent();
            CLang.Init(this);
        }

        private async void frmActivateUser_Load(object sender, EventArgs e)
        {
            client = new TelegramClient(apiId, apiHash);
            await client.ConnectAsync();
        }

        private async void btnGetSms_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                MessageBoxDev.ShowInfo("Нет соединения сервером");
                return;
            }

            TelNum = "+998" + edTelNum.Text.Replace("-", "").Replace(" ", "");
            seshash = await client.SendCodeRequestAsync(TelNum);
        }

        private async void btnSendVerificationCode_Click(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                MessageBoxDev.ShowInfo("Нет соединения сервером");
                return;
            }

            var user = await client.MakeAuthAsync(TelNum, seshash, edSMScode.Text);
            if (user != null)
            {
                MessageBoxDev.ShowInfo("Пользователь успешно активирован");
                Close();
            }
        }

    }
}