using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LLUtils
{
    public static void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public static AsyncOperation ReloadCurrentScene()
    {
        DOTween.KillAll();
        if (LLPoolManager.instance != null)
            LLPoolManager.instance.DespawnAll();
        return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static AsyncOperation LoadNextScene()
    {
        DOTween.KillAll();
        if (LLPoolManager.instance != null)
            LLPoolManager.instance.DespawnAll();
        return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public static AsyncOperation LoadPreviousScene()
    {
        DOTween.KillAll();
        if (LLPoolManager.instance != null)
            LLPoolManager.instance.DespawnAll();
        return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1, LoadSceneMode.Single);
    }

    private static Vector3 _pos;

    public static void X(this Transform transform, float x)
    {
        _pos = transform.position;
        _pos.x = x;
        transform.position = _pos;
    }

    public static void Y(this Transform transform, float y)
    {
        _pos = transform.position;
        _pos.y = y;
        transform.position = _pos;
    }

    public static void Z(this Transform transform, float z)
    {
        _pos = transform.position;
        _pos.z = z;
        transform.position = _pos;
    }

    public static void LocalX(this Transform transform, float x)
    {
        _pos = transform.localPosition;
        _pos.x = x;
        transform.localPosition = _pos;
    }

    public static void LocalY(this Transform transform, float y)
    {
        _pos = transform.localPosition;
        _pos.y = y;
        transform.localPosition = _pos;
    }

    public static void LocalZ(this Transform transform, float z)
    {
        _pos = transform.localPosition;
        _pos.z = z;
        transform.localPosition = _pos;
    }

    public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }

    public static Transform LastChild(this Transform t)
    {
        return t.GetChild(t.childCount - 1);
    }

    public static void DetachAllChild(this Transform t, Transform newParent = null, bool worldPositionStays = true)
    {
        for (int i = t.childCount - 1; i > -1; --i)
        {
            t.GetChild(i).SetParent(newParent, worldPositionStays);
        }
    }

    public static float X(this Transform t)
    {
        return t.position.x;
    }

    public static float Y(this Transform t)
    {
        return t.position.y;
    }

    public static float Z(this Transform t)
    {
        return t.position.z;
    }

    public static float LocalX(this Transform t)
    {
        return t.localPosition.x;
    }

    public static float LocalY(this Transform t)
    {
        return t.localPosition.y;
    }

    public static float LocalZ(this Transform t)
    {
        return t.localPosition.z;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void TransformShake(this Transform t, float timeShake = 1f, float force = 0.05f)
    {
        LLSystem.instance.StartCoroutine(COTransformShake(timeShake, force, t));
    }

    private static Vector3 camPos;
    private static IEnumerator COTransformShake(float time, float force, Transform t)
    {
        camPos = t.position;
        while (time > 0)
        {
            t.position = camPos + UnityEngine.Random.insideUnitSphere * force;
            time -= Time.deltaTime;
            yield return null;
        }
        t.position = camPos;
    }

    public static void ScreenShake(Camera cameraToShake = null, float timeShake = 1f, float force = 0.05f)
    {
        if (cameraToShake == null)
            cameraToShake = Camera.main;

        cameraToShake.transform.TransformShake(timeShake, force);
    }
}