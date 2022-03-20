using System.Threading.Tasks;
using NUnit.Framework;
using SampQueryApi;

namespace Tests
{    
    public class ServerInfoTest
    {
        const string SERVER_HOSTNAME = "glow-dm.ru";
        const string SERVER_IP = "46.105.144.50";
        const ushort SERVER_PORT = 6666;

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public void GetServerInfo_Returns_SampServerInfoDataInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerInfo();

            Assert.IsInstanceOf(typeof(SampServerInfoData), response);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerInfoAsync_Returns_SampServerInfoDataInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerInfoAsync();

            Assert.IsInstanceOf(typeof(SampServerInfoData), response);
        }

        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}