using Characters;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _InputField;

        Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public class MessageLogin : MessageBase
        {
            public string login;

            public override void Deserialize(NetworkReader reader)
            {
                login = reader.ReadString();
            }

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(login);
            }
        }

        public override void OnClientConnect(NetworkConnection connect)
        {
            base.OnClientConnect(connect);
            MessageLogin _login = new MessageLogin();
            _login.login = _InputField.text;
            connect.Send(100, _login);
        }

        public void ReceiveLogin(NetworkMessage message)
        {
            _players[message.conn.connectionId].PlayerName = message.reader.ReadString();
            _players[message.conn.connectionId].gameObject.name = _players[message.conn.connectionId].PlayerName;
        }  

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(100, ReceiveLogin);
        }
    }
}
