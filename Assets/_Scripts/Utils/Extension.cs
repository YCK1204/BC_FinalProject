using System.Collections.Generic;
using UnityEngine;

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
        return (to.position - from.position).normalized;
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
}
