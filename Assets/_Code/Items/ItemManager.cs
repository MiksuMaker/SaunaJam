using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    #region Properties
    static public ItemManager Instance;
    ItemSpawner itemSpawner;

    List<(ItemManifest, Room)> manifestationsList = new List<(ItemManifest, Room)>();
    string manifestPath = "Items/ItemManifest";

    Transform itemParent;
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
            Destroy(gameObject);
        }

        itemParent = FindObjectOfType<ItemParent>().gameObject.transform;
        itemSpawner = GetComponent<ItemSpawner>();
    }

    public void SetupSauna(Room saunaRoom, Orientation wallPreference = Orientation.north)
    {
        // Create Sauna
        Item sauna = new Item();
        sauna.type = Item.Type.sauna;

        // Add Sauna to saunaRoom
        AddItem(sauna, saunaRoom, wallPreference);
    }
    #endregion

    #region Spawn Items
    public void SpawnItems(ItemSet set)
    {
        itemSpawner.SpawnItems(set);
    }
    #endregion

    #region MANIFEST ITEMS
    public void ManifestRoomItems(Room r, Vector3 roomCoordinates)
    {
        if (!r.hasItems) { return; }

        foreach (Item i in r.items)
        {
            ManifestItem(i, r, roomCoordinates);
        }
    }

    public void ManifestItem(Item item, Room r, Vector3 roomCoordinates)
    {
        //GameObject itemGO = Instantiate(Resources.Load(manifestPath), itemParent) as GameObject;
        GameObject itemGO = Instantiate(Resources.Load(ItemTypeToPath(item)), itemParent) as GameObject;
        ItemManifest manifest = itemGO.GetComponent<ItemManifest>() as ItemManifest;

        manifestationsList.Add((manifest, r));

        manifest.SetupManifest(item, roomCoordinates, r.attribute);
    }

    public void DeManifestItem(ItemManifest manifest, Room r)
    {
        manifestationsList.Remove((manifest, r));
        Destroy(manifest.go);
    }

    public void ClearManifestations()
    {
        // Clear the list
        manifestationsList.Clear();

        // Destroy all GameObjects
        foreach (Transform c in itemParent.transform)
        {
            Destroy(c.gameObject);
        }
    }

    private string ItemTypeToPath(Item i)
    {
        string path = "Items/";

        switch (i.type)
        {
            case Item.Type.sauna: path += "Sauna"; break;
            case Item.Type.saunaStone: path += "SaunaStone"; break;
            case Item.Type.water: path += "Water"; break;
            case Item.Type.woodLog: path += "WoodLog"; break;
            case Item.Type.writing: path += "Writing"; break;
        }
        return path;
    }
    #endregion

    #region PutItems
    public void DecorateRoom(Room r, List<Item> itemList, bool officialText = false)
    {
        foreach (Item i in itemList)
        {
            //CreateAndAddItem(i.type, r, i.wallOrientation);
            AddItem(i, r, i.wallOrientation);

            if (i.type == Item.Type.writing)
            {
                i.official = true;
            }
        }
    }

    public void AddItem(Item item, Room r, Orientation wall)
    {
        item.wallOrientation = wall;
        r.items.Add(item);
    }

    public void CreateAndAddItem(Item.Type type, Room r, Orientation wall)
    {
        Item item = new Item();
        switch (type)
        {
            case Item.Type.sauna: item.type = Item.Type.sauna; break;
            case Item.Type.saunaStone: item.type = Item.Type.saunaStone; break;
            case Item.Type.water: item.type = Item.Type.water; break;
            case Item.Type.woodLog: item.type = Item.Type.woodLog; break;
            case Item.Type.writing: item.type = Item.Type.writing; break;
        }

        AddItem(item, r, wall);

        if (RoomManager.Instance.currentRoom == r) { ManifestItem(item, r, RoomManager.Instance.desiredLocation); }
    }
    #endregion

    #region Item Handling
    public bool TestForItem(Room r, Orientation facingOrientation)
    {
        if (!r.hasItems) { return false; }

        foreach (var i in r.items)
        {
            if (i.wallOrientation == facingOrientation)
            {
                return true;
            }
        }
        return false;
    }


    public ItemManifest GetItemManifest(Room r, Orientation facingOrientation)
    {
        foreach (var i in manifestationsList)
        {

            // If it is the same room AND the item is in the wanted orientation
            if (r == i.Item2 && i.Item1.item.wallOrientation == facingOrientation)
            {
                return i.Item1;
            }
        }
        return null;
    }

    public void RemoveItemManifest(ItemManifest manifest)
    {
        Item.Type type = manifest.item.type;

        // Go through all the manifestations, Remove from room
        int pairAmount = manifestationsList.Count;
        for (int i = 0; i < pairAmount; i++)
        {
            if (manifestationsList[i].Item1 == manifest)
            {
                // Remove Item from room
                manifestationsList[i].Item2.items.Remove(manifestationsList[i].Item1.item);

                // Destroy Item Manifestation
                DeManifestItem(manifest, manifestationsList[i].Item2);

                // Recalculate pairAmount
                pairAmount--;
            }
        }

        bool moveEnemies = true;
        if (type == Item.Type.saunaStone)
        {
            moveEnemies = false;
        }

        // Update
        RoomManager.Instance.UpdateRooms(moveEnemies);
    }

    public Item GetItem(Room r, Orientation facing)
    {
        foreach (Item i in r.items)
        {
            if (facing == i.wallOrientation)
            {
                return i;
            }
        }
        return null;
    }
    #endregion

    #region Wall picking
    private Orientation PickRandomWall(Room r)
    {
        // Pick a random Wall
        List<int> usedNums = new List<int>();

        Orientation randWall = Orientation.north; // Default

        for (int i = 0; i < 4; i++)
        {
            int newRandNum;
            while (true)
            {
                newRandNum = Random.Range(0, 4);
                if (!usedNums.Contains(newRandNum))
                { break; }
            }

            switch (newRandNum)
            {
                case 0: randWall = Orientation.north; break;
                case 1: randWall = Orientation.west; break;
                case 2: randWall = Orientation.east; break;
                case 3: randWall = Orientation.south; break;
            }

            if (CheckWallValidity(r, randWall))
            { break; }
        }
        return randWall;
    }
    #endregion

    #region Validation
    public bool CheckForWriting(Room r, Orientation wall)
    {
        if (!r.hasItems) { return false; }

        foreach (Item i in r.items)
        {
            if (i.wallOrientation == wall)
            {
                if (i.type == Item.Type.writing)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryGetRandomValidWall(Room r, out Orientation orientation)
    {
        // Shuffle orientations
        Orientation[] orientations = new Orientation[] { Orientation.north, Orientation.west, Orientation.east, Orientation.south };
        int n = orientations.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Orientation nxt = orientations[k];
            orientations[k] = orientations[n];
            orientations[n] = nxt;
        }

        // Try out all walls in random sequence
        foreach (var o in orientations)
        {
            if (CheckValidity(r,o))
            {
                // Valid wall, return orientation
                orientation = o;
                return true;
            }
        }
        // If not found, return false
        orientation = Orientation.north;
        return false;
    }

    public bool CheckValidity(Room r, Orientation wall)
    {
        // Check if both wall is valid AND that it isn't occupied
        if (CheckWallValidity(r, wall) && !CheckIfOccupiedByItem(r, wall))
        {
            return true;
        }
        else
            return false;
    }

    public bool CheckWallValidity(Room r, Orientation wall)
    {
        if (OrientationToWall(r, wall).neighbour == null)
        {
            // Is a wall
            return true;
        }
        else
        {
            // Is NOT a wall
            return false;
        }
    }

    public bool CheckIfOccupiedByItem(Room r, Orientation wall)
    {
        if (!r.hasItems) { return false; }

        foreach (Item i in r.items)
        {
            // Check if they take up the wall
            if (i.wallOrientation == wall)
            {
                // It is occupied
                return true;
            }
        }

        // There's free space
        return false;
    }

    public Item.Type CheckItemType(Room r, Orientation wall)
    {
        foreach (Item i in r.items)
        {
            if (i.wallOrientation == wall)
            {
                // Return type
                return i.type;
            }
        }
        return Item.Type.woodLog; // Default
    }

    private Connection OrientationToWall(Room r, Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.north: return r.north;
            case Orientation.west: return r.west;
            case Orientation.east: return r.east;
            case Orientation.south: return r.south;
            default: return r.east;
        }
    }
    #endregion
}
