using System.Threading.Tasks;
using NUnit.Framework;
using SampQueryApi;

namespace Tests
{
    // This test may fail if more than 100 players are playing on the server!!!
    public class ServerPlayersTest
    {
        const string SERVER_HOSTNAME = "glow-dm.ru";
        const string SERVER_IP = "46.105.144.50";
        const ushort SERVER_PORT = 6666;

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public void GetServerPlayers_Returns_CollectionOfSampServerPlayerDataInstances(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerPlayers();

            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(SampServerPlayerData));
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerPlayersAsync_Returns_CollectionOfSampServerPlayerDataInstances(string hostname, ushort port)
        {   
            var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerPlayersAsync();

            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(SampServerPlayerData));
        }
        
        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}