using DiscoDialogSimulator.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.Database.Assets
{
    public class DialogueEntry : Asset
    {
        public override string ToString()
        {
            return LookupValue("Title");
        }

        public bool IsJanusNode() => ContainsKey("Condition1");
        public bool IsWhiteCheck() => ContainsKey("DifficultyWhite");
        public bool IsRedCheck() => ContainsKey("DifficultyRed");
        public bool IsPassiveCheck() => ContainsKey("DifficultyPass");
        public bool IsAlsoAntipassive() => ContainsKey("Antipassive");
        public bool IsCostOptionNode() => ContainsKey("ClickCost");
        public int GetCost() => int.Parse(this["ClickCost"]);


        public int conversationID { get; set; }
        public bool isRoot { get; set; }
        public bool isGroup { get; set; }
        public string nodeColor { get; set; }
        public bool delaySimStatus { get; set; }
        public string falseConditionAction { get; set; }
        public ConditionPriority conditionPriority { get; set; }
        public List<Link> outgoingLinks { get; set; }

        public string conditionsString { get; set; }
        public string userScript { get; set; }
        public class Link
        {
            public int originConversationID { get; set; }
            public int originDialogueID { get; set; }
            public int destinationConversationID { get; set; }
            public int destinationDialogueID { get; set; }
            public bool isConnector { get; set; }

            public ConditionPriority priority { get; set; }
        }
    }
}
