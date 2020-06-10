using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HideNSeek.Logic
{
    /// <summary>
    /// The <see cref="Host"/> allows a player to host a lobby.
    /// </summary>
    public class Host
    {
        #region Constants
        const int PORT = 25555;
        const string LOCAL_ADDRESS = "127.0.0.1";
        #endregion

        #region Fields
        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _hostListener;
        private Dictionary<string, TcpClient> _clients;
        #endregion

        #region Properties
        /// <summary>
        /// The game lobby.
        /// </summary>
        internal Lobby Lobby { get; private set; }
        /// <summary>
        /// The host's player object.
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// Gets the server address.
        /// </summary>
        public string Address { get { return _hostListener.Server.RemoteEndPoint.ToString(); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a <see cref="Host"/> for a <see cref="Logic.Lobby"/>.
        /// </summary>
        public Host()
        {
            this.Lobby = new Lobby(this);

            IPAddress localAddress = IPAddress.Parse(LOCAL_ADDRESS);
            this._hostListener = new TcpListener(localAddress, PORT);

            this._cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                while (!this._cancellationTokenSource.IsCancellationRequested)
                {
                    await HostLobby();
                }
            });

            // TODO: Add player for host
            this.Player = null;
        }
        #endregion

        #region Functions
        public void StartGame()
        {
            Lobby.StartGame();
        }
        public void SetHidingTime(int seconds)
        {
            Lobby.SetHidingTime(seconds);
        }
        public void EndGame()
        {
            Lobby.EndGame();
        }
        public void CloseLobby()
        {
            _cancellationTokenSource.Cancel();
        }
        public TcpClient GetClient(string username)
        {
            if (_clients.ContainsKey(username))
                return _clients[username];

            return null;
        }
        #endregion

        #region Methods
        private async Task HostLobby()
        {
            TcpClient client = await this._hostListener.AcceptTcpClientAsync();
            _ = Task.Run(async () =>
            {
                await HandleTcpClientAsync(client);
            });
        }
        /// <summary>
        /// Handle new tcp client connections and store them in a dictionary.
        /// </summary>
        /// <param name="client">The new tcp client.</param>
        /// <returns></returns>
        private async Task HandleTcpClientAsync(TcpClient client)
        {
            // Open stream
            using (NetworkStream ns = client.GetStream())
            using (StreamReader sr = new StreamReader(ns))
            {
                int success = -1;

                // Get the new client's identifier
                string username = await sr.ReadLineAsync();

                if (!_clients.ContainsKey(username))
                {
                    _clients.Add(username, client);
                    Lobby.A
                    success = Lobby.HidingTime;
                }

                // Send the new client either a code of failure (-1) or the hiding time.
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    sw.WriteLine(success);
                }
            }
        }
        #endregion
    }
}
