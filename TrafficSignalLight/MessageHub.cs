using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace TrafficSignalLight
{
    public class MessageHub : Hub
    {
        // NCRepository repos = new NCRepository();

        public static async Task<int> Init()
        {
            // Create a connection to your SignalR hub
            var hubConnection = new HubConnection("http://localhost/TLC/");

            // Create a proxy for your hub
            IHubProxy chatHubProxy = hubConnection.CreateHubProxy("MessageHub"); // Replace "MessageHub" with your hub name

            // Register client-side methods to be called by the server
            chatHubProxy.On<string, string>("ReceiveMessage", (name, message) =>
            {
                // Handle the received message
                Console.WriteLine($"{name}: {message}");
            });

            try
            {
                // Start the connection
                await hubConnection.Start();

                // Invoke a server-side method
                await chatHubProxy.Invoke("SendMessage", "Client", "Hello from client!");
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return 0;
        }

        public async Task SendMessage(string user, string message)
        {
            if (Clients != null)
            {
                //var id = Context.ConnectionId;
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
        }

        public async Task SendUnitAction(string roomID, string actionID, string operatorData)
        {
            try
            {
                int _roomID = 0;
                int.TryParse(roomID, out _roomID);

                if (_roomID > 0)
                {
                    if (_roomID > 0 && Clients != null)
                    {
                        //var id = Context.ConnectionId;
                        await Clients.All.SendAsync("ReceiveUnitAction", _roomID.ToString(), actionID, operatorData);
                    }
                }
            }
            catch (Exception ex) { }
        }

        //server
        public void Join(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }
    }
}