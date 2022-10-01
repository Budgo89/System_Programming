using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
public abstract class Character : NetworkBehaviour
{
    protected Action OnUpdateAction { get; set; }
    protected abstract FireAction fireAction { get; set; }
    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Quaternion serverRotarion;
    [SyncVar] protected int serverDps;
    [SyncVar] protected int health;
    protected virtual void Initiate()
    {
        OnUpdateAction += Movement;
    }
    private void Update()
    {
        OnUpdate();
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(transform.forward);
            if (Physics.Raycast(ray, out hit))
            {
                var player = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                if (player != null)
                {
                    CmdShooting(50);
                }
            }
        }
    }
    private void OnUpdate()
    {
        OnUpdateAction?.Invoke();
    }
    [Command]
    protected void CmdUpdatePosition(Vector3 position, Quaternion rotarion)
    {
        serverPosition = position;
        serverRotarion = rotarion;

    }

    [Command]
    protected void CmdShooting(int dps)
    {
        serverDps = dps;
        health -= dps;
        if (health <=0)
        {
            NetworkManager.singleton.client.Disconnect();
        }
    }

    public abstract void Movement();
}
