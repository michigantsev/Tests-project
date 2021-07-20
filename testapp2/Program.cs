using Newtonsoft.Json;
using NUnit.Framework;
using NUnitLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace testapp2
{
    class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun().Execute(args);
        }
    }
    [TestFixture]
    public class Tests
    {
        [Test]
        public void CompareTimeXml()
        {
            bool ok = false;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            HttpResponseMessage response
                = client.GetAsync("http://demo.macroscop.com:8080/command?type=gettime&login=root&password=").Result;
            XDocument xdoc = XDocument.Parse(response.Content.ReadAsStringAsync().Result);
            string temp = xdoc.Element("string").Value;
            DateTime serverTime = DateTime.Parse(temp);
            DateTime localTime = DateTime.Now;
            TimeSpan timeDiff = serverTime - localTime;
            if (timeDiff.TotalSeconds <= 15)
                ok = true;
            Assert.IsTrue(ok);
        }
        [Test]
        public async Task CompareTimeJson()
        {
            bool ok = false;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response
                = await client.GetAsync("http://demo.macroscop.com:8080/command?type=gettime&login=root&password=&responsetype=json");
            string b = await response.Content.ReadAsStringAsync();
            var temp = JsonConvert.DeserializeObject(b);
            DateTime serverTime = DateTime.Parse(temp.ToString());
            DateTime localTime = DateTime.Now;
            TimeSpan timeDiff = serverTime - localTime;
            if (timeDiff.TotalSeconds <= 15)
                ok = true;
            Assert.IsTrue(ok);
        }
        [Test]
        public void ChannelsCount()
        {
            int expected = 6;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            HttpResponseMessage response
                = client.GetAsync("http://demo.macroscop.com:8080/configex?login=root&password=").Result;
            XDocument xdoc = XDocument.Parse(response.Content.ReadAsStringAsync().Result);
            Assert.GreaterOrEqual(xdoc.Root
                .Element("Channels")
                .Elements()
                .Count(), expected);
        }
    }
}
