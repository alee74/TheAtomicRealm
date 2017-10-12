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

    public GameObject GetPooledParticle(string tag) {

        // Loop through particles in pool and return first that is of desired type and inactive
        for (int i = 0; i < pooledParticles.Count; i++) {
            if (!pooledParticles[i].activeInHierarchy && pooledParticles[i].tag == tag)
                return pooledParticles[i];
        }

        // Create and return new particle of desired type if none available and pool should expand for particle type
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

    public bool RemoveParticleFromPool(GameObject particle) {

        if (pooledParticles.Contains(particle)) {

            pooledParticles.Remove(particle);
            return true;

        }

        return false;
    }

    public bool PutParticleInPool(GameObject particle) {

        foreach (PoolParticle kind in particlesToPool) {

            if (kind.particleToPool.tag == particle.tag) {

                pooledParticles.Add(particle);
                return true;

            }

        }

        return false;
    }

    public bool HasParticleOfType(string tagg) {

        foreach (GameObject particle in pooledParticles) {

            if (particle.gameObject.tag == tagg)
                return true;
        }

        return false;
    }

    void OnDestroy() {

        foreach (GameObject particle in pooledParticles)
            Destroy(particle);

    }

}
