using System.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;
using SampQueryApi;

namespace Tests
{
    // This test may fail if more than 100 players are playing on the server!!!
    public class ServerPlayersTest
    {
        private SampQuery sampQuery;

        [SetUp]
        public void Setup()
        {
            this.sampQuery = new SampQuery("glow-dm.ru", 6666);
        }

        [Test]
        public void RequestServerPlayers_And_CheckResponse_Sync()
        {   
            var response = this.sampQuery.GetServerPlayers();

            Assert.IsInstanceOf<IEnumerable<SampServerPlayerData>>(response);
            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(SampServerPlayerData));
        }

        [Test]
        public async Task RequestServerPlayers_And_CheckResponse_Async()
        {   
            var response = await this.sampQuery.GetServerPlayersAsync();

            Assert.IsInstanceOf<IEnumerable<SampServerPlayerData>>(response);
            CollectionAssert.AllItemsAreInstancesOfType(response, typeof(SampServerPlayerData));
        }
    }
}