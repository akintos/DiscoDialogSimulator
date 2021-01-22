using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator
{
    public class SettingsManager
    {
        private static SettingsManager instance;

        private bool showDialogueNo;
        private bool showDialogueId;
        private bool showArticyId;
        private bool enableTranslation;
        private bool showSource;
        private bool showCondition;

        public static SettingsManager GetInstance()
        {
            if (instance == null)
                instance = new SettingsManager();
            return instance;
        }

        private SettingsManager()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            showDialogueNo = Properties.Settings.Default.ShowDialogueNo;
            showDialogueId = Properties.Settings.Default.ShowDialogueId;
            showArticyId = Properties.Settings.Default.ShowArticyId;
            enableTranslation = Properties.Settings.Default.EnableTranslation;
            showSource = Properties.Settings.Default.ShowSource;
            showCondition = Properties.Settings.Default.ShowCondition;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.ShowDialogueNo = showDialogueNo;
            Properties.Settings.Default.ShowDialogueId = showDialogueId;
            Properties.Settings.Default.ShowArticyId = showArticyId;
            Properties.Settings.Default.EnableTranslation = enableTranslation;
            Properties.Settings.Default.ShowSource = showSource;
            Properties.Settings.Default.ShowCondition = showCondition;

            Properties.Settings.Default.Save();
        }

        public bool ShowDialogueNo
        {
            get => showDialogueNo;
            set {showDialogueNo = value; SaveSettings(); }
        }
        public bool ShowDialogueId
        {
            get => showDialogueId;
            set { showDialogueId = value; SaveSettings(); }
        }
        public bool ShowArticyId
        { 
            get => showArticyId; 
            set { showArticyId = value; SaveSettings(); } 
        }
        public bool EnableTranslation 
        { 
            get => enableTranslation; 
            set { enableTranslation = value; SaveSettings(); } 
        }
        public bool ShowSource 
        {
            get => showSource; 
            set { showSource = value; SaveSettings(); } 
        }
        public bool ShowCondition 
        { 
            get => showCondition;
            set { showCondition = value; SaveSettings(); } 
        }
    }
}
