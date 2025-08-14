using UIFramework.View;
using UnityEditor;
using UnityEngine;

namespace UIFramework.Editor
{
    public class OdinMenuItemExtensions
    {
        
        [MenuItem("UI/MVP生成器")]
        public static void ShowViewGeneratorWindow(MenuCommand command)
        {
            var window = UnityEditor.EditorWindow.GetWindow<ViewGeneratorWindow>();
            // var window = UnityEditor.EditorWindow.GetWindowWithRect<ViewGeneratorWindow>(new Rect(0, 0, 1000, 620));
            window.Show();
            BaseView view = command.context as BaseView;
            if (view == null)
            {
                Debug.Log("view is null");
                return;
            }

            window.SetViewComponent(view);
        }
        
        [MenuItem("CONTEXT/BaseView/绑定View组件")]
        public static void BindViewField(MenuCommand command)
        {
            ViewBinder binder = new ViewBinder(command.context);
            binder.Bind();
        }
    }
}