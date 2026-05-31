using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Cog
{
    /// <summary>
    /// Attach to any GameObject to create an AI-powered NPC coworker.
    /// NPCs observe the world, make decisions, and emit AgentEvents.
    /// Requires a CogWorldManager in the scene.
    /// </summary>
    public class CogNPC : MonoBehaviour
    {
        [Header("Identity")]
        public string npcId;  // Auto-generated if empty
        public CogPersonalityProfile profile;

        [Header("Behavior")]
        public bool autoObserve = true;
        [Range(1f, 30f)] public float thinkCooldown = 5f;
        [Range(0f, 10f)] public float observationRadius = 5f;

        [Header("State (read-only)")]
        [SerializeField] private bool isThinking;
        [SerializeField] private string lastAction;

        // Events
        public UnityEvent<AgentEvent> OnAgentAction;

        // Internal
        private CogWorldManager world;
        private float lastThinkTime;
        private readonly Queue<string> pendingEvents = new();

        private void Start()
        {
            if (string.IsNullOrEmpty(npcId))
                npcId = Guid.NewGuid().ToString("N")[..8];

            world = CogWorldManager.Instance;
            world.RegisterNpc(this);
        }

        private void OnDestroy()
        {
            world?.UnregisterNpc(this);
        }

        private void Update()
        {
            if (!isThinking && Time.time - lastThinkTime >= thinkCooldown && pendingEvents.Count > 0)
            {
                StartCoroutine(ThinkCoroutine());
            }
        }

        /// <summary>Feed an observation to this NPC.</summary>
        public void Observe(string content, ObservationCategory category = ObservationCategory.PlayerAction)
        {
            pendingEvents.Enqueue(content);
        }

        /// <summary>Async decision cycle. Non-blocking — uses coroutine.</summary>
        private IEnumerator ThinkCoroutine()
        {
            isThinking = true;

            // Collect pending events
            var events = new List<string>();
            while (pendingEvents.Count > 0)
                events.Add(pendingEvents.Dequeue());

            // Build context
            var context = string.Join("\n", events);

            // Request decision from the engine (native call, async)
            var task = world.RequestDecisionAsync(npcId, profile, context);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsCompletedSuccessfully)
            {
                lastAction = task.Result.text;
                OnAgentAction?.Invoke(task.Result);
            }

            lastThinkTime = Time.time;
            isThinking = false;
        }
    }
}
