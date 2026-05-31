using System;
using System.Collections.Generic;

namespace Cog
{
    /// <summary>Observation categories for memory tiering.</summary>
    public enum ObservationCategory
    {
        PlayerAction,
        NpcComment,
        WorldEvent,
        NpcThought
    }

    /// <summary>An observation pushed into NPC memory.</summary>
    [Serializable]
    public struct Observation
    {
        public float timestamp;
        public string content;
        public ObservationCategory category;

        public Observation(string content, ObservationCategory category = ObservationCategory.PlayerAction)
        {
            this.timestamp = UnityEngine.Time.time;
            this.content = content;
            this.category = category;
        }
    }

    /// <summary>Output from an NPC decision cycle.</summary>
    [Serializable]
    public struct AgentEvent
    {
        public string npcId;
        public string npcName;
        public string text;
        public string emotion;       // neutral, happy, annoyed, confused, excited
        public string targetNpcId;   // null = player, NPC id = directed at another NPC
        public float intensity;      // 1-5
    }

    /// <summary>A single dimension of relationship between two NPCs.</summary>
    [Serializable]
    public struct RelationshipWeights
    {
        public float trust;
        public float respect;
        public float affection;
        public float annoyance;

        public static RelationshipWeights Default => new()
        {
            trust = 0.5f, respect = 0.5f, affection = 0.3f, annoyance = 0.1f
        };
    }

    /// <summary>Categorization philosophy entry.</summary>
    [Serializable]
    public struct CategoryPriority
    {
        public string category;  // e.g., "alphabetical", "genre", "color", "vibes"
        public int priority;     // 1 = highest
    }
}
