using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using System.Net;
using Newtonsoft.Json;
namespace UVBot.Dialogs
{
    public class OpenData
    {
        public async Task<string> GetUVI(string location)
        {
            string err = "";
            string retStr = "";
            //string token = "KYfUA2YT6kG66dQeBDhWDg";
            string url = $"http://opendata.epa.gov.tw/ws/Data/UV/?$filter=SiteName%20eq%20%27{location}%27&$orderby=PublishTime%20desc&$skip=0&$top=1000&format=json";
            int uvi_value = 0;
            string json = "";
            List<UV> uv = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = Encoding.UTF8;
                    json = webClient.DownloadString(url);
                    uv = JsonConvert.DeserializeObject<List<UV>>(json);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                retStr= "";
            }
            if (uv == null || uv.Count == 0)
                retStr= "";
            else
                retStr = PrepareUVDesp(uv[0].UVI);
            return retStr;

        }
        private string PrepareUVDesp(string UV)
        {
            string outStr = "";
            Decimal UV_Value;
            if (Decimal.TryParse(UV, out UV_Value))
            {
                outStr = string.Format("紫外線指數:{0}", UV);

                if (UV_Value >=0m && UV_Value <=2m)
                    outStr = string.Format("紫外線指數:{0}為[低量級]。很安全", UV);
                if (UV_Value >= 3m && UV_Value <= 5m)
                    outStr = string.Format("紫外線指數:{0}為[中量級]。算是安全, 但不要長時間在外", UV);
                if (UV_Value >= 6m && UV_Value <= 7m)
                    outStr = string.Format("紫外線指數:{0}為[高量級]。指數有點高, 強烈建議做好防曬和戴帽子或撐陽傘", UV);
                if (UV_Value >= 8m && UV_Value <= 10m)
                    outStr = string.Format("紫外線指數:{0}為[過量級]。指數非常高, 能不出去就不出去, 不要長時間在外", UV);
                if (UV_Value >= 11m )
                    outStr = string.Format("紫外線指數:{0}為[危險級]。指數已達危害人體, 強烈建議留在室內", UV);
            }
            else
                outStr=  "找不到紫外線指數";

            return outStr;
        }
    }
}