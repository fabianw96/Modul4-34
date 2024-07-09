using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PrefabSpawner : EditorWindow
{
    private List<GameObject> _prefabChoiceLst;
    private List<GameObject> _spawnedPrefabs;
    private VisualElement _root;
    private Label _label;
    private DropdownField _prefabDropDown;
    private Toggle _enableButton;
    private Button _clearButton;
    private LayerMask _terrainLayer;
    

    [MenuItem("Window/UI Toolkit/PrefabSpawner")]
    public static void ShowExample()
    {
        PrefabSpawner wnd = GetWindow<PrefabSpawner>();
        wnd.titleContent = new GUIContent("PrefabSpawner");
    }

    public void CreateGUI()
    {
        _terrainLayer = LayerMask.GetMask("Terrain");
        
        SceneView.duringSceneGui += OnClick;

        _root = rootVisualElement;
        _label = new Label("PrefabSpawner");
        DrawReorderableList(_prefabChoiceLst, rootVisualElement);
        _enableButton = new Toggle("Enable spawning");
        _clearButton = new Button()
        {
            text = "Clear spawned prefabs!",
        };
        _clearButton.RegisterCallback<ClickEvent>(ClearSpawnedObjects);
        
        _root.Add(_label);
        _root.Add(_enableButton);
        _root.Add(_clearButton);
    }

    private void ClearSpawnedObjects(ClickEvent evt)
    {
        foreach (var obj in _spawnedPrefabs)
        {
            DestroyImmediate(obj);
        }
        
        _spawnedPrefabs.Clear();
    }

    private void OnClick(SceneView obj)
    {
        Event e = Event.current;
        
        if (_enableButton.value && e.type == EventType.MouseDrag)
        {
            GUIUtility.hotControl = 1;
            Vector3 mousePos = e.mousePosition;
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = obj.camera.pixelHeight - mousePos.y * pixelsPerPoint;
            mousePos.x *= pixelsPerPoint;

            Ray ray = obj.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _terrainLayer))
            {
                int randomChoice = Random.Range(0, _prefabChoiceLst.Count);
                GameObject o = Instantiate(_prefabChoiceLst[randomChoice], hit.point, quaternion.RotateY(Random.Range(0f,180f)));
                _spawnedPrefabs.Add(o);
            }
        }
    }

    private void DrawReorderableList<T>(List<T> sourceList, VisualElement rootVisElement, bool allowSceneObjects = true) where T : UnityEngine.Object
    {
        var list = new ListView(sourceList)
        {
            virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            showFoldoutHeader = true,
            headerTitle = "Prefabs",
            showAddRemoveFooter = true,
            reorderable = true,
            reorderMode = ListViewReorderMode.Animated,
            makeItem = () => new ObjectField
            {
                objectType = typeof(T),
                allowSceneObjects = allowSceneObjects
            },
            bindItem = (element, i) =>
            {
                ((ObjectField)element).value = sourceList[i];
                ((ObjectField)element).RegisterValueChangedCallback((value) =>
                {
                    sourceList[i] = (T)value.newValue;
                });
            }
        };
        
        rootVisElement.Add(list);
    }
}
