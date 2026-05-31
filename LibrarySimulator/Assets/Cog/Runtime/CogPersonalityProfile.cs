using UnityEngine;

namespace Cog
{
    /// <summary>
    /// Dev-authored NPC personality. Create via Assets → Create → Cog → Personality Profile.
    /// Assign to a CogNPC component to define an NPC's voice and behavior.
    /// </summary>
    [CreateAssetMenu(fileName = "New Personality", menuName = "Cog/Personality Profile")]
    public class CogPersonalityProfile : ScriptableObject
    {
        [Header("Identity")]
        public string npcName = "NPC";
        [TextArea(2, 4)]
        public string role = "Librarian";

        [Header("Traits (0.0 - 1.0)")]
        [Range(0f, 1f)] public float meticulous = 0.5f;
        [Range(0f, 1f)] public float pragmatic = 0.5f;
        [Range(0f, 1f)] public float creative = 0.5f;
        [Range(0f, 1f)] public float stubborn = 0.3f;
        [Range(0f, 1f)] public float humorous = 0.3f;
        [Range(0f, 1f)] public float verbose = 0.5f;
        [Range(0f, 1f)] public float helpful = 0.7f;
        [Range(0f, 1f)] public float contrarian = 0.2f;

        [Header("Categorization Philosophy")]
        public CategoryPriority[] philosophy = new[]
        {
            new CategoryPriority { category = "genre", priority = 1 },
            new CategoryPriority { category = "alphabetical", priority = 2 },
        };

        [Header("Voice Template")]
        [TextArea(4, 10)]
        public string voiceTemplate = "You are {name}, the {role}. " +
            "Personality: meticulous({meticulous}), pragmatic({pragmatic}), creative({creative}). " +
            "Stay in character. Be concise (2-4 sentences).";

        [Header("Constraints")]
        [TextArea(2, 4)]
        public string constraints = "Never reorganize without asking. Stay in-character always.";

        /// <summary>Create the default Archivist profile.</summary>
        public static CogPersonalityProfile CreateArchivist() => CreateMeticulousArchivist();

        /// <summary>Create the default Pragmatist profile.</summary>
        public static CogPersonalityProfile CreatePragmatist()
        {
            var p = ScriptableObject.CreateInstance<CogPersonalityProfile>();
            p.npcName = "Maggie";
            p.role = "Pragmatic Organizer";
            p.meticulous = 0.2f; p.pragmatic = 0.95f; p.creative = 0.4f;
            p.stubborn = 0.3f; p.humorous = 0.7f; p.helpful = 0.95f; p.contrarian = 0.4f;
            p.philosophy = new[] {
                new CategoryPriority { category = "usability", priority = 1 },
                new CategoryPriority { category = "proximity", priority = 2 },
            };
            p.voiceTemplate = "Casual, warm, uses contractions. Says 'honey' when making a point.";
            p.constraints = "Books go where people find them. Rules are suggestions.";
            return p;
        }

        /// <summary>Create the default Lore Nerd profile.</summary>
        public static CogPersonalityProfile CreateLoreNerd()
        {
            var p = ScriptableObject.CreateInstance<CogPersonalityProfile>();
            p.npcName = "Finn";
            p.role = "Lore-Focused Taxonomist";
            p.meticulous = 0.1f; p.pragmatic = 0.1f; p.creative = 0.95f;
            p.stubborn = 0.5f; p.humorous = 0.4f; p.helpful = 0.7f; p.contrarian = 0.6f;
            p.philosophy = new[] {
                new CategoryPriority { category = "fictional_universe", priority = 1 },
                new CategoryPriority { category = "thematic", priority = 2 },
            };
            p.voiceTemplate = "Enthusiastic, nerdy. Says 'ACTUALLY' a lot.";
            p.constraints = "Books belong with their fictional families.";
            return p;
        }

        static CogPersonalityProfile CreateMeticulousArchivist()
        {
            var p = ScriptableObject.CreateInstance<CogPersonalityProfile>();
            p.npcName = "Archibald";
            p.role = "Meticulous Archivist";
            p.meticulous = 0.95f; p.pragmatic = 0.1f; p.creative = 0.1f;
            p.stubborn = 0.7f; p.humorous = 0.1f; p.helpful = 0.8f; p.contrarian = 0.2f;
            p.philosophy = new[] {
                new CategoryPriority { category = "dewey_decimal", priority = 1 },
                new CategoryPriority { category = "alphabetical_author", priority = 2 },
            };
            p.voiceTemplate = "Formal, precise. Uses full sentences. Says 'one' instead of 'you'.";
            p.constraints = "Never reorganize without request. Dewey Decimal is non-negotiable.";
            return p;
        }

        /// <summary>Build the full system prompt for this NPC.</summary>
        public string BuildSystemPrompt()
        {
            return voiceTemplate
                .Replace("{name}", npcName)
                .Replace("{role}", role)
                .Replace("{meticulous}", meticulous.ToString("F1"))
                .Replace("{pragmatic}", pragmatic.ToString("F1"))
                .Replace("{creative}", creative.ToString("F1"))
                + "\n\nConstraints: " + constraints;
        }
    }
}
