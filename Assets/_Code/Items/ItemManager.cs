using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    #region Properties
    static public ItemManager Instance;

    List<ItemManifest> manifestationsList = new List<ItemManifest>();
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

    #region MANIFEST ITEMS
    public void ManifestRoomItems(Room r, Vector3 roomCoordinates)
    {
        if (!r.hasItems) { return; }

        foreach (Item i in r.items)
        {
            ManifestItem(i, roomCoordinates);
        }
    }

    public void ManifestItem(Item item, Vector3 roomCoordinates)
    {
        //GameObject itemGO = Instantiate(Resources.Load(manifestPath), itemParent) as GameObject;
        GameObject itemGO = Instantiate(Resources.Load(ItemTypeToPath(item)), itemParent) as GameObject;
        ItemManifest manifest = itemGO.GetComponent<ItemManifest>() as ItemManifest;

        manifestationsList.Add(manifest);

        manifest.SetupManifest(roomCoordinates);
    }

    public void DeManifestItem(ItemManifest manifest)
    {
        manifestationsList.Remove(manifest);
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
    public void AddItem(Item item, Room r, Orientation wall)
    {
        item.wallOrientation = wall;
        r.items.Add(item);
    }

    public void CreateAndAddItem(Item.Type type, Room r, Orientation wall)
    {
        
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
            while(true)
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
    private bool CheckValidity(Room r, Orientation wall)
    {
        // Check if both wall is valid AND that it isn't occupied
        if (CheckWallValidity(r, wall) && CheckIfOccupied(r, wall))
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

    public bool CheckIfOccupied(Room r, Orientation wall)
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
