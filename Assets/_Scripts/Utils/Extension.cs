using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
#endif
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public static class Extension
{
    #region FindChild
    /// <summary>
    /// 특정 Transform의 자식들 중에서 특정 타입의 컴포넌트를 가진 첫 번째 자식을 찾습니다.
    /// recursive가 true이면 모든 자식들을 재귀적으로 탐색합니다.
    /// name이 null이 아니면 해당 이름과 일치하는 자식만 찾습니다.
    /// </summary>
    /// <typeparam name="T">타겟 컴포넌트</typeparam>
    /// <param name="transform">부모 Transform</param>
    /// <param name="recursive">재귀 탐색 여부</param>
    /// <param name="name">타겟 오브젝트 이름(기본값 null)</param>
    /// <returns></returns>
    public static T FindChild<T>(this Transform transform, bool recursive = false, string name = null) where T : Component
    {
        if (recursive == false)
        {
            var childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (name == null || child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        var childs = transform.GetComponentsInChildren<T>();

        foreach (var child in childs)
        {
            if (name == null || child.name == name)
                return child;
        }
        return null;
    }
    /// <summary>
    /// 특정 Transform의 자식들 중에서 특정 타입의 컴포넌트를 가진 모든 자식을 찾습니다.
    /// recursive가 true이면 모든 자식들을 재귀적으로 탐색합니다.
    /// </summary>
    /// <typeparam name="T">타겟 컴포넌트</typeparam>
    /// <param name="transform">부모 Transform</param>
    /// <param name="recursive">재귀 탐색 여부</param>
    /// <returns></returns>
    public static T[] FindChilds<T>(this Transform transform, bool recursive = false) where T : Component
    {
        if (recursive == false)
        {
            List<T> results = new List<T>();
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                T component = child.GetComponent<T>();
                if (component != null)
                    results.Add(component);
            }
            return results.ToArray();
        }

        return transform.GetComponentsInChildren<T>();
    }
    #endregion
    /// <summary>
    /// gameObject에 특정 타입의 컴포넌트가 존재하면 해당 컴포넌트를 반환하고, 
    /// 존재하지 않으면 새로 추가한 후 반환합니다.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }
    /// <summary>
    /// gameObject에 특정 타입의 컴포넌트가 존재하는지 확인합니다.
    /// </summary>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }
    /// <summary>
    /// gameObject에서 특정 타입의 컴포넌트를 찾아 제거합니다.
    /// </summary>
    public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
            Object.DestroyImmediate(component);
    }

    /// <summary>
    /// 두 Transform 간의 거리를 계산합니다.
    /// </summary>
    public static float DistanceTo(this Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position);
    }
    /// <summary>
    /// 두 GameObject 간의 거리를 계산합니다.
    /// </summary>
    public static float DistanceTo(this GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }
    /// <summary>
    /// 두 Transform 간의 방향 벡터를 계산합니다.
    /// </summary>
    public static Vector3 DirectionTo(this Transform from, Transform to)
    {
        return from.gameObject.DirectionTo(to.gameObject);
    }
    /// <summary>
    /// 두 GameObject 간의 방향 벡터를 계산합니다.
    /// </summary>
    public static Vector3 DirectionTo(this GameObject from, GameObject to)
    {
        return (to.transform.position - from.transform.position).normalized;
    }

    // 위치 관련
    /// <summary>
    /// 월드 좌표계에서 Transform의 x 위치를 설정합니다.
    /// </summary>
    public static void SetX(this Transform transform, float x)
    {
        var pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }
    /// <summary>
    /// 월드 좌표계에서 Transform의 y 위치를 설정합니다.
    /// </summary>
    public static void SetY(this Transform transform, float y)
    {
        var pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
    /// <summary>
    /// 월드 좌표계에서 Transform의 z 위치를 설정합니다.
    /// </summary>
    public static void SetZ(this Transform transform, float z)
    {
        var pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }
    /// <summary>
    /// 로컬 좌표계에서 Transform의 x 위치를 설정합니다.
    /// </summary>
    public static void SetLocalX(this Transform transform, float x)
    {
        var pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }
    /// <summary>
    /// 로컬 좌표계에서 Transform의 y 위치를 설정합니다.
    /// </summary>
    public static void SetLocalY(this Transform transform, float y)
    {
        var pos = transform.localPosition;
        pos.y = y;
        transform.localPosition = pos;
    }
    /// <summary>
    /// 로컬 좌표계에서 Transform의 z 위치를 설정합니다.
    /// </summary>
    public static void SetLocalZ(this Transform transform, float z)
    {
        var pos = transform.localPosition;
        pos.z = z;
        transform.localPosition = pos;
    }
    public static IEnumerator LateStart(Action callback)
    {
        yield return null;
        callback?.Invoke();
    }
    /// <summary>
    /// 비동기로 url에 있는 이미지를 불러와 Texture2D로 저장합니다.
    /// </summary>
    /// <param name="url">이미지 url</param>
    /// <param name="callback">이미지를 저장할 콜백함수</param>
    /// <returns></returns>
    public static IEnumerator LoadTextureByURL(string url, Action<Texture2D> callback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            callback.Invoke(((DownloadHandlerTexture)www.downloadHandler).texture);
        }
    }
    public static async Awaitable<Texture2D> LoadTextureByURLAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError($"[TextureLoader] 유효하지 않은 URL");
            return null;
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[TextureLoader] {url}에서 텍스처 로드 실패: {www.error}");
                return null;
            }

            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            if (texture == null)
            {
                Debug.LogError($"[TextureLoader] 텍스처 다운로드 성공했으나 texture가 null");
                return null;
            }

            return texture;
        }
    }
    /// <summary>
    /// Texture2D를 Sprite로 변환합니다.
    /// </summary>
    /// <param name="texture">변환할 텍스처</param>
    public static Sprite ToSprite(this Texture2D texture)
    {
        var newTexture = texture.RemoveNoise();
        return Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
    }
    /// <summary>
    /// 흐려진 텍스처 이미지 해상도를 바로 잡아줍니다.
    /// </summary>
    /// <param name="texture">변환될 텍스처</param>
    /// <returns></returns>
    public static Texture2D RemoveNoise(this Texture2D texture)
    {
        var newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();
        newTexture.filterMode = FilterMode.Point;
        newTexture.anisoLevel = 0;
        return newTexture;
    }
