using SAMPQuery;

namespace Tests
{
    public class RconTest
    {
        const string SERVER_HOSTNAME = "eipq.am";
        const string SERVER_IP = "85.208.51.76";
        const ushort SERVER_PORT = 7777;
        const string RCON_COMMAND = "echo Hello world!";
        const string CHANGEME = "changeme";
        const string ANY_RCON_PASSWORD = "hehe";
        const string EMPTY_STRING = "";

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, ANY_RCON_PASSWORD, EMPTY_STRING)]
        [TestCase(SERVER_IP, SERVER_PORT, ANY_RCON_PASSWORD, EMPTY_STRING)]
        public void SendRconCommandAsync_PassingEmptyCommand_ThrowsArgumentException(string host, ushort port, string password, string command)
        {
            var sampQuery = this.CreateDefaultSampQueryInstance(host, port, password);
            AsyncTestDelegate func = async () => await sampQuery.SendRconCommandAsync(command);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Property("ParamName").EqualTo(nameof(command)), func);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, ANY_RCON_PASSWORD, null)]
        [TestCase(SERVER_IP, SERVER_PORT, ANY_RCON_PASSWORD, null)]
        public void SendRconCommandAsync_PassingNullCommand_ThrowsArgumentException(string host, ushort port, string password, string? command)
        {
            var sampQuery = this.CreateDefaultSampQueryInstance(host, port, password);
            AsyncTestDelegate func = async () => await sampQuery.SendRconCommandAsync(command);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Property("ParamName").EqualTo(nameof(command)), func);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, EMPTY_STRING, RCON_COMMAND)]
        [TestCase(SERVER_IP, SERVER_PORT, EMPTY_STRING, RCON_COMMAND)]
        public void SendRconCommandAsync_PassingEmptyPassword_ThrowsArgumentException(string host, ushort port, string password, string command)
        {
            var sampQuery = this.CreateDefaultSampQueryInstance(host, port, password);
            AsyncTestDelegate func = async () => await sampQuery.SendRconCommandAsync(command);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Property("ParamName").EqualTo(nameof(password)), func);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, null, RCON_COMMAND)]
        [TestCase(SERVER_IP, SERVER_PORT, null, RCON_COMMAND)]
        public void SendRconCommandAsync_PassingNullPassword_ThrowsArgumentException(string host, ushort port, string? password, string command)
        {
            var sampQuery = this.CreateDefaultSampQueryInstance(host, port, password);
            AsyncTestDelegate func = async () => await sampQuery.SendRconCommandAsync(command);

            Assert.ThrowsAsync(Is.TypeOf<ArgumentNullException>().And.Property("ParamName").EqualTo(nameof(password)), func);
        }

        [Test]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, CHANGEME, RconPasswordExceptionMessages.CHANGEME_NOT_ALLOWED)]
        [TestCase(SERVER_IP, SERVER_PORT, CHANGEME, RconPasswordExceptionMessages.CHANGEME_NOT_ALLOWED)]
        [TestCase(SERVER_HOSTNAME, SERVER_PORT, ANY_RCON_PASSWORD, RconPasswordExceptionMessages.INVALD_RCON_PASSWORD)]
        [TestCase(SERVER_IP, SERVER_PORT, ANY_RCON_PASSWORD, RconPasswordExceptionMessages.INVALD_RCON_PASSWORD)]
        public void SendRconCommandAsync_Throws_RconPasswordException(string host, ushort port, string password, string message)
        {
            var sampQuery = this.CreateDefaultSampQueryInstance(host, port, password);
            AsyncTestDelegate func = async () => await sampQuery.SendRconCommandAsync(RCON_COMMAND);

            Assert.ThrowsAsync(Is.TypeOf<RconPasswordException>().And.Message.EqualTo(message), func);
        }

        private SampQuery CreateDefaultSampQueryInstance(string ip, ushort port, string password)
        {
            var sampQuery = new SampQuery(ip, port, password);
            return sampQuery;
        }
    }
}