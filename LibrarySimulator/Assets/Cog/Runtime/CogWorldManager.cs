using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Cog
{
    /// <summary>
    /// Singleton coordinator for all Cog NPCs in a scene.
    /// One per scene — manages the native engine lifecycle.
    /// </summary>
    public class CogWorldManager : MonoBehaviour
    {
        public static CogWorldManager Instance { get; private set; }

        [Header("Engine Config")]
        public string apiKey = "";
        public string modelTier = "budget";  // budget | standard | premium

        [Header("Debug")]
        [SerializeField] private int registeredNpcCount;
        [SerializeField] private int totalDecisions;

        private readonly Dictionary<string, CogNPC> npcs = new();
        private readonly List<Observation> worldEvents = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEngine();
        }

        private void InitializeEngine()
        {
            // v0.5: stub — in production, calls cog_init() via DllImport
            // var config = $"{{\"api_key\":\"{apiKey}\",\"model\":\"{modelTier}\"}}";
            // IntPtr ctx = cog_init(config);
            Debug.Log($"[Cog] WorldManager initialized. Model: {modelTier}");
        }

        private void OnDestroy()
        {
            // cog_shutdown(ctx);
        }

        /// <summary>Register an NPC with the world.</summary>
        public void RegisterNpc(CogNPC npc)
        {
            npcs[npc.npcId] = npc;
            registeredNpcCount = npcs.Count;
            Debug.Log($"[Cog] NPC registered: {npc.profile?.npcName ?? npc.npcId} ({registeredNpcCount} total)");
        }

        /// <summary>Unregister an NPC.</summary>
        public void UnregisterNpc(CogNPC npc)
        {
            npcs.Remove(npc.npcId);
            registeredNpcCount = npcs.Count;
        }

        /// <summary>Log a world event. Distributed to all NPCs within range.</summary>
        public void LogEvent(string content, ObservationCategory category, Vector3 position)
        {
            worldEvents.Add(new Observation(content, category));

            foreach (var npc in npcs.Values)
            {
                if (npc.autoObserve && Vector3.Distance(npc.transform.position, position) <= npc.observationRadius)
                {
                    npc.Observe(content, category);
                }
            }
        }

        /// <summary>Request an LLM-backed decision for an NPC. Returns async task.</summary>
        public async Task<AgentEvent> RequestDecisionAsync(string npcId, CogPersonalityProfile profile, string context)
        {
            totalDecisions++;

            // v0.5: calls native engine via FFI
            // For now, returns a placeholder — real impl calls cog_tick()
            await Task.Delay(100); // simulate async work

            return new AgentEvent
            {
                npcId = npcId,
                npcName = profile?.npcName ?? npcId,
                text = $"[{npcId}] Observed: {context[..Mathf.Min(context.Length, 80)]}...",
                emotion = "neutral",
                targetNpcId = null,
                intensity = 1f
            };
        }

        /// <summary>Get all registered NPCs.</summary>
        public IReadOnlyDictionary<string, CogNPC> GetNpcs() => npcs;

        /// <summary>Get world event log.</summary>
        public IReadOnlyList<Observation> GetWorldEvents() => worldEvents;
    }
}
