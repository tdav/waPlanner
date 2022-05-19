using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using waPlanner.ModelViews;

namespace waPlanner.BackgroundQueue
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly ISendDocumentsQueue<SendDocumentsModel> queue;

        private readonly IServiceScopeFactory scopeFactory;

        public MyBackgroundWorker(IServiceScopeFactory _scopeFactory, ISendDocumentsQueue<SendDocumentsModel> queue)
        {
            this.queue = queue;
            scopeFactory = _scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    //int cd = 0; int cm = 0; int cu = 0; int ch = 0;

            //    var ctx = Program.ctx;
            //    ctx.BeginTrans();

            //    var cold = ctx.GetCollection<tbDocument>();
            //    cold.InsertBulk(queueDoc.GetAll());

            //    var colm = ctx.GetCollection<tbMessage>();
            //    colm.InsertBulk(queueMes.GetAll());

            //    var colu = ctx.GetCollection<tbUser>();
            //    colu.InsertBulk(queueUser.GetAll());

            //    var colc = ctx.GetCollection<tbChat>();
            //    colc.InsertBulk(queueChat.GetAll());

            //    ctx.Commit();
            //}


            

            Console.WriteLine(DateTime.Now);

            await Task.Delay(1000, stoppingToken);

        }
    }
}
