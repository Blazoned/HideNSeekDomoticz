using System;
using System.Net;
using System.Net.Sockets;
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
        #endregion

        #region Properties
        /// <summary>
        /// The game lobby.
        /// </summary>
        public Lobby Lobby { get; private set; }
        /// <summary>
        /// Gets the server address.
        /// </summary>
        public string Address { get { return _hostListener.Server.RemoteEndPoint.ToString(); } }
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a <see cref="Host"/> for a <see cref="Logic.Lobby"/>.
        /// </summary>
        /// <param name="username">The user's identifier.</param>
        public Host(string username)
        {
            this.Lobby = new Lobby(username);

            IPAddress localAddress = IPAddress.Parse(LOCAL_ADDRESS);
            
            this._hostListener = new TcpListener(localAddress, PORT);
            this._hostListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            this._hostListener.Start();

            this._cancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                while (!this._cancellationTokenSource.IsCancellationRequested)
                {
                    await HostLobby();
                }
            });
        }
        #endregion

        #region Functions
        /// <summary>
        /// Starts the game for every <see cref="Player"/>.
        /// </summary>
        public async void StartGame()
        {
            await Lobby.StartGame();
        }
        /// <summary>
        /// Sets the amount of time the hider has for hiding.
        /// </summary>
        /// <param name="seconds">The amount of seconds.</param>
        public void SetHidingTime(int seconds)
        {
            Lobby.SetHidingTime(seconds);
        }
        /// <summary>
        /// Ends the game for every <see cref="Player"/>
        /// </summary>
        public async void EndGame()
        {
            await Lobby.EndGame();
        }
        /// <summary>
        /// Closes the lobby to new <see cref="Player"/>s.
        /// </summary>
        public void CloseLobby()
        {
            _cancellationTokenSource.Cancel();
        }
        /// <summary>
        /// Disbands the entire lobby.
        /// </summary>
        public async void DisbandLobby()
        {
            await Lobby.DisconnectAsync(Lobby.HostPlayer);
            _hostListener.Stop();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start accepting incomming connections.
        /// </summary>
        /// <returns></returns>
        private async Task HostLobby()
        {
            TcpClient client = await this._hostListener.AcceptTcpClientAsync();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            _ = Task.Run(async () =>
            {
                await Lobby.AddClientAsync(client);
            });
        }
        #endregion

        #region Events
        public delegate void PlayerConnectedHandler();
        public event PlayerConnectedHandler PlayerConnected;
        #endregion
    }
}
