using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private int scoreForHit = 5;
    [SerializeField]
    private int scoreForKill = 100;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    [Header("Sounds:")]
    [SerializeField]
    private AudioSource shotSound;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();


        if (currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    /// <summary>
    /// Is called on the server when a player shoots
    /// </summary>
    [Command]
    private void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    /// <summary>
    /// Is called on all clients when we need to do a shoot effect
    /// </summary>
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    /// <summary>
    /// Is called on the server when we hit comething
    /// Takes in the hit point and normal of the surface
    /// </summary>
    [Command]
    private void CmdOnHit(Vector3 _pos, Vector3 _noraml)
    {
        RpcDoHitEffect(_pos, _noraml);
    }

    /// <summary>
    /// Is called on all clients
    /// </summary>
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);

        shotSound.Play();
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
        { 
            return;
        }

        // Take away bullets
        Player _player = GetComponent<Player>();

        if (_player.GetBullets() <= 0)
        {
            return;
        }

        _player.TakeAwayBullets(1);

        // We are shooting, call the OnShoot method on the server 
        CmdOnShoot();

        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, _player.name, currentWeapon.damage);
            }

            // We hit something, call the OnHit method on the server
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    [Command]
    private void CmdPlayerShot(string _targetPlayerID, string _haterPlayerID, int _damage)
    {
        Debug.Log($"{_targetPlayerID} take damage from {_haterPlayerID}.");

        Player _targetPlayer = GameManager.GetPlayer(_targetPlayerID);
        _targetPlayer.RpcTakeDamage(_damage);

        // Reward for a hit/kill
        Player _haterPlayer = GameManager.GetPlayer(_haterPlayerID);
        _haterPlayer.AddScore(scoreForHit);

        if (_targetPlayer.GetHealth() - _damage <= 0)
        {
            _haterPlayer.AddScore(scoreForKill);
        }
    }
}
