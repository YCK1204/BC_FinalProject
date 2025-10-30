#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class AnimationKeyframeRespacer : EditorWindow
{
    private AnimationClip animationClip;
    private int frameInterval = 2;
    private int frameRate = 60;
    private bool previewMode = true;

    [MenuItem("Tools/Animation Keyframe Respacer")]
    public static void ShowWindow()
    {
        GetWindow<AnimationKeyframeRespacer>("Keyframe Respacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation Keyframe Respacer", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // 애니메이션 클립 선택
        animationClip = (AnimationClip)EditorGUILayout.ObjectField(
            "Animation Clip",
            animationClip,
            typeof(AnimationClip),
            false
        );

        EditorGUILayout.Space(5);

        // 설정
        GUILayout.Label("간격 설정", EditorStyles.boldLabel);
        frameInterval = EditorGUILayout.IntField("프레임 간격", frameInterval);
        frameRate = EditorGUILayout.IntField("프레임레이트 (FPS)", frameRate);

        EditorGUILayout.Space(5);

        // 정보 표시
        if (animationClip != null)
        {
            var spriteCount = CountSprites(animationClip);
            float newDuration = (spriteCount * frameInterval) / (float)frameRate;

            EditorGUILayout.HelpBox(
                $"현재 스프라이트 개수: {spriteCount}개\n" +
                $"현재 애니메이션 길이: {animationClip.length:F3}초\n" +
                $"새로운 애니메이션 길이: {newDuration:F3}초",
                MessageType.Info
            );
        }

        EditorGUILayout.Space(10);

        // 버튼
        GUI.enabled = animationClip != null;
        if (GUILayout.Button("키프레임 간격 재조정", GUILayout.Height(30)))
        {
            RespaceKeyframes();
        }
        GUI.enabled = true;

        EditorGUILayout.Space(5);

        EditorGUILayout.HelpBox(
            "주의: 이 작업은 애니메이션 클립을 직접 수정합니다.\n" +
            "백업 후 진행하시거나 버전 관리를 사용하세요.",
            MessageType.Warning
        );
    }

    private int CountSprites(AnimationClip clip)
    {
        EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

        foreach (var binding in bindings)
        {
            if (binding.type == typeof(SpriteRenderer) && binding.propertyName == "m_Sprite")
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                return keyframes.Length;
            }
        }

        return 0;
    }

    private void RespaceKeyframes()
    {
        if (animationClip == null)
        {
            EditorUtility.DisplayDialog("오류", "애니메이션 클립을 선택해주세요.", "확인");
            return;
        }

        // 애니메이션 클립의 경로 가져오기
        string path = AssetDatabase.GetAssetPath(animationClip);

        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("오류", "애니메이션 클립이 저장된 에셋이 아닙니다.", "확인");
            return;
        }

        // 확인 다이얼로그
        if (!EditorUtility.DisplayDialog("확인",
            $"'{animationClip.name}' 애니메이션의 키프레임 간격을 {frameInterval} 프레임으로 재조정하시겠습니까?\n\n" +
            "이 작업은 되돌릴 수 없습니다.",
            "재조정", "취소"))
        {
            return;
        }

        // 애니메이션 클립 복사본 생성 (안전)
        Undo.RecordObject(animationClip, "Respace Animation Keyframes");

        // 모든 SpriteRenderer 바인딩 찾기
        EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(animationClip);

        foreach (var binding in bindings)
        {
            if (binding.type == typeof(SpriteRenderer) && binding.propertyName == "m_Sprite")
            {
                // 기존 키프레임 가져오기
                ObjectReferenceKeyframe[] oldKeyframes = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);

                // 새로운 키프레임 생성
                ObjectReferenceKeyframe[] newKeyframes = new ObjectReferenceKeyframe[oldKeyframes.Length];

                for (int i = 0; i < oldKeyframes.Length; i++)
                {
                    float newTime = (i * frameInterval) / (float)frameRate;
                    newKeyframes[i] = new ObjectReferenceKeyframe
                    {
                        time = newTime,
                        value = oldKeyframes[i].value
                    };
                }

                // 새로운 키프레임 적용
                AnimationUtility.SetObjectReferenceCurve(animationClip, binding, newKeyframes);
            }
        }

        // 애니메이션 클립 저장
        EditorUtility.SetDirty(animationClip);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("완료",
            $"키프레임 간격이 {frameInterval} 프레임으로 재조정되었습니다.",
            "확인");
    }
}
#endif