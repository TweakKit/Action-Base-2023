using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Data
{
    public class DataCleaner
    {
        #region Class Methods

        [MenuItem("Tools/Clean Data", false, 1)]
        public static void CleanData()
        {
            Debug.LogWarning("Clean all data");
            PlayerPrefs.DeleteAll();
            ClearFolder( Application.persistentDataPath + "/Save");
        }

        private static void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach(FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }
        }
        
        #endregion Class Methods
    }
}