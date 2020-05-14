using DiscoDialogSimulator.Database;
using DiscoDialogSimulator.Database.Assets;
using System;
using System.Collections.Generic;
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
                    {
                        authResult = "인증 성공";
                    }
                }
                else if (key.Length == 0)
                {
                    authResult = "키 파일이 비어 있음";
                }
                else
                {
                    authResult = "키 길이 불일치";
                }
            }
            else
            {
                authResult = "키 파일이 없음";
            }

            LabelAuth.Content = "인증 결과 : " + authResult;

            sim = new DialogueSimulator(db, dialogueNo, dialogueId, wlc);
            sim.NavigateHandler += HandleRequestNavigate;

            RichTextBoxDialogue.Document = new FlowDocument();

            AddParagraph(new Paragraph(new Run("오른쪽 칸에 대화문 번호와 대사 번호를 입력하고 [시작] 버튼을 누르세요.")));

            // ShowDialogueEntry(627, 1);
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
            TextBoxConversation.Text = "";
            TextBoxDialogue.Text = "";
        }

        private void ButtonFindPrevious_Click(object sender, RoutedEventArgs e)
        {
            ClearDialogueBox();

            if (GetDialogueTextBoxValue(out int conversationId, out int dialogueId))
            {
                var targetEntry = sim.GetDialogueEntry(conversationId, dialogueId);

                var history = new List<DialogueEntry>();
                var currentEntry = targetEntry;

                for (int i = 0; i < 10; i++)
                {
                    var previousEntries = sim.FindPreviousEntry(currentEntry);
                    if (previousEntries.Count == 0)
                        break;

                    currentEntry = previousEntries[0];

                    if (!history.Contains(currentEntry))
                        history.Add(currentEntry);
                    else
                        break;
                }

                for (int i = history.Count - 1; i > 0; i--)
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

            if (!int.TryParse(TextBoxConversation.Text, out conversationId))
            {
                MessageBox.Show($"올바르지 않은 대화문 번호 : \"{TextBoxConversation.Text}\"");
                return false;
            }

            if (!int.TryParse(TextBoxDialogue.Text, out dialogueId))
            {
                MessageBox.Show($"올바르지 않은 대사 번호 : \"{TextBoxDialogue.Text}\"");
                return false;
            }

            try
            {
                sim.GetDialogueEntry(conversationId, dialogueId);
            }
            catch (Exception)
            {
                MessageBox.Show($"존재하지 않는 대화문/대사 번호 : \"{conversationId}/{dialogueId}\"");
                return false;
            }
            return true;
        }

    }
}
