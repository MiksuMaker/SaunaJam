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
	[Header("Monsters")]
	[SerializeField]
	public int steamSpawnAmount = 0;
	[SerializeField]
	public int gnomeSpawnAmount = 0;
	[Space]
	[SerializeField]
	public ItemSet itemSet;
	[SerializeField]
	public int requiredLogs = 1;

	//[SerializeField]
	// Start Dialogue here
	// Reaction Dialogues object
}
