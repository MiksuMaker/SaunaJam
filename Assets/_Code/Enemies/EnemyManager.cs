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

        // Spawn
        SpawnEnemy(Enemy.Type.steam, rooms[1]);
        SpawnEnemy(Enemy.Type.steam, rooms[2]);
        SpawnEnemy(Enemy.Type.steam, rooms[3]);
        //SpawnEnemy(Enemy.Type.gnome, 1, rooms[2]);
    }

    private void SpawnEnemy(Enemy.Type type, Room spawnRoom)
    {
        // Create
        Enemy monster = new Enemy(type);
        monster.orientation = Orientation.north;
        enemies.Add(monster);

        // Assign to room
        spawnRoom.monster = monster;
        monster.currentRoom = spawnRoom;
        monster.lastRoom = spawnRoom;
    }
    #endregion

    #region MANIFESTATION
    public void ManifestEnemy(Room r, Vector3 worldPos)
    {
        if (r.monster == null) { return; }

        // Spawn Enemy Manifest
        EnemyManifest manifest = (Instantiate(Resources.Load(enemyPath)) as GameObject).GetComponent<EnemyManifest>();
        PlaceManifestation(manifest, worldPos);

        // Graphics
        manifest.AlterGnomeGraphics((r.monster.type == Enemy.Type.gnome));

        // Do type specific move
        if (r.monster.type == Enemy.Type.steam)
        { ParticleManager.Instance.GetParticles(worldPos, Particle.Type.steamMonster); }
        else
        { manifest.TurnManifest(r.monster.orientation); }

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
        for (int i = 0; i < amount; i++)
        {
            // Destroy for now
            Destroy(manifestations[0].gameObject);
            manifestations.RemoveAt(0);
        }
    }
    #endregion

    #region MOVING
    public void MoveEnemies()
    {
        mover.UpdateEnemies(enemies);
    }
    #endregion

    #region Detection
    public bool CheckIfRoomHasEnemies(Room r)
    {
        if (r.monster == null)
        {
            return false;
        }
        else
        {
            // Inform the Monster /etc.
            InformMonsterOfPlayer(r.monster);

            // Return true
            return true;
        }
    }

    public void InformMonsterOfPlayer(Enemy e)
    {
        // Do what you want to do here
        mover.StumbleIntoEnemy(e);
    }
    #endregion

    #region INTERACTION
    public void GetRidOfSteam()
    {
        bool needToUpdateEnemies = false;
        foreach (var e in enemies)
        {
            // Check if they're hunting and of type steam
            if (e.mode == Enemy.Mode.hunt && e.type == Enemy.Type.steam)
            {
                // Move them away from Player
                mover.RelocateEnemy(e);
                needToUpdateEnemies = true;
            }
        }

        // Update Enemies
        if (needToUpdateEnemies)
        {
            RoomManager.Instance.UpdateRooms(false);
        }
    }
    #endregion
}

