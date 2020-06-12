using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HideNSeek.DAL.Seeker
{
    /// <summary>
    /// Defines the implementation for data access of a Domoticz hiding system.
    /// </summary>
    public class DomoticzHider : IHiderDAL
    {
        #region Constants
        private const string USERVARNAME = "Path";
        #endregion

        #region Fields
        private HttpClient _webClient;
        #endregion

        #region Properties
        public string ApiAddressBase { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initiate an object for accessing Domoticz hiding system data.
        /// </summary>
        /// <param name="ipAddress">The address of the Domoticz server.</param>
        /// <param name="port">The port of the Domoticz server.</param>
        public DomoticzHider(string ipAddress, string port)
        {
            this._webClient = new HttpClient();
            this.ApiAddressBase = $"http://{ipAddress}:{port}";

            this._webClient.BaseAddress = new Uri(this.ApiAddressBase);
            this._webClient.DefaultRequestHeaders.Accept.Clear();
            this._webClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Functions
        /// <summary>
        /// Gets the room names of each room on the hiders floorplan.
        /// </summary>
        /// <returns>A list of room names.</returns>
        public IEnumerable<string> GetRooms()
        {
            dynamic results = GetFloorplanRooms();

            List<string> rooms = new List<string>();

            foreach (dynamic room in results)
            {
                rooms.Add(Convert.ToString(room.Name));
            }

            return rooms;
        }
        /// <summary>
        /// Signal to the hiding system that a new game has started.
        /// </summary>
        public void StartHiding()
        {
            ResetPath();
            // StartHidingScene();
        }

        /// <summary>
        /// Gets the hiders current trail.
        /// </summary>
        /// <returns>The path the hider has taken through the house.</returns>
        public string GetCurrentPath()
        {
            dynamic results = GetUserVariables();
            
            foreach (dynamic uVar in results)
            {
                if (Convert.ToString(uVar.Name) == "Path")
                    return Convert.ToString(uVar.Value);
            }

            return string.Empty;
        }
        /// <summary>
        /// Gets the hider's current position.
        /// </summary>
        /// <returns>The name of the room the hider is currently in.</returns>
        public string GetCurrentRoom()
        {
            // Retrieve path
            dynamic results = GetUserVariables();
            string path = string.Empty;

            foreach (dynamic uVar in results)
            {
                if (Convert.ToString(uVar.Name) == "Path")
                    path = Convert.ToString(uVar.Value);
            }

            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Get last activated device
            string[] devices = path.Split('|');
            string[] device = devices.LastOrDefault().Split('/');

            // Get room of last activated device
            dynamic rooms = GetFloorplanRooms();

            foreach (dynamic room in rooms)
            {
                int id = Convert.ToInt32(room.idx);

                dynamic roomDevices = GetRoomDevices(id);

                foreach (dynamic roomDevice in roomDevices)
                {
                    if (Convert.ToString(roomDevice.Name) == device.FirstOrDefault())
                        return Convert.ToString(room.Name);
                }
            }

            // No matching room found
            return string.Empty;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Call the Domoticz web api.
        /// </summary>
        /// <param name="apiPath">The api path.</param>
        /// <returns>The contents of the api response.</returns>
        private string CallApi(string apiPath)
        {
            string apiCall = $"/json.htm?{apiPath}";

            HttpResponseMessage response = _webClient.GetAsync(apiCall).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Reset the hider's path.
        /// </summary>
        private void ResetPath()
        {
            string apiPath = "type=command&" +
                             "param=updateuservariable&" +
                             $"vname={USERVARNAME}&" +
                             "vtype=2&" +
                             "vvalue=";

            CallApi(apiPath);
        }
        /// <summary>
        /// Start the hiding emulation via a scene.
        /// </summary>
        private void StartHidingScene()
        {
            string apiPath = "type=command&" +
                             "param=switchscene&" +
                             "idx=1&" +
                             "switchcmd=On";

            CallApi(apiPath);
        }

        /// <summary>
        /// Get the rooms available in the floorplan.
        /// </summary>
        /// <returns>A list of rooms.</returns>
        private dynamic GetFloorplanRooms()
        {
            string apiPath = "type=plans&" +
                             "order=name&" +
                             "used=true";

            string results = CallApi(apiPath);
            return JsonConvert.DeserializeObject<dynamic>(results).result;
        }
        /// <summary>
        /// Get the devices available in a room.
        /// </summary>
        /// <param name="roomId">The room's identifier.</param>
        /// <returns>A list of devices.</returns>
        private dynamic GetRoomDevices(int roomId)
        {
            string apiPath = "type=command&" +
                             "param=getplandevices&" +
                             $"idx={roomId}";

            string results = CallApi(apiPath);
            return JsonConvert.DeserializeObject<dynamic>(results).result;
        }
        /// <summary>
        /// Get all user variables.
        /// </summary>
        /// <returns>A list of user variables.</returns>
        private dynamic GetUserVariables()
        {
            string apiPath = "type=command&" +
                             "param=getuservariables";

            string results = CallApi(apiPath);
            return JsonConvert.DeserializeObject<dynamic>(results).result;
        }
        #endregion
    }
}
