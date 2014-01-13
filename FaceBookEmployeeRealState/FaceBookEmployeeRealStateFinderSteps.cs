using System;
using System.Collections.Generic;
using AlteryxGalleryAPIWrapper;
using AnalogStoreAnalysis;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FaceBookEmployeeRealState
{
    [Binding]
    public class FaceBookEmployeeRealStateFinderSteps
    {
        public string alteryxurl;
        public string _sessionid;
        private string _appid; //= "506f68147ae24a0724305f78";
        private string _userid;
        private string _appName;
        private string jobid;
        private string outputid;
        private string validationId;

        // public delegate void DisposeObject();
        //private Client Obj = new Client("https://devgallery.alteryx.com/api/");
        Client Obj = new Client("https://gallery.alteryx.com/api");
        RootObject jsString = new RootObject();


        [Given(@"alteryx running at ""(.*)""")]
        public void GivenAlteryxRunningAt(string url)
        {
            alteryxurl = url;
        }

        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {
            _sessionid = Obj.Authenticate(user, password).sessionId;
        }

        [When(@"I run analog store analysis with NumberOfFaceBookShare (.*) NumberOfBedRooms (.*) NumberOfBathRooms (.*)")]
        public void WhenIRunAnalogStoreAnalysisWithNumberOfFaceBookShareNumberOfBedRoomsNumberOfBathRooms(string RangeOfFaceBookSharePrice, int NumberOfBedRooms, int NumberOfBathRooms)
        {
            string response = Obj.SearchApps("Facebook Employee");
            var appresponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(response);
            int count = appresponse["recordCount"];
            _userid = appresponse["records"][0]["owner"]["id"];
            _appName = appresponse["records"][0]["primaryApplication"]["fileName"];
            _appid = appresponse["records"][0]["id"];
            jsString.appPackage.id = _appid;
            jsString.userId = _userid;
            jsString.appName = _appName;
            string appinterface = Obj.GetAppInterface(_appid);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);

            List<Jsonpayload.Question> questionAnsls = new List<Jsonpayload.Question>();


            var PriceMax = new List<Jsonpayload.datac>();
            PriceMax.Add(new Jsonpayload.datac() { key = "/1200000\"", value = "true" });
            var Beds = new List<Jsonpayload.datac>();
            Beds.Add(new Jsonpayload.datac() { key = "/beds-1\"", value = "true" });
            var Bath = new List<Jsonpayload.datac>();
            Bath.Add(new Jsonpayload.datac() { key = "/baths-1\"", value = "true" });

            string PriceRange = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(PriceMax);
            string NumOfBed = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(Beds);
            string NumOfBath = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(Bath);
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    Jsonpayload.Question questionAns = new Jsonpayload.Question();
                    questionAns.name = "PriceMax";
                    questionAns.answer = PriceRange;
                    jsString.questions.Add(questionAns);
                }
                else if (i == 1)
                {
                    Jsonpayload.Question questionAns = new Jsonpayload.Question();
                    questionAns.name = "Beds";
                    questionAns.answer = NumOfBed;
                    jsString.questions.Add(questionAns);
                }
                else
                {
                    Jsonpayload.Question questionAns = new Jsonpayload.Question();
                    questionAns.name = "Beds";
                    questionAns.answer = NumOfBath;
                    jsString.questions.Add(questionAns);
                    // jsString.questions.AddRange(questionAnsls);
                }
            }


            // jsString.questions.AddRange(questionAnsls);
            jsString.jobName = "Job Name";
            var postData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsString);
            string postdata = postData.ToString();
            string resjobqueue = Obj.QueueJob(postdata);
            var jobqueue = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(resjobqueue);
            jobid = jobqueue["id"];

            int counts = 0;
            string status = "";

        CheckValidate:
            System.Threading.Thread.Sleep(1000);
            if (status == "Completed" && counts < 15)
            {
                //string disposition = validationStatus.disposition;
            }
            else if (counts < 15)
            {
                string jobstatusresp = Obj.GetJobStatus(jobid);
                var statusResponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(jobstatusresp);
                status = statusResponse["status"];
                goto CheckValidate;
            }

            else
            {
                throw new Exception("Complete Status Not found");

            }
        }

        [Then(@"I see the Facebook Shares result ""(.*)""")]
        public void ThenISeeTheFacebookSharesResult(string facebookShare)
        {
            string getmetadata = Obj.GetOutputMetadata(jobid);
            dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);
            int count = metadataresp.Count;
            for (int j = 0; j <= count - 1; j++)
            {
                outputid = metadataresp[j]["id"];
            }
            string getjoboutput = Obj.GetJobOutput(jobid, outputid, "html");
            string htmlresponse = getjoboutput;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlresponse);
            string output = doc.DocumentNode.SelectSingleNode("//div[@class='DefaultText']").InnerText ;
            //decimal finaloutput = Math.Round(output, 2);
            StringAssert.Contains(facebookShare, output);
            // ScenarioContext.Current.Pending();
        }
    }
}
