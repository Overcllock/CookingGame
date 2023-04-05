using Game.Content;

namespace UnityEditor.MenuItems
{
    public static class ContentEditorMenus
    {
        [MenuItem(EditorConsts.CONTENT_MENU + "Update all databases", priority = 1000)]
        public static void UpdateAllDatabases()
        {
            var dbs = AssetDatabaseUtility.GetAssets<ScrobjectDatabase>();
            foreach (var db in dbs)
            {
                db.UpdateContent();
            }

            AssetDatabase.SaveAssets();
        }
    }
}