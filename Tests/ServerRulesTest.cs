using SAMPQuery;

namespace Tests
{
    public class ServerRulesTest
    {
        const string SERVER_HOSTNAME = "eipq.am";
        const string SERVER_IP = "85.208.51.76";
        const ushort SERVER_PORT = 7777;

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public void GetServerRules_Returns_ServerRulesInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerRules();

            Assert.That(response, Is.InstanceOf<ServerRules>());
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerRulesAsync_Returns_ServerRulesInstance(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerRulesAsync();

            Assert.That(response, Is.InstanceOf<ServerRules>());
        }

        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}