using LegionMaster.Config;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.Config
{
    public class ConfigDownloaderWindow: EditorWindow
    {
        private string _mainUrl = "https://docs.google.com/spreadsheets/d/1mGqnPeUgnBW56DfwwTtDw2_bN-giVftBK9TseADS5Og";

        private string _hyperCasualUrl =
            "https://docs.google.com/spreadsheets/d/1Y72wJA_x7QGOhcDHAycNWJ39UsG7gpe1U4ACd0LbnHY";
        private const int MAIN_SHEET_ID_LIST = 515831250; //id of sheet that contains list of all other sheets
        private const int HYPERCASUAL_SHEET_ID_LIST = 515831250;
        private const string MAIN_CONFIG_PATH = "Resources/Configs";
        private const string HYPERCASUAL_CONFIG_PATH = "Resources/Configs/HyperCasual";

        [MenuItem("LM/Download Configs")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ConfigDownloaderWindow));
        }
        
        private void OnGUI () {
            GUILayout.Label("URL of google sheet with main configs", EditorStyles.boldLabel);
            GUILayout.Label("Should have format: https://docs.google.com/spreadsheets/d/KEY", EditorStyles.label);
            GUILayout.Label("without last / and anything after it", EditorStyles.label);
            _mainUrl = EditorGUILayout.TextField ("Url", _mainUrl);
            GUILayout.Label("URL of google sheet with hypercasual configs", EditorStyles.boldLabel);
            _hyperCasualUrl = EditorGUILayout.TextField ("Url", _hyperCasualUrl);

            if (GUILayout.Button("Download all"))
            {
                DownloadAll();
            }
            if (GUILayout.Button("Download localization only"))
            {
                DownloadLocalization();
            }
        }

        private void DownloadAll()
        {
            new ConfigDownloader(_mainUrl, MAIN_SHEET_ID_LIST).Download(MAIN_CONFIG_PATH);
            new ConfigDownloader(_hyperCasualUrl, HYPERCASUAL_SHEET_ID_LIST).Download(HYPERCASUAL_CONFIG_PATH);
        }
        
        private void DownloadLocalization()
        {
            new ConfigDownloader(_mainUrl, MAIN_SHEET_ID_LIST).Download(MAIN_CONFIG_PATH,new[] { Configs.LOCALIZATION });
        }
    }
}