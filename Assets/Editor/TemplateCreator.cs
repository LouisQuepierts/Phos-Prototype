using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System.Collections.Generic;
using Phos.Utils;

namespace PhosEditor {
    public class TemplateCreator : Editor {
        public const string ProjectName = "Phos";

        private delegate string Preprocessor(string text, string filename);
        private static Preprocessor preprocessor = DefaultPreprocessor;

        private static Dictionary<string, string> mapping = new();



        [MenuItem("Assets/Create/C# Script", false, 1)]
        static void CreateCSharp() {
            preprocessor = DefaultPreprocessor;
            CreateTemplate("CSharp.cs");
        }

        [MenuItem("Assets/Create/ScritableObject Script", false, 1)]
        static void CreateScriptableObject() {
            preprocessor = DefaultPreprocessor;
            CreateTemplate("ScriptableObject.cs");
        }

        [MenuItem("Assets/Create/Predicate Script", false, 1)]
        static void CreatePredicate() {
            preprocessor = DefaultPreprocessor;
            CreateTemplate("Predicate.cs");
        }

        [MenuItem("Assets/Create/Inspector Script", false, 1)]
        static void CreateInspector() {
            preprocessor = (text, filename) => {
                string objectType = filename.Replace("Inspector", "");
                text = Regex.Replace(text, "#SCRIPTNAME#", filename);
                return text;
            };
            CreateTemplate("Inspector.cs");
        }

        private static void CreateTemplate(string template) {
            string path = "Assets";
            foreach (UnityEngine.Object item in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0, CreateInstance<CreateScriptAsset>(),
                path + "/New" + template,
                null, "Assets/Editor/Templates/" + template + ".txt"
            );
        }

        private static string DefaultPreprocessor(string text, string filename) {
            return Regex.Replace(text, "#SCRIPTNAME#", filename);
        }

        class CreateScriptAsset : EndNameEditAction {
            public override void Action(int instanceId, string newScriptPath, string templatePath) {
                UnityEngine.Object obj = CreateTemplateScriptAsset(newScriptPath, templatePath);
                ProjectWindowUtil.ShowCreatedAsset(obj);
            }

            public static UnityEngine.Object CreateTemplateScriptAsset(string newScriptPath, string templatePath) {
                string fullPath = Path.GetFullPath(newScriptPath);
                StreamReader streamReader = new StreamReader(templatePath);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                string filename = Path.GetFileNameWithoutExtension(newScriptPath);
                string directory = Path.GetDirectoryName(newScriptPath);

                string @namespace = directory
                    .Replace("Assets\\Scripts", ProjectName)
                    .Replace("Assets\\Editor", ProjectName + "Editor")
                    .Replace('\\', '.');

                Debug.Log(directory);

                mapping.Clear();
                mapping["SCRIPTNAME"] = filename;
                mapping["NAMESPACE"] = @namespace;
                //text = preprocessor?.Invoke(text, filename);
                text = StringUtils.EnhancedReplace(text, mapping);

                bool encoderShouldEmitUTF8Identifier = true;
                bool throwOnInvalidBytes = false;
                UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
                bool append = false;
                StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
                streamWriter.Write(text);
                streamWriter.Close();
                AssetDatabase.ImportAsset(newScriptPath);
                return AssetDatabase.LoadAssetAtPath(newScriptPath, typeof(UnityEngine.Object));
            }
        }
    }
}