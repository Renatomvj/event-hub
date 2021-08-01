using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Poc_event_hub_pub
{
    class Program
    {
        // connection string to the Event Hubs namespace
        private const string connectionString = "";
        
        // name of the event hub
        private const string eventHubName = "";

        // number of events to be sent to the event hub
        private const int numOfEvents = 3;

        static EventHubProducerClient producerClient;

        static async Task Main(string[] args)
        {
            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(connectionString, eventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            for (int i = 1; i <= 100; i++)
            {

                
                var e = new Event($"new customer {i} ", "create", new { end = "end1", tel = "tel1" });

                var json = JsonConvert.SerializeObject(e);

                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }

            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 3 events has been published.");
            }
            finally
            {
                await producerClient.DisposeAsync();
            }
        }
    }

    class Event
    {
        public Guid ID { get;  private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public object Content  { get; private set; }

        public Event(string name, string type, object content)
        {
            this.ID = Guid.NewGuid();
            this.Name = name;
            this.Type = type;
            this.Content = content;
        }
    }

}
