using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawner : EditorWindow
    {
        //Lists for prefabs
        private List<GameObject> _prefabChoiceLst = new();
        private List<GameObject> _spawnedPrefabs = new();
        private GameObject[] _batchingObjects;
        private Dictionary<GameObject, Transform> _positionDictionary = new ();
    
        //Visual UI Elements
        private VisualElement _root;
        private ListView _listView;
        private Label _label;
        private DropdownField _prefabDropDown;
        private Toggle _enableButton;
        private Toggle _placeMultiPrefabs;
        private Button _clearButton;
        private Button _loadPrefabsButton;
        private Slider _radiusSlider;
        private Slider _minDistanceSlider;
    
        //Values
        private float _radius;
        private float _minDistanceBetweenPrefabs;
        private string _dataPath;
        private string[] _subPaths;


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

            #region FolderTesting
            //
            // //dropdown or something to choose a filepath for listview
            // if (!Directory.Exists(Application.dataPath + "/Resources"))
            // {
            //     Debug.LogWarning("Please make sure a Resources folder exists.");
            // }
            //
            // _dataPath = Application.dataPath + "/Resources";
            // _subPaths = Directory.GetDirectories(_dataPath);
            // Debug.Log(_subPaths[0]);
            //
            // List<string> pathList = new();
            //
            // foreach (var path in _subPaths)
            // {
            //     pathList.Add(path);
            // }
            //
            // _prefabDropDown = new DropdownField("Folder", pathList, 0);
            // string chosenpath = _prefabDropDown.value;
            // _root.Add(_prefabDropDown);
            // _loadPrefabsButton.RegisterCallback<ClickEvent>(LoadPrefabsFromFolder);
            // _loadPrefabsButton = new Button()
            // {
            //     text = "Load prefabs from folder"
            // };
            // _root.Add(_loadPrefabsButton);
            #endregion
            
            _label = new Label("PrefabSpawner");
            _root.Add(_label);
            DrawReorderableList(_prefabChoiceLst, rootVisualElement);
            _enableButton = new Toggle("Enable spawning");
            _placeMultiPrefabs = new Toggle("Spawn Multiple");
            _label = new Label("Radius");
            _root.Add(_label);
            _radiusSlider = new Slider(0f, 5f);
            _root.Add(_radiusSlider);
            _label = new Label("Min Distance between Prefabs");
            _root.Add(_label);
            _minDistanceSlider = new Slider(0f, 5f);
            _root.Add(_minDistanceSlider);
            _clearButton = new Button
            {
                text = "Clear spawned prefabs!",
            };
        
            _clearButton.RegisterCallback<ClickEvent>(ClearSpawnedObjects);
        
            _root.Add(_enableButton);
            _root.Add(_placeMultiPrefabs);
            _root.Add(_clearButton);
        }

        // private void LoadPrefabsFromFolder(ClickEvent evt)
        // {
        //     _prefabChoiceLst.Clear();
        //
        //     if (_root.Contains(_listView))
        //     {
        //         _root.Remove(_listView);
        //     }
        //     
        //     GameObject[] gos = Resources.LoadAll<GameObject>(_dataPath);
        //     
        //     foreach (var gosobj in gos)
        //     {
        //         _prefabChoiceLst.Add(gosobj);
        //     }
        // }

        private void EvaluateMousePosition(SceneView obj)
        {
            Vector3 mousePos = Event.current.mousePosition;
            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = obj.camera.pixelHeight - mousePos.y * pixelsPerPoint;
            mousePos.x *= pixelsPerPoint;
        
            Ray ray = obj.camera.ScreenPointToRay(mousePos);
            _radius = _radiusSlider.value;
            _minDistanceBetweenPrefabs = _minDistanceSlider.value;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                DrawCircle(hit);

                if (!_enableButton.value) return;
                
                GUIUtility.hotControl = 1;

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
        }

        private void ClearSpawnedObjects(ClickEvent evt)
        {
            foreach (var obj in _spawnedPrefabs)
            {
                DestroyImmediate(obj);
            }
        
            _spawnedPrefabs.Clear();
            _positionDictionary.Clear();
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
        
            int randomChoice = Random.Range(0, _prefabChoiceLst.Count);
            GameObject o = Instantiate(_prefabChoiceLst[randomChoice], hit.point + new Vector3(Random.insideUnitCircle.x * _radius, 0, Random.insideUnitCircle.y * _radius), quaternion.RotateY(Random.Range(0f,180f)));
            CheckOverlappingPrefabs(o);

            if (o == null) return;
            
            _positionDictionary.Add(o, o.transform);
            o.GetComponent<MeshRenderer>().sharedMaterial.enableInstancing = true;
            _spawnedPrefabs.Add(o);
        }

        private void SpawnSinglePrefab(RaycastHit hit)
        {
            if (Event.current.type != EventType.MouseDown) return;
            DrawPrefabs(hit);
        }

        private void SpawnMultiPrefabs(RaycastHit hit)
        {
            if (Event.current.type != EventType.MouseDrag) return;
            DrawPrefabs(hit);
        }

        private void CheckOverlappingPrefabs(GameObject o)
        {
            foreach (KeyValuePair<GameObject, Transform> obj in _positionDictionary)
            {
                if (Vector3.Distance(o.transform.position, obj.Value.position) < _minDistanceBetweenPrefabs)
                {
                    DestroyImmediate(o);
                    return;
                }
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
