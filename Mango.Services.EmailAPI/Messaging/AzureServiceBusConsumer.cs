using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Service;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailRegisterUserQueue;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _emailCartProcesser;
        private readonly ServiceBusProcessor _emailRegisterUserProcesser;
        private readonly ServiceBusProcessor _emailOrderPlacedProcesser;
        private readonly EmailService _emailService;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            emailRegisterUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisterUserQueue");
            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _emailCartProcesser = client.CreateProcessor(emailCartQueue);
            _emailRegisterUserProcesser = client.CreateProcessor(emailRegisterUserQueue);
            _emailOrderPlacedProcesser = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);
            _emailService = emailService;
        }

        public async Task Start()
        {
            _emailCartProcesser.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcesser.ProcessErrorAsync += ErrorHanler;
            await _emailCartProcesser.StartProcessingAsync();

            _emailRegisterUserProcesser.ProcessMessageAsync += OnEmailRegisterRequestReceived;
            _emailRegisterUserProcesser.ProcessErrorAsync += ErrorHanler;
            await _emailRegisterUserProcesser.StartProcessingAsync();

            _emailOrderPlacedProcesser.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcesser.ProcessErrorAsync += ErrorHanler;
            await _emailOrderPlacedProcesser.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcesser.StopProcessingAsync();
            await _emailCartProcesser.DisposeAsync();

            await _emailRegisterUserProcesser.StopProcessingAsync();
            await _emailRegisterUserProcesser.DisposeAsync();

            await _emailOrderPlacedProcesser.StopProcessingAsync();
            await _emailOrderPlacedProcesser.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                // TODO: log email here
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnEmailRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string objMessage = JsonConvert.DeserializeObject<string>(body);
            try
            {
                // TODO: log email here
                await _emailService.EmailRegisterAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where you will receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
            try
            {
                // TODO: log email here
                await _emailService.LogOrderPlaced(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Task ErrorHanler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
