using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

//TODO Add using
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using System.Linq;
using UVBot.Dialogs;
namespace UVBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //語意處理
                //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                await Conversation.SendAsync(activity, () => new UVDialog());
            }
            else
            {
                //HandleSystemMessage(activity);
                //TODO chage as below
                await HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        //系統訊息處理
        //private Activity HandleSystemMessage(Activity message)
        //TODO chage as below
        private async Task<HttpResponseMessage> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                IConversationUpdateActivity update = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();

                    if (update.MembersAdded.Any())
                    {
                        var reply = message.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != message.Recipient.Id)
                            {
                                reply.Text = $"我是UV Bot, 歡迎 {newMember.Name}!";
                            }
                            else
                            {
                                //reply.Text = $"我是UV Bot, 歡迎 {message.From.Name}";
                            }
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}