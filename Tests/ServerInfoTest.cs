using System.Threading.Tasks;
using NUnit.Framework;
using SampQueryApi;

namespace Tests
{    
    public class ServerInfoTest
    {
        private SampQuery sampQuery;

        [SetUp]
        public void Setup()
        {
            this.sampQuery = new SampQuery("glow-dm.ru", 6666);
        }

        [Test]
        public void RequestServerInfo_And_CheckResponse_Sync()
        {   
            var response = this.sampQuery.GetServerInfo();

            Assert.IsInstanceOf(typeof(SampServerInfoData), response);
        }

        [Test]
        public async Task RequestServerInfo_And_CheckResponse_Async()
        {   
            var response = await this.sampQuery.GetServerInfoAsync();

            Assert.IsInstanceOf(typeof(SampServerInfoData), response);
        }
    }
}