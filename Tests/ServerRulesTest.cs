using System.Threading.Tasks;
using NUnit.Framework;
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

            /* Unmerged change from project 'Tests (net7)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (net5)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (netcoreapp3.1)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */
            var sampQuery = CreateDefaultSampQueryInstance(hostname, port);
            var response = sampQuery.GetServerRules();

            Assert.IsInstanceOf(typeof(ServerRules), response);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT)]
        [TestCase(SERVER_IP, SERVER_PORT)]
        public async Task GetServerRulesAsync_Returns_ServerRulesInstance(string hostname, ushort port)
        {

            /* Unmerged change from project 'Tests (net7)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (net5)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */

            /* Unmerged change from project 'Tests (netcoreapp3.1)'
            Before:
                        var sampQuery = this.CreateDefaultSampQueryInstance(hostname, port);
            After:
                        var sampQuery = ServerRulesTest.CreateDefaultSampQueryInstance(hostname, port);
            */
            var sampQuery = CreateDefaultSampQueryInstance(hostname, port);
            var response = await sampQuery.GetServerRulesAsync();

            Assert.IsInstanceOf(typeof(ServerRules), response);
        }

        private static SampQuery CreateDefaultSampQueryInstance(string ip, ushort port)
        {
            var sampQuery = new SampQuery(ip, port);
            return sampQuery;
        }
    }
}