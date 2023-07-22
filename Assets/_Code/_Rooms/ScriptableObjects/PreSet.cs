using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/StartingSet")]
public class PreSet : ScriptableObject
{
    #region Properties
    public List<Layout> instructions = new List<Layout>();
    #endregion

    [System.Serializable]
    public class Layout
    {
        public string name;
        public TypeRoom type;
        public Orientation orientation;

        public string north_Neighbour_name = "";
        public string west_Neighbour_name = "";
        public string east_Neighbour_name = "";
        public string south_Neighbour_name = "";

        public List<Item> items = new List<Item>();
        public bool hasItems {  get { return (items.Count != 0); } }
    }

}
