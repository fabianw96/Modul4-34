using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerView : EditorWindow
    {
        private PrefabSpawnerViewModel _viewModel;
        
        //Visual UI Elements
        private VisualElement _root;
        private ListView _listView;
        private Label _titleLabel;
        private DropdownField _layerDropDown;
        private Toggle _enableButton;
        private Toggle _placeMultiPrefabs;
        private Button _clearButton;
        private Button _loadDataButton;
        private Button _saveDataButton;
        private Slider _radiusSlider;
        private Slider _minDistanceSlider;
        
        //Testing
        private Scene _currentScene;
        
        [MenuItem("Fabian/PrefabSpawner")]
        public static void ShowExample()
        {
            PrefabSpawnerView wnd = GetWindow<PrefabSpawnerView>();
            wnd.titleContent = new GUIContent("PrefabSpawner");
        }
        

        public void CreateGUI()
        {
            _viewModel = new PrefabSpawnerViewModel();
            //TODO: Save and Load spawned prefabs from json?
            _root = rootVisualElement;
            
            _root.Add(new Label("Prefab Spawner"));
            
            DrawReorderableList(_viewModel.PrefabChoiceList, rootVisualElement);
            
            _layerDropDown = new DropdownField("Layer: ", _viewModel.LayerMasks,0);
            _root.Add(_layerDropDown);
            
            _enableButton = new Toggle("Enable spawning");
            _root.Add(_enableButton);
            
            _placeMultiPrefabs = new Toggle("Spawn Multiple");
            _root.Add(_placeMultiPrefabs);
            
            _root.Add(new Label("Radius"));
            _radiusSlider = new Slider(0f, 5f);
            _radiusSlider.RegisterValueChangedCallback(evt => _viewModel.Radius = evt.newValue);
            _root.Add(_radiusSlider);
            
            _root.Add(new Label("Spawn distance"));
            _minDistanceSlider = new Slider(0f, 5f);
            _minDistanceSlider.RegisterValueChangedCallback(evt => _viewModel.MinDistanceBetweenPrefabs = evt.newValue);
            _root.Add(_minDistanceSlider);
            
            _clearButton = new Button { text = "Clear spawned prefabs!" };
            _clearButton.clicked += () => _viewModel.ClearSpawnedObjects();
            _root.Add(_clearButton);

            _saveDataButton = new Button { text = "Save current List" };
            _root.Add(_saveDataButton);
            _loadDataButton = new Button { text = "Load List from File" };
            _root.Add(_loadDataButton);
            
            SceneView.duringSceneGui += EvaluateMousePosition;

        }

        private void EvaluateMousePosition(SceneView obj)
        {
            Vector3 mousePos = Event.current.mousePosition;
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = obj.camera.pixelHeight - mousePos.y * pixelsPerPoint;
            mousePos.x *= pixelsPerPoint;
        
            Ray ray = obj.camera.ScreenPointToRay(mousePos);

            if (!Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.NameToLayer(_layerDropDown.value)))
                return;
            
            DrawCircle(hit);

            if (!_enableButton.value) 
                return;
                
            GUIUtility.hotControl = 1;
            
            _viewModel.FoundCollidersForDeletion = Physics.OverlapSphere(hit.point, _viewModel.Radius);
            
            switch (_placeMultiPrefabs.value)
            {
                case true:
                    SpawnMultiPrefabs(hit);
                    break;
                case false:
                    SpawnSinglePrefab(hit);
                    break;
            }
        }

        private void DrawCircle(RaycastHit hit)
        {
            if (!_enableButton.value)
                return;

            Handles.color = Color.green;
            Handles.CircleHandleCap(0,hit.point,Quaternion.Euler(90f, 0f, 0f), _viewModel.Radius, EventType.Repaint);
        }

        private void SpawnSinglePrefab(RaycastHit hit)
        {
            if (Event.current.type == EventType.MouseDown)
            {            
                switch (Event.current.button)
                {
                    case 0:
                        _viewModel.SpawnPrefabs(hit.point, _layerDropDown.value);
                        break;
                    case 1:
                        _viewModel.DeletePrefabs(_layerDropDown.value);
                        break;
                }
            };

        }
        
        private void SpawnMultiPrefabs(RaycastHit hit)
        {
            if (Event.current.type != EventType.MouseDrag) return;
            
            switch (Event.current.button)
            {
                case 0:
                    _viewModel.SpawnPrefabs(hit.point, _layerDropDown.value);
                    break;
                case 1:
                    _viewModel.DeletePrefabs(_layerDropDown.value);
                    break;
            }
        }

        private void DrawReorderableList<T>(List<T> sourceList, VisualElement rootVisElement, bool allowSceneObjects = true) where T : UnityEngine.Object
        {
            _listView = new ListView(sourceList)
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
        
            rootVisElement.Add(_listView);
        }
    }
}
