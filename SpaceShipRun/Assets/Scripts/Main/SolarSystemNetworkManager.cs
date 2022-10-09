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
using Unity.Collections;
using UnityEngine.UIElements;
using System.Reflection;
#pragma warning restore 618

namespace Main
{
#pragma warning restore 618
    public class SolarSystemNetworkManager : NetworkManager
#pragma warning restore 618
    {
        struct FractalPart
        {
            public Vector3 WorldPosition;
            public Quaternion WorldRotation;
            public float SpinAngle;
        }

        #region Asteroid
        [SerializeField] private Mesh _meshAsteroid;
        [SerializeField] private Material _materialAsteroid;
        [SerializeField, Range(10, 100)] private int _depthAsteroid = 10;
        [SerializeField, Range(0, 360)] private int _speedRotationAsteroid = 80;
        private const float _positionOffset = 1.5f;
        private const float _scaleBias = .5f;
        private const int _childCount = 5;
        private FractalPart[] _parts;
        private Matrix4x4[] _matrices;
        private ComputeBuffer _matricesBuffers;
        private static readonly int _matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock _propertyBlock;

        private static readonly Vector3[] _directions =
        {
            Vector3.up,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        private static readonly Quaternion[] _rotations =
        {
            Quaternion.identity,
            Quaternion.Euler(.0f, .0f, 90.0f),
            Quaternion.Euler(.0f, .0f, -90.0f),
            Quaternion.Euler(90.0f, .0f, .0f),
            Quaternion.Euler(-90.0f, .0f, .0f)
        };
        #endregion

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

        private void OnEnable()
        {
            _parts = new FractalPart[_depthAsteroid];
            _matrices = new Matrix4x4[_depthAsteroid];
            //_matricesBuffers = new ComputeBuffer();
            var stride = 16 * 4;
            for (int i = 0, length = 1; i < _parts.Length; i++, length++)
            {
                _parts[i] = new FractalPart();
                _matrices[i] = _matrices[i] = new Matrix4x4();
                _matricesBuffers = new ComputeBuffer(length, stride);
            }
            _parts[0] = CreatePart(0);
            for (var li = 1; li < _parts.Length; li++)
            {
                _parts[li] = CreatePart(li);

            }
            _propertyBlock ??= new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            _matricesBuffers.Dispose();
            _parts = null;
            _matrices = null;
            _matricesBuffers = null;
        }
        private void OnValidate()
        {
            if (_parts is null || !enabled)
            {
                return;
            }

            OnDisable();
            OnEnable();
        }
        private FractalPart CreatePart(int i) => new FractalPart
        {
            WorldPosition = _directions[i],
            WorldRotation = _rotations[i],
        };

        private void Update()
        {
            var spinAngelDelta = _speedRotationAsteroid * Time.deltaTime;
            var rootPart = _parts[0];
            rootPart.SpinAngle += spinAngelDelta;
            var deltaRotation = Quaternion.Euler(.0f, rootPart.SpinAngle, .0f);
            rootPart.WorldRotation = rootPart.WorldRotation * deltaRotation;
            _parts[0] = rootPart;
            _matrices[0] = Matrix4x4.TRS(rootPart.WorldPosition,
                rootPart.WorldRotation, Vector3.one);
            var scale = 1.0f;
            int li;
            for (li = 1; li < _parts.Length; li++)
            {
                var parent = _parts[li / _childCount];
                var part = _parts[li];
                part.SpinAngle += spinAngelDelta;
                deltaRotation = Quaternion.Euler(.0f, part.SpinAngle, .0f);
                part.WorldRotation = parent.WorldRotation * part.WorldRotation * deltaRotation;
                part.WorldPosition = parent.WorldPosition +
                                     parent.WorldRotation * (_positionOffset * scale * part.WorldPosition);
                _parts[li] = part;
                _matrices[li] = Matrix4x4.TRS(part.WorldPosition, part.WorldRotation, scale * Vector3.one);
            }

            var bounds = new Bounds(rootPart.WorldPosition, 3f * Vector3.one);
            var buffer = _matricesBuffers;
                buffer.SetData(_matrices);
                _propertyBlock.SetBuffer(_matricesId, buffer);
                _materialAsteroid.SetBuffer(_matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(_meshAsteroid, 0, _materialAsteroid, bounds, buffer.count, _propertyBlock);
            
        }
    }
}
