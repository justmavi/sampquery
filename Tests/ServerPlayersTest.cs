using SAMPQuery;

namespace Tests
{
    // This test may fail if more than 100 players are playing on the server!!!
    public class ServerPlayersTest
    {
        const string SERVER_HOSTNAME = "eipq.am";
        const string SERVER_IP = "85.208.51.76";
        const ushort SERVER_PORT = 7777;

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerPlayersAsync_Returns_CollectionOfServerPlayerInstances(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerPlayersAsync();

            Assert.That(response, Is.All.InstanceOf<ServerPlayer>());
        }
        
        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}