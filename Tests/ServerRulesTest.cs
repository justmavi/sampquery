using System.Threading.Tasks;
using NUnit.Framework;
using SAMPQuery;

namespace Tests
{
    public class ServerRulesTest
    {
        const string SERVER_HOSTNAME = "glow-dm.ru";
        const string SERVER_IP = "46.105.144.50";
        const ushort SERVER_PORT = 6666;

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public void GetServerRules_Returns_SampServerRulesDataInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerRules();

            Assert.IsInstanceOf(typeof(SampServerRulesData), response);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerRulesAsync_Returns_SampServerRulesDataInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerRulesAsync();

            Assert.IsInstanceOf(typeof(SampServerRulesData), response);
        }

        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}