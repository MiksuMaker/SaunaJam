using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
	[SerializeField]
	public PreSet preset;
	[SerializeField]
	public GenerationPreference preference;
	[SerializeField]
	public int steamSpawnAmount = 0;
	[SerializeField]
	public int gnomeSpawnAmount = 0;
	//[SerializeField]
	// Start Dialogue here
	// Reaction Dialogues object
}
