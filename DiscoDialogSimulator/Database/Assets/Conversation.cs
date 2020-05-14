using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Assets
{
    public class Conversation : Asset
    {
        public List<DialogueEntry> dialogueEntries { get; set; }

        public DialogueEntry GetDialogueEntry(int id)
        {
            return DialogueDict[id];
        }

        private Dictionary<int, DialogueEntry> DialogueDict
        {
            get
            {
                if (dialogueDict == null)
                    dialogueDict = dialogueEntries.ToDictionary(x => x.id);
                return dialogueDict;
            }
        }

        private Dictionary<int, DialogueEntry> dialogueDict;

        public override string ToString()
        {
            return LookupValue("Title");
        }
    }
}
