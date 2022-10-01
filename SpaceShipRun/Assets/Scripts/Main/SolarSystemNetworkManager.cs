using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = System.Random;
#pragma warning disable 618
using static UnityEngine.Networking.NetworkServer;
#pragma warning restore 618

namespace Main
{
#pragma warning restore 618
    public class SolarSystemNetworkManager : NetworkManager
#pragma warning restore 618
    {

        [SerializeField] private TMP_InputField playerInputField;
        [SerializeField] private TMP_Text tmpText;
        [SerializeField] private GameObject crystalPrefab;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform lineParent;
        public List<GameObject> crystals;

        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
        private Dictionary<string, int> _points = new Dictionary<string, int>();

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            //player.GetComponent<ShipController>().PlayerName = playerName;
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            _points.Add(playerInputField.text, 0);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            SpawnCrystal();
        }

        public void UpdateCrystal(GameObject crystal, string playerName)
        {
            crystals.Remove(crystal);
            NetworkServer.Destroy(crystal);
            _points[playerName] += 10;
            if (crystals.Count <= 0)
            {
                SpawnScoreTable();
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(100, ReceiveName);
            
        }

        public class MassageLogin : MessageBase
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

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);
            MassageLogin _login = new MassageLogin();
            _login.login = playerInputField.text;
            connection.Send(100, _login);
            playerInputField.gameObject.SetActive(false);
            tmpText.gameObject.SetActive(false);
        }

        public void ReceiveName(NetworkMessage networkMessage)
        {
            _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
            _players[networkMessage.conn.connectionId].gameObject.name =
                _players[networkMessage.conn.connectionId].PlayerName;
        }

        public void SpawnScoreTable()
        {
            List<GameObject> lines = new List<GameObject>();
            for (int i = 0; i < _players.Count; i++)
            {
                var line = Instantiate(linePrefab, lineParent);
                lines.Add(line);
            }
            _points = _points.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            int position = 1;
            foreach (var line in lines)
            {
                var l = line.GetComponent<LineView>();
                l.Position.text = position.ToString();
                int max = _points.Max(x => x.Value);
                l.Name.text = _points.FirstOrDefault(x => x.Value == max).Key.ToString();
                l.points.text = _points.Max(x => x.Value == max).ToString();
                _points.Remove(_points.FirstOrDefault(x => x.Value == max).Key);
            }

        }

        public void SpawnCrystal()
        {
            crystals = new List<GameObject>();
            var rand = new Random();
            for (int i = 0; i < 1; i++)
            {
                var pos = new Vector3(rand.Next(-500, 500), rand.Next(-50, 50), rand.Next(-500, 500));
                crystals.Add(Instantiate(crystalPrefab, pos, transform.rotation));

            }

            foreach (var crystal in crystals)
            {
                NetworkServer.Spawn(crystal);
            }
        }
    }
}