#if UNITY_EDITOR
    public static T LoadWithAddresssableByGroup<T>(string sourceName, string groupName) where T : UnityEngine.Object
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        foreach (var group in settings.groups)
        {
            if (group.name != groupName)
                continue;
            foreach (var entry in group.entries)
            {
                if (entry.address == sourceName)
                {
                    string path = AssetDatabase.GUIDToAssetPath(entry.guid);
                    return (AssetDatabase.LoadAssetAtPath<T>(path));
                }
            }
        }
        return null;
    }
#endif
    public static T FindNearestObject<T>(this Transform from, float range, LayerMask layer) where T : Component
    {
        return from.position.FindNearestObject<T>(range, layer);
    }
    public static T FindNearestObject<T>(this Vector3 from, float range, LayerMask layer) where T : Component
    {
        var objects = Physics2D.OverlapCircleAll(from, range, layer);
        T nearest = null;
        float minDist = range;
        foreach (var obj in objects)
        {
            float dist = Vector3.Distance(from, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = obj.GetComponent<T>();
            }
        }
        return nearest;
    }
    public static T FindNearestObject<T>(this Transform from, float range, string name) where T : Component
    {
        return from.FindNearestObject<T>(range, LayerMask.NameToLayer(name));
    }
    public static T FindNearestObject<T>(this Vector3 from, float range, string name) where T : Component
    {
        return from.FindNearestObject<T>(range, LayerMask.NameToLayer(name));
    }
}
