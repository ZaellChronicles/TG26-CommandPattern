using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FeatureCreator.Editor
{
    public class FeatureCreatorWindow : EditorWindow
    {
        #region Unity API

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            root.style.paddingTop = 12;
            root.style.paddingBottom = 12;
            root.style.paddingLeft = 12;
            root.style.paddingRight = 12;

            // --- Feature Name -----
            root.Add(MakeLabel("Feature Name"));
            _nameField = new TextField { value = _featureName };
            _nameField.RegisterValueChangedCallback(evt => _featureName = evt.newValue);
            root.Add(_nameField);
            root.Add(MakeSpacer(12));

            // --- Assemblies -----
            root.Add(MakeLabel("Assemblies"));
            root.Add(MakeSpacer(4));

            _runtimeToggle = new Toggle("Runtime") { value = _createRuntime };
            _runtimeToggle.RegisterValueChangedCallback(evt =>
            {
                _createRuntime = evt.newValue;
                _editorToggle.SetEnabled(_createRuntime);
                if (!_createRuntime) _editorToggle.value = false;
                RefreshReferencesUI();
            });
            root.Add(_runtimeToggle);

            _editorToggle = new Toggle("Editor") { value = _createEditor };
            _editorToggle.RegisterValueChangedCallback(evt => _createEditor = evt.newValue);
            root.Add(_editorToggle);
            root.Add(MakeSpacer(12));

            // --- External References -----
            _referencesSection = new VisualElement();
            root.Add(_referencesSection);

            RefreshReferencesUI();

            // --- Create Button -----
            var createBtn = new Button(OnCreate) { text = "Create Feature" };
            createBtn.style.height = 36;
            createBtn.style.unityFontStyleAndWeight = FontStyle.Bold;
            createBtn.style.marginTop = 8;
            root.Add(createBtn);
        }

        #endregion


        #region Public API

        public static void ShowWindow(string targetPath)
        {
            FeatureCreatorWindow window = GetWindow<FeatureCreatorWindow>(true, "Create Feature", true);
            window._targetPath = targetPath;
            window.minSize = new Vector2(340, 100);
            window.maxSize = new Vector2(340, 600);
        }

        #endregion


        #region Main API

        private void OnCreate()
        {
            string trimmedName = _featureName.Trim();

            if (string.IsNullOrEmpty(trimmedName))
            {
                EditorUtility.DisplayDialog("Create Feature", "Please enter a feature name.", "OK");
                return;
            }

            if (!IsValidFileName(trimmedName))
            {
                EditorUtility.DisplayDialog("Create Feature", $"Feature name '{trimmedName}' contains invalid characters.", "OK");
                return;
            }

            if (!_createRuntime && !_createEditor)
            {
                EditorUtility.DisplayDialog("Create Feature", "Please select at least one assembly to create.", "OK");
                return;
            }

            string featurePath = $"{_targetPath}/{trimmedName}";

            CreateFolderRecursive(featurePath);

            string runtimeAsmdefPath = null;
            string runtimeAsmdefName = $"{trimmedName}.Runtime";

            if (_createRuntime)
            {
                string runtimePath = $"{featurePath}/Runtime";
                string runtimeSrcPath = $"{runtimePath}/src";
                CreateFolderRecursive(runtimeSrcPath);

                List<string> runtimeRefs = new List<string>();
                foreach (AssemblyDefinitionAsset asmdef in _externalReferences)
                {
                    if (asmdef != null) runtimeRefs.Add(asmdef.name);
                }

                runtimeAsmdefPath = $"{runtimePath}/{runtimeAsmdefName}.asmdef";
                CreateAsmdef(runtimeAsmdefPath, runtimeAsmdefName, runtimeRefs, false);
            }

            if (_createEditor)
            {
                string editorPath = $"{featurePath}/Editor";
                string editorSrcPath = $"{editorPath}/src";
                CreateFolderRecursive(editorSrcPath);

                string editorAsmdefName = $"{trimmedName}.Editor";
                string editorAsmdefPath = $"{editorPath}/{editorAsmdefName}.asmdef";

                List<string> editorRefs = new List<string>();
                if (_createRuntime) editorRefs.Add(runtimeAsmdefName);

                CreateAsmdef(editorAsmdefPath, editorAsmdefName, editorRefs, true);
            }

            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Object featureFolder = AssetDatabase.LoadAssetAtPath<Object>(featurePath);
            if (featureFolder != null)
            {
                Selection.activeObject = featureFolder;
                EditorGUIUtility.PingObject(featureFolder);
            }

            Close();
        }

        private void RefreshReferencesUI()
        {
            _referencesSection.Clear();

            if (!_createRuntime) return;

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.marginBottom = 4;

            header.Add(MakeLabel("External References"));

            var addBtn = new Button(() =>
            {
                _externalReferences.Add(null);
                RefreshReferencesUI();
            })
            { text = "+" };
            addBtn.style.width = 24;
            addBtn.style.height = 18;
            addBtn.style.marginLeft = 4;
            header.Add(addBtn);

            _referencesSection.Add(header);

            if (_externalReferences.Count == 0)
            {
                var hint = new Label("No external references.");
                hint.style.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                hint.style.marginBottom = 8;
                _referencesSection.Add(hint);
            }
            else
            {
                for (int i = 0; i < _externalReferences.Count; i++)
                {
                    int index = i;

                    var row = new VisualElement();
                    row.style.flexDirection = FlexDirection.Row;
                    row.style.marginBottom = 2;

                    var field = new ObjectField
                    {
                        objectType = typeof(AssemblyDefinitionAsset),
                        allowSceneObjects = false,
                        value = _externalReferences[index]
                    };
                    field.style.flexGrow = 1;
                    field.RegisterValueChangedCallback(evt =>
                    {
                        _externalReferences[index] = evt.newValue as AssemblyDefinitionAsset;
                    });

                    var removeBtn = new Button(() =>
                    {
                        _externalReferences.RemoveAt(index);
                        RefreshReferencesUI();
                    })
                    { text = "✕" };
                    removeBtn.style.width = 24;
                    removeBtn.style.marginLeft = 2;

                    row.Add(field);
                    row.Add(removeBtn);
                    _referencesSection.Add(row);
                }

                _referencesSection.Add(MakeSpacer(8));
            }
        }

        private static void CreateAsmdef(string path, string name, List<string> references, bool editorOnly)
        {
            var refs = new List<string>();
            foreach (string r in references)
            {
                refs.Add($"        \"{r}\"");
            }

            string refsBlock = refs.Count > 0
                ? $"[\n{string.Join(",\n", refs)}\n    ]"
                : "[]";

            string includePlatforms = editorOnly ? "\n    \"includePlatforms\": [\"Editor\"]," : "";

            string content =
                $"{{\n" +
                $"    \"name\": \"{name}\",{includePlatforms}\n" +
                $"    \"rootNamespace\": \"{name}\",\n" +
                $"    \"references\": {refsBlock},\n" +
                $"    \"autoReferenced\": true\n" +
                $"}}";

            File.WriteAllText(Path.Combine(Application.dataPath, "..", path), content);
        }

        private static void CreateFolderRecursive(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static Label MakeLabel(string text)
        {
            Label label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 2;
            return label;
        }

        private static VisualElement MakeSpacer(float height)
        {
            var spacer = new VisualElement();
            spacer.style.height = height;
            return spacer;
        }

        private static bool IsValidFileName(string name) =>
            System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_\-]+$");

        #endregion


        #region Privates and Protecteds

        private string _targetPath = "Assets";
        private string _featureName = "";
        private bool _createRuntime = true;
        private bool _createEditor = false;
        private List<AssemblyDefinitionAsset> _externalReferences = new List<AssemblyDefinitionAsset>();

        private TextField _nameField;
        private Toggle _runtimeToggle;
        private Toggle _editorToggle;
        private VisualElement _referencesSection;

        #endregion
    }
}