using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Properties
    static public EnemyManager Instance;
    EnemyMover mover;

    int steamAmount = 1;

    public List<Enemy> enemies = new List<Enemy>();
    public List<EnemyManifest> manifestations = new List<EnemyManifest>();

    string enemyPath = "Enemies/Enemy Manifest";

    #endregion

    #region Setup
    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        mover = GetComponent<EnemyMover>();
    }
    #endregion

    #region SPAWNING
    public void SpawnEnemies()
    {
        List<Room> rooms = RoomManager.Instance.roomsList;

        Enemy steam = new Enemy(Enemy.Type.steam);
        enemies.Add(steam);

        // Assign to room
        rooms[0].monster = steam;
        steam.currentRoom = rooms[0];
        steam.lastRoom = rooms[0];
    }

    #endregion

    #region MANIFESTATION
    public void ManifestEnemy(Room r, Vector3 worldPos)
    {
        if (r.monster == null) { return; }

        // Spawn Enemy Manifest
        EnemyManifest manifest = (Instantiate(Resources.Load(enemyPath)) as GameObject).GetComponent<EnemyManifest>();
        PlaceManifestation(manifest, worldPos);
        manifest.gameObject.transform.parent = transform;
        manifestations.Add(manifest);
    }

    private void PlaceManifestation(EnemyManifest manifest, Vector3 pos)
    {
        manifest.gameObject.transform.position = pos;
    }

    public void ClearManifestations()
    {
        int amount = manifestations.Count;
        //int amount = transform.childCount;
        Debug.Log("Manifestations amount: " + amount);
        for (int i = 0; i < amount; i++)
        {
            // Destroy for now
            Destroy(manifestations[0].gameObject);
            manifestations.RemoveAt(0);
            //Destroy(transform.GetChild(0).gameObject);
        }
        //manifestations.Clear();
    }
    #endregion

    #region MOVING
    public void MoveEnemies()
    {
        mover.MoveEnemies(enemies);
    }
    #endregion
}

