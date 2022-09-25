using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable 618
using static UnityEngine.Networking.NetworkServer;
#pragma warning restore 618

namespace Assets.Scripts.Crystals
{
#pragma warning disable 618
    public class Crystal : NetworkBehaviour
#pragma warning disable 618
    {
        [SerializeField] private GameObject crystalPrefab;
        private List<GameObject> crystals;

        private void Start()
        {
            SpawnCrystal();
        }

        private void SpawnCrystal()
        {

            if (!isServer)
            {
                return;
            }

            crystals = new List<GameObject>();
            for (int i = 0; i < 10; i++)
            {

                crystals.Add(Instantiate(crystalPrefab, transform.position, transform.rotation)); 
                
            }

            foreach (var crystal in crystals)
            {
                SpawnWithClientAuthority(crystal, connectionToClient);
            }
        }
    }
}
