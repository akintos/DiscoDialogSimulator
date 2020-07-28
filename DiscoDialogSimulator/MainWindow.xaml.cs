using DiscoDialogSimulator.Database;
using DiscoDialogSimulator.Database.Assets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiscoDialogSimulator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string KEY_FILENAME = "api_key.txt";

        private readonly DialogueSimulator sim;

        private Paragraph lastResponseParagraph;

        public MainWindow()
        {
            InitializeComponent();

            var db = DialogueDatabase.Deserialize("data/database.json");
            var dialogueNo = DialogueNumber.LoadJson("data/dialogue_no.json");
            var dialogueId = DialogueId.LoadJson("data/dialogue_id.json");

            WeblateClient wlc = null;
            string authResult = string.Empty;

            if (File.Exists(KEY_FILENAME))
            {
                string key = File.ReadAllText(KEY_FILENAME).Trim();
                if (key.Length == 40)
                {
                    wlc = new WeblateClient("http://akintos.iptime.org/api/", key);
                    if (!wlc.TestAuth("disco-elysium"))
                    {
                        wlc = null;
                        authResult = "인증 실패";
                    }
                    else
                        authResult = "인증 성공";
                }
                else if (key.Length == 0)
                    authResult = "키 파일이 비어 있음";
                else
                    authResult = "키 길이 불일치";
            }
            else
                authResult = "키 파일이 없음";

            LabelAuth.Content = "인증 결과 : " + authResult;

            sim = new DialogueSimulator(db, dialogueNo, dialogueId, wlc);
            sim.NavigateHandler += HandleRequestNavigate;

            RichTextBoxDialogue.Document = new FlowDocument();

            AddParagraph(new Paragraph(new Run("Enter Conversation/Dialogue/Articy ID and press [Start] button.")));

            ReadConfiguration();
        }

        private void ReadConfiguration()
        {
            CheckBoxShowArticyId.IsChecked = sim.ShowArticyId = bool.Parse(ConfigurationManager.AppSettings["ShowArticyId"] ?? "false");
            CheckBoxShowDialogueNo.IsChecked = sim.ShowDialogueNo = bool.Parse(ConfigurationManager.AppSettings["ShowDialogueNo"] ?? "false");
            CheckBoxShowDialogueId.IsChecked =  sim.ShowDialogueId = bool.Parse(ConfigurationManager.AppSettings["ShowDialogueId"] ?? "false");
            CheckBoxEnableTranslation.IsChecked = sim.EnableTranslation = bool.Parse(ConfigurationManager.AppSettings["EnableTranslation"] ?? "true");
            CheckBoxShowSource.IsChecked = sim.ShowSource = bool.Parse(ConfigurationManager.AppSettings["ShowSource"] ?? "false");
            CheckBoxShowCondition.IsChecked = sim.ShowCondition = bool.Parse(ConfigurationManager.AppSettings["ShowCondition"] ?? "false");
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                settings.SaveSetting("ShowArticyId", sim.ShowArticyId.ToString());
                settings.SaveSetting("ShowDialogueNo", sim.ShowDialogueId.ToString());
                settings.SaveSetting("ShowDialogueId", sim.ShowDialogueId.ToString());
                settings.SaveSetting("EnableTranslation", sim.EnableTranslation.ToString());
                settings.SaveSetting("ShowSource", sim.ShowSource.ToString());
                settings.SaveSetting("ShowCondition", sim.ShowCondition.ToString());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void ShowDialogueEntry(int conversationId, int dialogueId)
        {
            var currentEntry = sim.GetDialogueEntry(conversationId, dialogueId);

            while (true)
            {
                var paragraph = sim.GetDialogueParagraph(currentEntry);

                if (paragraph != null && paragraph.Inlines.Count != 0)
                    AddParagraph(paragraph);

                if (currentEntry.outgoingLinks.Count != 1)
                    break;

                currentEntry = sim.GetNextEntry(currentEntry.outgoingLinks[0]);
            }

            if (currentEntry.outgoingLinks.Count == 0)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run("\n[END OF CONVERSATION]\n"));
                AddParagraph(paragraph);
                RichTextBoxDialogue.ScrollToEnd();
                return;
            }

            ShowResponse(currentEntry);

            RichTextBoxDialogue.ScrollToEnd();
        }

        private void ShowResponse(DialogueEntry currentEntry)
        {
            lastResponseParagraph = sim.GetResponseParagraph(currentEntry);
            AddParagraph(lastResponseParagraph);
        }

        private void AddParagraph(Paragraph graph)
        {
            RichTextBoxDialogue.Document.Blocks.Add(graph);
        }

        private void ClearDialogueBox()
        {
            RichTextBoxDialogue.Document.Blocks.Clear();
            lastResponseParagraph = null;
        }

        private void HandleRequestNavigate(object sender, RequestNavigateEventArgs args)
        {
            var uri = args.Uri;
            if (uri.Scheme == "http")
            {
                System.Diagnostics.Process.Start(uri.ToString());
                return;
            }

            if (uri.Scheme != "dialog")
                throw new Exception($"Unexpected URI {uri}");

            if (lastResponseParagraph != null && RichTextBoxDialogue.Document.Blocks.Remove(lastResponseParagraph))
                lastResponseParagraph = null;

            ShowDialogueEntry(int.Parse(uri.Host), uri.Port);
        }

        private void TextBoxConversation_TextChanged(object sender, TextChangedEventArgs e)
        {
            LabelConversation.Visibility = string.IsNullOrEmpty((sender as TextBox)?.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBoxDialogue_TextChanged(object sender, TextChangedEventArgs e)
        {
            LabelDialogue.Visibility = string.IsNullOrEmpty((sender as TextBox)?.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TextBoxArticyID_TextChanged(object sender, TextChangedEventArgs e)
        {
            LabelArticyID.Visibility = string.IsNullOrEmpty((sender as TextBox)?.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBoxConversation.Clear();
            TextBoxDialogue.Clear();
            TextBoxArticyID.Clear();
        }

        private void ButtonFindPrevious_Click(object sender, RoutedEventArgs e)
        {
            ClearDialogueBox();

            if (GetDialogueTextBoxValue(out int conversationId, out int dialogueId))
            {
                var targetEntry = sim.GetDialogueEntry(conversationId, dialogueId);

                var history = new List<DialogueEntry>();
                var currentEntry = targetEntry;

                while (history.Count < 10)
                {
                    var previousEntries = sim.FindPreviousEntry(currentEntry);
                    if (previousEntries.Count == 0)
                        break;

                    currentEntry = previousEntries[0];

                    if (history.Contains(currentEntry))
                        break;

                    if (currentEntry.outgoingLinks.Count == 1)
                        history.Add(currentEntry);
                }

                for (int i = history.Count - 1; i >= 0; i--)
                {
                    var prevParagraph = sim.GetDialogueParagraph(history[i]);
                    if (prevParagraph != null) AddParagraph(prevParagraph);
                }
                var paragraph = sim.GetDialogueParagraph(targetEntry);
                if (paragraph != null) AddParagraph(paragraph);
            }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (GetDialogueTextBoxValue(out int conversationId, out int dialogueId))
            {
                ClearDialogueBox();
                ShowDialogueEntry(conversationId, dialogueId);
            }
        }

        private bool GetDialogueTextBoxValue(out int conversationId, out int dialogueId)
        {
            conversationId = 0;
            dialogueId = 0;

            if (!string.IsNullOrEmpty(TextBoxArticyID.Text))
            {
                var entry = sim.GetDialogueEntry(TextBoxArticyID.Text);
                if (entry == null)
                {
                    MessageBox.Show($"Invalid Articy ID : \"{TextBoxArticyID.Text}\"");
                    return false;
                }
                conversationId = entry.conversationID;
                dialogueId = entry.id;
                return true;
            }

            if (!int.TryParse(TextBoxConversation.Text, out conversationId))
            {
                MessageBox.Show($"Invalid conversation no : \"{TextBoxConversation.Text}\"");
                return false;
            }

            if (!int.TryParse(TextBoxDialogue.Text, out dialogueId))
            {
                MessageBox.Show($"Invalid dialogue no : \"{TextBoxDialogue.Text}\"");
                return false;
            }

            try
            {
                sim.GetDialogueEntry(conversationId, dialogueId);
            }
            catch (Exception)
            {
                MessageBox.Show($"Missing conversation/dialogue no : \"{conversationId}/{dialogueId}\"");
                return false;
            }
            return true;
        }

        private void OptionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == CheckBoxShowDialogueNo)
                sim.ShowDialogueNo = CheckBoxShowDialogueNo.IsChecked.Value;
            else if (sender == CheckBoxShowDialogueId)
                sim.ShowDialogueId = CheckBoxShowDialogueId.IsChecked.Value;
            else if (sender == CheckBoxShowArticyId)
                sim.ShowArticyId = CheckBoxShowArticyId.IsChecked.Value;
            else if (sender == CheckBoxEnableTranslation)
                sim.EnableTranslation = CheckBoxEnableTranslation.IsChecked.Value;
            else if (sender == CheckBoxShowSource)
                sim.ShowSource = CheckBoxShowSource.IsChecked.Value;
            else if (sender == CheckBoxShowCondition)
                sim.ShowCondition = CheckBoxShowCondition.IsChecked.Value;
            else
                return;

            SaveConfiguration();
        }
    }
}
