using DiscoDialogSimulator.Database;
using DiscoDialogSimulator.Database.Assets;
using DiscoDialogSimulator.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace DiscoDialogSimulator
{
    public class DialogueSimulator
    {
        private readonly DialogueDatabase db;
        private readonly DialogueNumber dialogueNo;

        private readonly DialogueId dialogueId;
        private readonly WeblateClient wlc;

        public event RequestNavigateEventHandler NavigateHandler;

        public DialogueSimulator(DialogueDatabase db, DialogueNumber dialogueNo, DialogueId dialogueId, WeblateClient wlc)
        {
            this.db = db;
            this.dialogueNo = dialogueNo;
            this.dialogueId = dialogueId;
            this.wlc = wlc;
        }

        public Paragraph GetDialogueParagraph(int conversationId, int dialogueId)
        {
            return GetDialogueParagraph(db.GetDialogueEntry(conversationId, dialogueId));
        }

        public Paragraph GetDialogueParagraph(DialogueEntry dialogue)
        {
            var hasBody = false;
            var paragraph = new Paragraph();

            bool isWhiteCheck = dialogue.IsWhiteCheck();
            bool isRedCheck = dialogue.IsRedCheck();
            bool isPassiveCheck = dialogue.IsPassiveCheck();

            if (isWhiteCheck || isRedCheck)
            {
                var skillName = ArticyBridge.ArticyIdToSkillName[dialogue[FieldNames.SKILLTYPE]];
                var difficultyId = isWhiteCheck ? dialogue[FieldNames.DIFFICULTYWHITE] : dialogue[FieldNames.DIFFICULTYRED];
                int difficulty = ArticyBridge.GetDifficultyValue(int.Parse(difficultyId));

                var checkRun = new Run($"[{skillName} {difficulty}] ");
                checkRun.Foreground = isWhiteCheck ? Brushes.Gray : Brushes.Red;

                paragraph.Inlines.Add(checkRun);
            }
            else if (isPassiveCheck)
            {
                int difficulty = ArticyBridge.GetDifficultyValue(int.Parse(dialogue[FieldNames.DIFFICULTYPASS]));
                var checkRun = new Run($"[Passive check {difficulty}] ");
                paragraph.Inlines.Add(checkRun);
            }

            if (dialogue.ContainsKey(FieldNames.ACTOR) && dialogue[FieldNames.ACTOR] != "0")
            {
                var actor = db.GetActor(int.Parse(dialogue[FieldNames.ACTOR]));
                var actorRun = new Run(actor.Name + " : ");
                actorRun.FontWeight = FontWeights.Bold;
                paragraph.Inlines.Add(actorRun);
            }

            if (dialogue.ContainsKey(FieldNames.DIALOGUE_TEXT))
            {
                paragraph.Inlines.Add(MakeDialogueHyperlinkText(dialogue, FieldNames.DIALOGUE_TEXT));
                hasBody = true;
            }

            if (dialogue.IsJanusNode())
            {
                for (int i = 1; i <= 4; i++)
                {
                    if (string.IsNullOrWhiteSpace(dialogue["Alternate" + i])) continue;

                    var alternativeRun = MakeDialogueHyperlinkText(dialogue, "Alternate" + i);
                    alternativeRun.Foreground = Brushes.DarkSlateGray;
                    paragraph.Inlines.Add(new Run("\nAlternate" + i + " : "));
                    paragraph.Inlines.Add(alternativeRun);
                }
                hasBody = true;
            }

            if (isWhiteCheck || isRedCheck)
            {
                for (int i = 1; i <= 10; i++)
                {
                    if (string.IsNullOrWhiteSpace(dialogue["tooltip" + i])) continue;

                    var modifierRun = MakeDialogueHyperlinkText(dialogue, "tooltip" + i);
                    var modifier = dialogue["modifier" + i];
                    modifierRun.Inlines.Add(new Run(" " + modifier));
                    modifierRun.Foreground = Brushes.Gray;
                    paragraph.Inlines.Add(new Run("\n    "));
                    paragraph.Inlines.Add(modifierRun);
                }
                hasBody = true;
            }

            return hasBody ? paragraph : null;
        }

        public IList<DialogueEntry> FindPreviousEntry(DialogueEntry targetEntry)
        {
            var result = new List<DialogueEntry>();

            foreach (var entry in db.GetConversation(targetEntry.conversationID).dialogueEntries)
            {
                foreach (var link in entry.outgoingLinks)
                {
                    if (link.destinationConversationID == targetEntry.conversationID && link.destinationDialogueID == targetEntry.id)
                    {
                        result.Add(entry);
                        break;
                    }   
                }
            }

            return result;
        }

        public Paragraph GetResponseParagraph(DialogueEntry entry)
        {
            var paragraph = new Paragraph();

            for (int i = 0; i < entry.outgoingLinks.Count; i++)
            {
                var nextEntry = GetNextEntry(entry.outgoingLinks[i]);

                string text = $"{i + 1}. ";

                bool isRedCheck = nextEntry.IsRedCheck();
                bool isWhiteCheck = nextEntry.IsWhiteCheck();

                if (isRedCheck || isWhiteCheck)
                {
                    var skillName = ArticyBridge.ArticyIdToSkillName[nextEntry[FieldNames.SKILLTYPE]];
                    var difficultyId = isWhiteCheck ? nextEntry[FieldNames.DIFFICULTYWHITE] : nextEntry[FieldNames.DIFFICULTYRED];
                    int difficulty = ArticyBridge.GetDifficultyValue(int.Parse(difficultyId));
                    text += $"[{skillName} {difficulty}] ";
                }

                if (nextEntry.ContainsKey(FieldNames.DIALOGUE_TEXT))
                    text += GetTranslatedField(nextEntry, FieldNames.DIALOGUE_TEXT);
                else
                    text += nextEntry[FieldNames.TITLE];
                text += "\n";

                var textRun = new Run(text);
                Hyperlink textLink = new Hyperlink(textRun);
                textLink.NavigateUri = new Uri($"dialog://{nextEntry.conversationID}:{nextEntry.id}/");
                textLink.TextDecorations = null;            // Remove underline
                textLink.Foreground = Brushes.Black;        // Remove blue hyperlink color
                textLink.RequestNavigate += NavigateHandler;

                if (isRedCheck) textLink.Foreground = Brushes.Red;
                if (isWhiteCheck) textLink.Foreground = Brushes.Gray;

                paragraph.Inlines.Add(textLink);
            }

            return paragraph;
        }

        public DialogueEntry GetNextEntry(DialogueEntry.Link link)
        {
            return db.GetConversation(link.destinationConversationID).GetDialogueEntry(link.destinationDialogueID);
        }

        public DialogueEntry GetDialogueEntry(int conversationId, int dialogueId)
        {
            return db.GetDialogueEntry(conversationId, dialogueId);
        }

        public Hyperlink MakeDialogueHyperlinkText(DialogueEntry entry, string fieldName)
        {
            int no = dialogueNo[fieldName + "/" + entry[FieldNames.ARTICY_ID]];
            var textRun = new Run(GetTranslatedField(entry, fieldName));
            Hyperlink textLink = new Hyperlink(textRun)
            {
                NavigateUri = new Uri("http://akintos.iptime.org/translate/disco-elysium/dialogue/ko/?offset=" + no),
                TextDecorations = null,            // Remove underline
                Foreground = Brushes.Black        // Remove blue hyperlink color
            };
            textLink.RequestNavigate += NavigateHandler;

            return textLink;
        }

        private string GetTranslatedField(DialogueEntry entry, string fieldName)
        {
            string key = fieldName + "/" + entry[FieldNames.ARTICY_ID];
            if (wlc != null && dialogueId != null && dialogueId.TryGetValue(key, out int id))
            {
                var tr = wlc.GetTranslation(id);
                if (tr != null && tr.translated)
                    return tr.target;
            }
            return entry[fieldName];
        }
    }
}
