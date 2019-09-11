using System.Collections;
using System.Collections.Generic;
using LibLabSystem;
using UnityEngine;

public class DespawnParticle : MonoBehaviour
{
    Coroutine coroutine;
    ParticleSystem emitter;

    void OnSpawned()
    {
        coroutine = StartCoroutine(DOStart());
    }

    void OnDespawned()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        emitter.Clear(true);
    }

    IEnumerator DOStart()
    {
        if (emitter == null)
            emitter = GetComponent<ParticleSystem>();

        yield return new WaitForSeconds(emitter.main.startDelayMultiplier);

        while (emitter.IsAlive(true))
        {
            if (!emitter.gameObject.activeInHierarchy)
            {
                emitter.Clear(true);
                yield break;
            }

            yield return null;
        }

        LLPoolManager.instance.Despawn(transform);
    }
}