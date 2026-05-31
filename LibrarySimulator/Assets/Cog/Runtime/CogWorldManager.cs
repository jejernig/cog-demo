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

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogWarning("[Cog] No API key set. Using stub responses. Set apiKey in CogWorldManager inspector.");
                return StubResponse(npcId, profile);
            }

            try
            {
                var systemPrompt = profile != null ? profile.BuildSystemPrompt() 
                    : $"You are {npcId}, an AI coworker in a library. Be helpful and concise.";

                var userPrompt = $"Recent observations:\n{context}\n\n" +
                    "Based on what you've observed, should you speak or act? " +
                    "Respond with JSON: {\"text\": \"what you say\", \"emotion\": \"neutral|happy|annoyed|confused|excited\", \"intensity\": 1-5}";

                var requestBody = JsonUtility.ToJson(new LlmRequest
                {
                    model = modelTier switch { "premium" => "gpt-4o", _ => "gpt-4.1-nano" },
                    messages = new[] {
                        new LlmMessage { role = "system", content = systemPrompt },
                        new LlmMessage { role = "user", content = userPrompt }
                    },
                    max_tokens = 150,
                    temperature = 0.7f
                });

                using var www = UnityEngine.Networking.UnityWebRequest.Post(
                    "https://api.openai.com/v1/chat/completions", requestBody, "application/json");
                www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                www.timeout = 15;

                var op = www.SendWebRequest();
                while (!op.isDone) await Task.Yield();

                if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[Cog] LLM API error: {www.error}");
                    return StubResponse(npcId, profile);
                }

                var response = JsonUtility.FromJson<LlmResponse>(www.downloadHandler.text);
                var content = response.choices?[0]?.message?.content ?? "";

                // Parse JSON from LLM response
                try { return JsonUtility.FromJson<LlmAgentEvent>(content).ToAgentEvent(npcId, profile?.npcName ?? npcId); }
                catch { /* fall through to raw text */ }

                return new AgentEvent
                {
                    npcId = npcId,
                    npcName = profile?.npcName ?? npcId,
                    text = content.Trim(),
                    emotion = "neutral",
                    targetNpcId = null,
                    intensity = 2
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Cog] LLM exception: {e.Message}");
                return StubResponse(npcId, profile);
            }
        }

        AgentEvent StubResponse(string npcId, CogPersonalityProfile profile)
        {
            return new AgentEvent
            {
                npcId = npcId,
                npcName = profile?.npcName ?? npcId,
                text = npcId switch
                {
                    "archibald" => "One must consider the Dewey Decimal classification for this volume.",
                    "maggie" => "Honey, that book belongs where folks will actually find it.",
                    "finn" => "ACTUALLY, this is part of a larger literary universe!",
                    _ => "Interesting choice of placement."
                },
                emotion = "neutral",
                targetNpcId = null,
                intensity = 2
            };
        }

        [System.Serializable] class LlmRequest { public string model; public LlmMessage[] messages; public int max_tokens; public float temperature; }
        [System.Serializable] class LlmMessage { public string role; public string content; }
        [System.Serializable] class LlmResponse { public LlmChoice[] choices; }
        [System.Serializable] class LlmChoice { public LlmMessage message; }
        [System.Serializable] class LlmAgentEvent { public string text; public string emotion; public int intensity;
            public AgentEvent ToAgentEvent(string id, string name) => new() { npcId=id, npcName=name, text=text, emotion=emotion??"neutral", intensity=intensity }; }

        /// <summary>Get all registered NPCs.</summary>
        public IReadOnlyDictionary<string, CogNPC> GetNpcs() => npcs;

        /// <summary>Get world event log.</summary>
        public IReadOnlyList<Observation> GetWorldEvents() => worldEvents;
    }
}
