using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.DialogueSystem
{
    public static class FieldNames
    {
        /// <summary>
        /// Dialogue Articy ID
        /// </summary>
        public const string ARTICY_ID       = "Articy Id";

        /// <summary>
        /// Dialogue title, unused
        /// </summary>
        public const string TITLE           = "Title";

        /// <summary>
        /// Dialogue text
        /// </summary>
        public const string DIALOGUE_TEXT   = "Dialogue Text";

        /// <summary>
        /// Actor ID
        /// </summary>
        public const string ACTOR = "Actor";

        /// <summary>
        /// Conversant ID
        /// </summary>
        public const string CONVERSANT      = "Conversant";

        /// <summary>
        /// White check difficulty Articy ID
        /// </summary>
        public const string DIFFICULTYWHITE = "DifficultyWhite";

        /// <summary>
        /// Red check difficulty Articy ID
        /// </summary>
        public const string DIFFICULTYRED   = "DifficultyRed";

        /// <summary>
        /// Active check skill type Articy ID
        /// </summary>
        public const string SKILLTYPE       = "SkillType";

        /// <summary>
        /// Passive check difficulty Articy ID
        /// </summary>
        public const string DIFFICULTYPASS = "DifficultyPass";

        public static string[] TOOLTIPS = { "tooltip1", "tooltip2", "tooltip3", "tooltip4" };
        public static string[] MODIFIERS = { "modifier1", "modifier2", "modifier3", "modifier4" };
    }
}
