using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PoolParticle {
    public GameObject particleToPool;
    public int numParticles;
    public bool expandable;
}



public class ObjectPool : MonoBehaviour {

    public static ObjectPool SharedInstance;

    public List<GameObject> pooledParticles;
    public List<PoolParticle> particlesToPool;

    void Awake() {
        SharedInstance = this;
    }

	// Use this for initialization
	void Start () {

        pooledParticles = new List<GameObject>();
         
        foreach (PoolParticle particle in particlesToPool) {
            for (int i = 0; i < particle.numParticles; i++) {
                GameObject part = Instantiate(particle.particleToPool);
                part.SetActive(false);
                pooledParticles.Add(part);
            }
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameObject GetPooledParticle(string tag) {

        for (int i = 0; i < pooledParticles.Count; i++) {
            if (!pooledParticles[i].activeInHierarchy && pooledParticles[i].tag == tag)
                return pooledParticles[i];
        }

        foreach (PoolParticle particle in particlesToPool) {
            if (particle.particleToPool.tag == tag) {
                if (particle.expandable) {
                    GameObject part = Instantiate(particle.particleToPool);
                    part.SetActive(false);
                    pooledParticles.Add(part);
                    return part;
                }
            }
        }

        return null;
    }
}
