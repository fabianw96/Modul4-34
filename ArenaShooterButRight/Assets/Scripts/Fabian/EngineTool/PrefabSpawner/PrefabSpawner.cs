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
    //Lists for prefabs
    private List<GameObject> _prefabChoiceLst = new();
    private List<GameObject> _spawnedPrefabs = new();
    
    //Visual UI Elements
    private VisualElement _root;
    private Label _label;
    private DropdownField _prefabDropDown;
    private Toggle _enableButton;
    private Button _clearButton;
    private Slider _radiusSlider;
    
    //Values
    private float _radius;


    [MenuItem("Window/UI Toolkit/PrefabSpawner")]
    public static void ShowExample()
    {
        PrefabSpawner wnd = GetWindow<PrefabSpawner>();
        wnd.titleContent = new GUIContent("PrefabSpawner");
    }

    public void CreateGUI()
    {
        SceneView.duringSceneGui += EvaluateMousePosition;

        _root = rootVisualElement;
        _label = new Label("PrefabSpawner");
        DrawReorderableList(_prefabChoiceLst, rootVisualElement);
        _enableButton = new Toggle("Enable spawning");
        _radiusSlider = new Slider(0f, 10f);
        _clearButton = new Button()
        {
            text = "Clear spawned prefabs!",
        };
        _clearButton.RegisterCallback<ClickEvent>(ClearSpawnedObjects);
        
        _root.Add(_label);
        _root.Add(_enableButton);
        _root.Add(_radiusSlider);
        _root.Add(_clearButton);
    }
    
    private void EvaluateMousePosition(SceneView obj)
    {
        Vector3 mousePos = Event.current.mousePosition;
        float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
        mousePos.y = obj.camera.pixelHeight - mousePos.y * pixelsPerPoint;
        mousePos.x *= pixelsPerPoint;
        
        Ray ray = obj.camera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        _radius = _radiusSlider.value;

        if (Physics.Raycast(ray, out hit))
        {
            DrawCircle(hit);
            DrawPrefabs(hit);
        }
    }

    private void ClearSpawnedObjects(ClickEvent evt)
    {
        foreach (var obj in _spawnedPrefabs)
        {
            DestroyImmediate(obj);
        }
        
        _spawnedPrefabs.Clear();
    }

    private void DrawCircle(RaycastHit hit)
    {

        if (!_enableButton.value)
            return;

        Handles.color = Color.green;
        Handles.CircleHandleCap(0,hit.point,Quaternion.Euler(90f, 0f, 0f), _radius, EventType.Repaint);
    }

    private void DrawPrefabs(RaycastHit hit)
    {
        if (!_enableButton.value || Event.current.type != EventType.MouseDrag) 
            return;
        
        GUIUtility.hotControl = 1;
        int randomChoice = Random.Range(0, _prefabChoiceLst.Count);
        //Positionen speichern und vergleichen
        GameObject o = Instantiate(_prefabChoiceLst[randomChoice], hit.point + new Vector3(Random.insideUnitCircle.x * _radius, 0, Random.insideUnitCircle.y * _radius), quaternion.RotateY(Random.Range(0f,180f)));
        o.GetComponent<MeshRenderer>().sharedMaterial.enableInstancing = true;
        _spawnedPrefabs.Add(o);
        
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
