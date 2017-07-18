using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//TODO Using
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace UVBot.Dialogs
{
    //TODO 
    [LuisModel("查一下", "查一下")]
    [Serializable]
    public class UVDialog : LuisDialog<Object>
    {
        //TODO declare static var for entiy
        private const string Entity_location = "地點";
        //TODO declare static var for entiy
        private enum IntentEnum { 問紫外線};
        public UVDialog()
        {
            //_activity = activity;
        }
        public UVDialog(ILuisService service)
            : base(service)
        {

        }
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {

            string missStr = GetMissUnderstood();
            //string message = $"{missStr}: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            string message = $"{missStr} ";


            await context.PostAsync(message);
            context.Wait(MessageReceived);

        }
        private string GetMissUnderstood()
        {
            DateTime now = DateTime.Now;
            string outStr = "歹勢~我不太了您說的耶~";
            if (now.Second > 40)
                outStr = "可以再明確點嗎~";
            else if (now.Second > 20)
                outStr = "不好意思, 不太懂~";

            return outStr;
        }
        [LuisIntent("問紫外線")]
        public async Task AskUV(IDialogContext context, LuisResult result)
        {

            EntityRecommendation location;
            var reply = context.MakeMessage();
            OpenData openData = new OpenData();
            string inq_loc = "";
            string UV_GOV_URL = "http://www.cwb.gov.tw/V7/observe/UVI/UVI.htm";
            if (result.Query.Contains("全台"))
            {
                reply.Text = UV_GOV_URL;
            }
            else
            {
                result.TryFindEntity(Entity_location, out location);

                if (location == null)
                {
                    inq_loc = "臺北";
                }
                else
                    inq_loc = location.Entity;
                string UVCommand = await openData.GetUVI(inq_loc);
                if (!String.IsNullOrEmpty(UVCommand))
                {

                    reply.Text = inq_loc + "的" + UVCommand + "。";
                    if (location == null)
                    {
                        reply.Text += "全台紫外線指數:" + UV_GOV_URL;

                    }
                }

                else
                    //Get Entity
                    reply.Text = "無法查詢您輸入地點的紫外線指數";
            }
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        private string UVLocationMapping(string inStr)
        {
            string outStr = inStr.Replace("台", "臺");
            return outStr;
        }
    }
}