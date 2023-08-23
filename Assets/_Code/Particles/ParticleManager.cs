using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    #region Properties
    public static ParticleManager Instance;



    [SerializeField]
    public List<Particle> pooledSteamMonsterParticles = new List<Particle>();

    string steamPath = "Particles/Steam Monster Particles";
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

        SetupParticles();
    }

    private void SetupParticles()
    {
        // Spawn More
        Particle temp;
        int amountToPool = 10;
        for (int i = 0; i < amountToPool; i++)
        {
            temp = (Instantiate(Resources.Load(steamPath)) as GameObject).GetComponent<Particle>();
            temp.Hide();
            pooledSteamMonsterParticles.Add(temp);
            temp.gameObject.transform.parent = transform;
        }
    }
    #endregion

    #region HANDLE PARTICLES
    public void GetParticles(Vector3 worldPos, Particle.Type type)
    {
        switch (type)
        {
            case Particle.Type.steamMonster:
                //pooledSteamMonsterParticles[0].transform.position = worldPos;
                GetPooledParticle(pooledSteamMonsterParticles, worldPos);
                break;
        }
    }

    public void HideAllParticles()
    {
        foreach (var p in pooledSteamMonsterParticles)
        {
            //HideParticles(p);
            p.Hide();
        }
    }

    public void HideParticles(GameObject particles)
    {
        // Move them to a location far away
        particles.transform.position = Vector3.up * 10f;
    }
    #endregion

    private void GetPooledParticle(List<Particle> particleList, Vector3 worldPos)
    {
        for (int i = 0; i < particleList.Count; i++)
        {
            // Check that they're not in use
            if (!particleList[i].inUse)
            {
                // Use them
                particleList[i].UseAt(worldPos);
                //Debug.Log("Particle nro " + i + " in use at " + worldPos.ToString());
                break;
            }
        }
    }
}
