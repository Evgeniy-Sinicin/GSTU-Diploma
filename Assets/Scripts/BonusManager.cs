using Assets.Scripts.Bonuses;
using Assets.Scripts.Factories;
using Assets.Scripts.Interfaces;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : NetworkBehaviour
{
    private bool isOccupied;

    [SerializeField]
    private int rate = 10;

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<AbstractBonus> bonuses;

    [SyncVar]
    private int iteration = 0;

    private List<IBonusFactory> factories = new List<IBonusFactory>()
    {
        new HealthFactory(),
        new ArmorFactory(),
        new AmmoFactory()
    };

    private void Start()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        while (true)
        {
            if (isServer)
            RpcSpawnBonus();

            yield return new WaitForSeconds(rate);
        }
    }

    //[Client]
    //private void SpawnBonus()
    //{
    //    RpcSpawnBonus();
    //}

    //[Command]
    //private void CmdSapwnBonus()
    //{
    //    RpcSpawnBonus();
    //}

    [ClientRpc]
    private void RpcSpawnBonus()
    {
        Transform _spawnPoint = spawnPoints[iteration % spawnPoints.Count];

        GameObject _bonus = Instantiate(bonuses[iteration % bonuses.Count], _spawnPoint.position, _spawnPoint.rotation).gameObject;
        Destroy(_bonus, rate);

        isOccupied = true;
        iteration++;

        Debug.Log("Bonus spawned.");
    }
}
