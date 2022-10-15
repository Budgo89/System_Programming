using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
//[CustomEditor(typeof(MeshModify))]
public class MeshModifyerEditor : Editor
{
    enum SupportedAspects
    {
        Aspect4by3 = 1,
        Aspect5by4 = 2,
        Aspect16by10 = 3,
        Aspect16by9 = 4
    };

    Camera _cam = null;
    RenderTexture _rt;
    Texture2D _tex2d;
    Scene _scene;

    // preview variables
    SupportedAspects _aspectChoiceIdx = SupportedAspects.Aspect16by10;
    float _curAspect;
    // world space (orthographicSize)
    float _worldScreenHeight = 5;
    int _renderTextureHeight = 1080;

    float ToFloat(SupportedAspects aspects)
    {
        switch (aspects)
        {
            case SupportedAspects.Aspect16by10:
                return 16 / 10f;
            case SupportedAspects.Aspect16by9:
                return 16 / 9f;
            case SupportedAspects.Aspect4by3:
                return 4 / 3f;
            case SupportedAspects.Aspect5by4:
                return 5 / 4f;
            default:
                throw new ArgumentException();
        }
    }

    void DrawRefScene()
    {
        _rt = new RenderTexture(Mathf.RoundToInt(_curAspect * _renderTextureHeight), _renderTextureHeight, 16);
        _cam.targetTexture = _rt;
        _cam.Render();
        _tex2d = new Texture2D(_rt.width, _rt.height, TextureFormat.RGBA32, false);
        _tex2d.Apply(false);
        Graphics.CopyTexture(_rt, _tex2d);
    }

    Vector2 GetGUIPreviewSize()
    {
        Vector2 camSizeWorld = new Vector2(_worldScreenHeight * _curAspect, _worldScreenHeight);
        float scaleFactor = EditorGUIUtility.currentViewWidth / camSizeWorld.x;
        return new Vector2(EditorGUIUtility.currentViewWidth, scaleFactor * camSizeWorld.y);
    }

    #region Init
    void OnEnable()
    {
        void OpenSceneDelay()
        {
            EditorApplication.delayCall -= OpenSceneDelay;
            DrawRefScene();
        }

        _aspectChoiceIdx = SupportedAspects.Aspect16by10;

        _scene = SceneManager.GetActiveScene();

        // PrefabUtility.LoadPrefabContentsIntoPreviewScene("Assets/Prefabs/Demo/DemoBkg.prefab", _scene);
        _cam = _scene.GetRootGameObjects()[0].GetComponentInChildren<Camera>();
        _cam.cameraType = CameraType.Preview;
        _cam.scene = _scene;
        _curAspect = ToFloat(_aspectChoiceIdx);
        _cam.aspect = _curAspect;
        _cam.orthographicSize = _worldScreenHeight;

        EditorApplication.delayCall += OpenSceneDelay;
    }

    void OnDisable()
    {
        EditorSceneManager.ClosePreviewScene(_scene);
    }
    #endregion

    void OnCamSettingChange()
    {
        _curAspect = ToFloat(_aspectChoiceIdx);
        _cam.aspect = _curAspect;
        _cam.orthographicSize = _worldScreenHeight;
        DrawRefScene();
    }

    // GUI states
    class GUIControlStates
    {
        public bool foldout = false;
    };
    GUIControlStates _guiStates = new GUIControlStates();
    public override void OnInspectorGUI()
    {
        // draw serializedObject fields
        // ....


        // display options
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            _aspectChoiceIdx = (SupportedAspects)EditorGUILayout.EnumPopup("label", (Enum)_aspectChoiceIdx);
            if (scope.changed)
            {
                OnCamSettingChange();
            }
        }
        _guiStates.foldout = EditorGUILayout.Foldout(_guiStates.foldout, "label", true);
        if (_guiStates.foldout)
        {
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                _worldScreenHeight = EditorGUILayout.FloatField("label", _worldScreenHeight);
                _renderTextureHeight = EditorGUILayout.IntField("label", _renderTextureHeight);

                if (scope.changed)
                {
                    OnCamSettingChange();
                }
            }
        }

        if (_tex2d != null)
        {
            Vector2 sz = GetGUIPreviewSize();
            Rect r = EditorGUILayout.GetControlRect(false,
                GUILayout.Height(sz.y),
                GUILayout.ExpandHeight(false));
            EditorGUI.DrawPreviewTexture(r, _tex2d);
        }
    }

}
