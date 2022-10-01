using System;
using Assets.Scripts.Crystals;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        protected override float speed => shipSpeed;

        [SerializeField] private Transform cameraAttach;
        private CameraOrbit cameraOrbit;
        private PlayerLabel playerLabel;
        private float shipSpeed;
        private Rigidbody rb;

        private SolarSystemNetworkManager _manager;

        [SyncVar] private string playerName;

        private void OnGUI()
        {
            if (cameraOrbit == null)
            {
                return;
            }
            cameraOrbit.ShowPlayerLabels(playerLabel);
        }

        public override void OnStartAuthority()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                return;
            }
            gameObject.name = playerName;
            cameraOrbit = FindObjectOfType<CameraOrbit>();
            cameraOrbit.Initiate(cameraAttach == null ? transform : cameraAttach);
            playerLabel = GetComponentInChildren<PlayerLabel>();
            _manager = GameObject.Find("Main").GetComponent<SolarSystemNetworkManager>();
            base.OnStartAuthority();
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)
            {
                return;
            }

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            shipSpeed = Mathf.Lerp(shipSpeed, speed * faster,
                SettingsContainer.Instance.SpaceShipSettings.Acceleration);

            var currentFov = isFaster
                ? SettingsContainer.Instance.SpaceShipSettings.FasterFov
                : SettingsContainer.Instance.SpaceShipSettings.NormalFov;
            cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);

            var velocity = cameraOrbit.transform.TransformDirection(Vector3.forward) * shipSpeed;
            rb.velocity = velocity * Time.deltaTime;

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(
                    Quaternion.AngleAxis(cameraOrbit.LookAngle, -transform.right) *
                    velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        protected override void FromServerUpdate() { }
        protected override void SendToServer() { }

        [ClientCallback]
        private void LateUpdate()
        {
            cameraOrbit?.CameraMovement();
        }

        [ServerCallback]
        public void OnTriggerEnter(Collider collider)
        {
            var crystalView = collider.gameObject.GetComponent<CrystalView>();
            if (crystalView != null)
            {
                crystalView.gameObject.SetActive(false);
                CmdUpdateCrystal(crystalView.gameObject);
                return;
            }
            var rand = new Random();
            var newPosition = new Vector3(rand.Next(-500, 500), rand.Next(-500, 500), rand.Next(-500, 500));

            RpcChangePosition(newPosition);
            transform.position = newPosition;

        }

        [Command]
        private void CmdUpdateCrystal(GameObject gameObject)
        {
            _manager.UpdateCrystal(gameObject, playerName);
        }


        [ClientRpc]
        private void RpcChangePosition(Vector3 position)
        {
            gameObject.SetActive(false);
            transform.position = position;
            gameObject.SetActive(true);
        }
    }
}
