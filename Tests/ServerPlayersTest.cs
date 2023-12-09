using System.Threading.Tasks;
using NUnit.Framework;
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
        public void GetServerPlayers_Returns_CollectionOfServerPlayerInstances(string hostname, ushort port)
        {

            /* Unmerged change from project 'Tests (net7)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (net5)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (netcoreapp3.1)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */
            var sampQuery = CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerPlayers();

            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(ServerPlayer));
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerPlayersAsync_Returns_CollectionOfServerPlayerInstances(string hostname, ushort port)
        {

            /* Unmerged change from project 'Tests (net7)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (net5)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (netcoreapp3.1)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerPlayersTest.CreateDefaultSampQueryInstance(hostname, port);
            */
            var sampQuery = CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerPlayersAsync();

            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(ServerPlayer));
        }

        private static SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}