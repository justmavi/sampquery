using System.Threading.Tasks;
using NUnit.Framework;
using SampQueryApi;

namespace Tests
{
    public class ServerRulesTest
    {
        private SampQuery sampQuery;

        [SetUp]
        public void Setup()
        {
            this.sampQuery = new SampQuery("glow-dm.ru", 6666);
        }

        [Test]
        public void RequestServerRules_And_CheckResponse_Sync()
        {   
            var response = this.sampQuery.GetServerRules();

            Assert.IsInstanceOf(typeof(SampServerRulesData), response);
        }

        [Test]
        public async Task RequestServerRules_And_CheckResponse_Async()
        {   
            var response = await this.sampQuery.GetServerRulesAsync();

            Assert.IsInstanceOf(typeof(SampServerRulesData), response);
        }
    }
}