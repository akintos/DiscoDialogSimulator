using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoDialogSimulator.DialogueSystem
{
    public static class ArticyBridge
    {
		public static int GetDifficultyValue(int articyDifficultyId)
		{
			return (int)ArticyDifficultyIdToDifficulty[articyDifficultyId];
		}

        public static readonly Difficulty[] ArticyDifficultyIdToDifficulty = new Difficulty[]
		{
			Difficulty.EXTRATRIVIAL,
			Difficulty.VERYEASY,
			Difficulty.AVERAGE,
			Difficulty.CHALLENGING,
			Difficulty.LEGENDARY,
			Difficulty.GODLY,
			Difficulty.IMPOSSIBRU3,
			Difficulty.IMPOSSIBRU5,
			Difficulty.TRIVIAL,
			Difficulty.EASY,
			Difficulty.MEDIUM,
			Difficulty.FORMIDABLE,
			Difficulty.HEROIC,
			Difficulty.IMPOSSIBRU2,
			Difficulty.IMPOSSIBRU4
		};

		public static readonly Dictionary<string, string> ArticyIdToSkillName = new Dictionary<string, string>
		{
			{"0x0000000000000000", "Unspecified skill"},
			{"0x0100000A00000042", "Authority"},
			{"0x0100000A0000003A", "Composure"},
			{"0x0100000400000918", "Conceptualization"},
			{"0x01000011000010D8", "Half Light"},
			{"0x0100000A0000001A", "Drama"},
			{"0x0100000A00000026", "Electrochemistry"},
			{"0x0100000400000773", "Empathy"},
			{"0x01000004000009A7", "Endurance"},
			{"0x0100000A0000004A", "Esprit de Corps"},
			{"0x0100000A0000002A", "Hand/Eye Coordination"},
			{"0x010000040000076F", "Inland Empire"},
			{"0x0100000A00000036", "Interfacing"},
			{"0x0100000400000767", "Logic"},
			{"0x0100000A00000022", "Pain Threshold"},
			{"0x0100000400000BC3", "Perception"},
			{"0x0100000800000BB0", "Perception (Hearing)"},
			{"0x0100000800000BBC", "Perception (Sight)"},
			{"0x0100000800000BAC", "Perception (Smell)"},
			{"0x0100000800000BB8", "Perception (Taste)"},
			{"0x0100000400000B11", "Physical Instrument"},
			{"0x0100000A0000002E", "Reaction Speed"},
			{"0x0100000A00000016", "Rhetoric"},
			{"0x0100000A00000032", "Savoir Faire"},
			{"0x0100000400000BC7", "Shivers"},
			{"0x0100000A00000046", "Suggestion"},
			{"0x010000040000076B", "Encyclopedia"},
			{"0x0100000A0000001E", "Visual Calculus"},
			{"0x0100000A0000003E", "Volition"}
		};

		public static readonly Dictionary<string, SkillType> ArticyIdToSkillType = new Dictionary<string, SkillType>()
		{
			{ "0x0100000400000767", SkillType.LOGIC },
			{ "0x010000040000076B", SkillType.ENCYCLOPEDIA },
			{ "0x0100000A00000016", SkillType.RHETORIC },
			{ "0x0100000A0000001A", SkillType.DRAMA },
			{ "0x0100000400000918", SkillType.CONCEPTUALIZATION },
			{ "0x0100000A0000001E", SkillType.VISUAL_CALCULUS },
			{ "0x0100000A0000003E", SkillType.VOLITION },
			{ "0x010000040000076F", SkillType.INLAND_EMPIRE },
			{ "0x0100000400000773", SkillType.EMPATHY },
			{ "0x0100000A00000042", SkillType.AUTHORITY },
			{ "0x0100000A00000046", SkillType.SUGGESTION },
			{ "0x0100000A0000004A", SkillType.ESPRIT_DE_CORPS },
			{ "0x0100000400000B11", SkillType.PHYSICAL_INSTRUMENT },
			{ "0x0100000A00000026", SkillType.ELECTROCHEMISTRY },
			{ "0x01000004000009A7", SkillType.ENDURANCE },
			{ "0x01000011000010D8", SkillType.HALF_LIGHT },
			{ "0x0100000A00000022", SkillType.PAIN_THRESHOLD },
			{ "0x0100000400000BC7", SkillType.SHIVERS },
			{ "0x0100000A0000002A", SkillType.HE_COORDINATION },
			{ "0x0100000400000BC3", SkillType.PERCEPTION },
			{ "0x0100000800000BB0", SkillType.HEARING },
			{ "0x0100000800000BBC", SkillType.SIGHT },
			{ "0x0100000800000BAC", SkillType.SMELL },
			{ "0x0100000800000BB8", SkillType.TASTE },
			{ "0x0100000A0000002E", SkillType.REACTION },
			{ "0x0100000A00000032", SkillType.SAVOIR_FAIRE },
			{ "0x0100000A00000036", SkillType.INTERFACING },
			{ "0x0100000A0000003A", SkillType.COMPOSURE }
		};
    }
}
