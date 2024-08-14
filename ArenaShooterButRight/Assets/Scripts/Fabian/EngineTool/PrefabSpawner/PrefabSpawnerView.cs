using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerView : EditorWindow
    {
        private PrefabSpawnerViewModel _viewModel;
        
        //visual UI Elements
        private VisualElement _root;
        private ListView _listView;
        private Label _titleLabel;
        private DropdownField _layerDropDown;
        private Toggle _enableButton;
        private Toggle _placeMultiPrefabs;
        private Toggle _checkForOverlap;
        private Button _clearButton;
        private Button _loadDataButton;
        private Button _saveDataButton;
        private Slider _radiusSlider;
        private Slider _minDistanceSlider;
        
        [MenuItem("Fabian/PrefabSpawner")]
        public static void ShowExample()
        {
            PrefabSpawnerView wnd = GetWindow<PrefabSpawnerView>();
            wnd.titleContent = new GUIContent("PrefabSpawner");
        }

        public void CreateGUI()
        {
            _viewModel = new PrefabSpawnerViewModel();
            
            String currentScene =  SceneManager.GetActiveScene().name;
            
            _root = rootVisualElement;
            
            _root.Add(new Label("Prefab Spawner"));
            
            _saveDataButton = new Button { text = "Save current List" };
            _saveDataButton.clicked += () => _viewModel.SaveDataToJson(currentScene, _layerDropDown.value);
            _root.Add(_saveDataButton);
            
            _loadDataButton = new Button { text = "Load List from File" };
            _loadDataButton.clicked += () => _viewModel.LoadDataFromJson(currentScene);
            _loadDataButton.clicked += SetValues;
            _root.Add(_loadDataButton);
            
            _layerDropDown = new DropdownField("Layer: ", _viewModel.LayerMasks,0);
            _root.Add(_layerDropDown);
            
            _enableButton = new Toggle("Enable Painting");
            _root.Add(_enableButton);
            
            _placeMultiPrefabs = new Toggle("Paint Multiple");
            _root.Add(_placeMultiPrefabs);

            _checkForOverlap = new Toggle("Check for overlapping Objects");
            _root.Add(_checkForOverlap);
            
            _root.Add(new Label("Radius"));
            _radiusSlider = new Slider(0f, 5f);
            _radiusSlider.value = _viewModel.Radius;
            _radiusSlider.RegisterValueChangedCallback(evt => _viewModel.Radius = evt.newValue);
            _root.Add(_radiusSlider);
            
            _root.Add(new Label("Spawn distance"));
            _minDistanceSlider = new Slider(0f, 5f);
            _minDistanceSlider.RegisterValueChangedCallback(evt => _viewModel.MinDistanceBetweenPrefabs = evt.newValue);
            _root.Add(_minDistanceSlider);
            
            _clearButton = new Button { text = "Clear spawned prefabs!" };
            _clearButton.clicked += () => _viewModel.ClearSpawnedObjects();
            _root.Add(_clearButton);
            
            DrawReorderableList(_viewModel.PrefabChoiceList, rootVisualElement, false);
            
            SceneView.duringSceneGui += EvaluateMousePosition;
        }

        private void SetValues()
        {
            DrawReorderableList(_viewModel.PrefabChoiceList, rootVisualElement, true);
            _layerDropDown.value = _viewModel.ChosenLayer;
            _minDistanceSlider.value = _viewModel.MinDistanceBetweenPrefabs;
            _radiusSlider.value = _viewModel.Radius;
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
                        _viewModel.SpawnPrefabs(hit.point, _layerDropDown.value, _checkForOverlap.value);
                        break;
                    case 1:
                        _viewModel.DeletePrefabs(_layerDropDown.value);
                        break;
                }
            }

        }
        
        private void SpawnMultiPrefabs(RaycastHit hit)
        {
            if (Event.current.type != EventType.MouseDrag) return;
            
            switch (Event.current.button)
            {
                case 0:
                    _viewModel.SpawnPrefabs(hit.point, _layerDropDown.value, _checkForOverlap.value);
                    break;
                case 1:
                    _viewModel.DeletePrefabs(_layerDropDown.value);
                    break;
            }
        }

        private void DrawReorderableList<T>(List<T> sourceList, VisualElement rootVisElement, bool update, bool allowSceneObjects = true) where T : UnityEngine.Object
        {
            if (update && rootVisElement.Contains(_listView))
            {
                rootVisElement.Remove(_listView);
            }
            
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
