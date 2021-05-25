using Assets.Scripts.Bonuses;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead { get => _isDead; protected set => _isDead = value; }

    [SerializeField]
    private int maxArmor = 50;
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private int startBullets = 30;

    [SyncVar]
    private int currentArmor;
    [SyncVar]
    private int currentHealth = 100;
    [SyncVar]
    private int currentBullets;
    [SyncVar]
    private int currentScore;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    [Header("Sounds:")]
    [SerializeField]
    private AudioSource ammoSound;
    [SerializeField]
    private AudioSource armorSound;
    [SerializeField]
    private AudioSource healthSound;
    [SerializeField]
    private AudioSource deathSound;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            // Switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];

            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void AddScore(int _incrementScore)
    {
        currentScore += _incrementScore;
    }


    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(99999);
        }

        if (isDead)
        {
            isDead = false;
            StartCoroutine(Respawn());
        }
    }


    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetBullets()
    {
        return currentBullets;
    }

    public void AddBullets(int _count)
    {
        currentBullets += _count;
    }

    public void TakeAwayBullets(int _count)
    {
        currentBullets -= _count;

        if (currentBullets < 0)
        { 
            currentBullets = 0;
        }
    }

    public void Heal(int _points)
    {
        currentHealth += _points;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void AddArmor(int _points)
    {
        currentArmor += _points;

        if (currentArmor > maxArmor)
        {
            currentArmor = maxArmor;
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetArmor()
    {
        return currentArmor;
    }

    public float GetMaxArmor()
    {
        return maxArmor;
    }

    private void Die()
    {
        // isDead = true;

        // Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        // Disable collider
        Collider _col = GetComponent<Collider>();

        if (_col != null)
        {
            _col.enabled = false;
        }

        // Spawn death effect
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

        // Play audio effect
        deathSound.Play();

        // Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is DEAD!");

        //StartCoroutine(Respawn());

        isDead = true;

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        //Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        //transform.position = _spawnPoint.position;
        //transform.rotation = _spawnPoint.rotation;

        //yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }

    private void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;
        currentArmor = 0;
        currentBullets = startBullets;

        // Enable the Components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        // Enable the GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        // Enable the Collider
        Collider _col = GetComponent<Collider>();

        if (_col != null)
        {
            _col.enabled = true;
        }

        // Create spawn Effect
        GameObject _gfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
    }

    private void OnCollisionEnter(Collision _collision)
    {
        AbstractBonus _bonus = _collision.gameObject.GetComponent<AbstractBonus>();

        if (_bonus != null)
        {
            _bonus.Buff(this);
            PlayAudioBonusEffect(_bonus);
            Destroy(_collision.gameObject);
        }
    }

    [Client]
    private void PlayAudioBonusEffect(AbstractBonus _bonus)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (_bonus is Ammo)
        {
            ammoSound.Play();
        }
        else if (_bonus is Armor)
        {
            armorSound.Play();
        }
        else if (_bonus is Health)
        {
            healthSound.Play();
        }
    }
}
