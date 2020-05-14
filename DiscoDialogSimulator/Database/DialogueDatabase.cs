using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using DiscoDialogSimulator.Database.Assets;
using DiscoDialogSimulator.DialogueSystem;

namespace DiscoDialogSimulator.Database
{
    public class DialogueDatabase
    {
        public static DialogueDatabase Deserialize(string path)
        {
            var serializer = new JsonSerializer();

            serializer.Converters.Add(new FieldsConverter());

            using (var reader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return serializer.Deserialize<DialogueDatabase>(jsonReader);
            }
        }

        public Actor GetActor(int id) => ActorDict[id];

        public Conversation GetConversation(int id) => ConversationDict[id];

        public DialogueEntry GetDialogueEntry(int conversationId, int dialogueId)
        {
            return GetConversation(conversationId).GetDialogueEntry(dialogueId);
        }

        public DialogueEntry GetDialogueEntry(string articyId)
        {
            if (DialogueArticyDict.TryGetValue(articyId, out var entry))
                return entry;
            return null;
        }

        private Dictionary<int, Actor> ActorDict
        {
            get
            {
                if (_actorDict == null)
                    _actorDict = actors.ToDictionary(x => x.id);
                return _actorDict;
            }
        }

        private Dictionary<int, Conversation> ConversationDict
        {
            get
            {
                if (_conversationDict == null)
                    _conversationDict = conversations.ToDictionary(x => x.id);
                return _conversationDict;
            }
        }

        private Dictionary<string, DialogueEntry> DialogueArticyDict
        {
            get
            {
                if (_dialogueArticyDict == null)
                {
                    _dialogueArticyDict = new Dictionary<string, DialogueEntry>();
                    foreach (var conversation in this.conversations)
                    {
                        foreach (var dialogue in conversation.dialogueEntries)
                        {
                            if (!dialogue.fields.ContainsKey(FieldNames.DIALOGUE_TEXT) || string.IsNullOrEmpty(dialogue[FieldNames.DIALOGUE_TEXT])) continue;
                            _dialogueArticyDict.Add(dialogue[FieldNames.ARTICY_ID], dialogue);
                        }
                    }

                }
                return _dialogueArticyDict;
            }
        }

        private Dictionary<int, Actor> _actorDict;
        private Dictionary<int, Conversation> _conversationDict;
        private Dictionary<string, DialogueEntry> _dialogueArticyDict;

        public string version { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string globalUserScript { get; set; }

        // public List<EmphasisSetting> emphasisSettings;
        public List<Actor> actors { get; set; }
        public List<Item> items { get; set; }
        public List<Location> locations { get; set; }
        public List<Variable> variables { get; set; }
        public List<Conversation> conversations { get; set; }
    }
}
