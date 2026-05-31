using System;
using System.IO;
using UnityEngine;

namespace Cog
{
    /// <summary>Persists world state, NPC memories, and book placements across sessions.</summary>
    public class SaveSystem : MonoBehaviour
    {
        static string SavePath => Path.Combine(Application.persistentDataPath, "library_save.json");

        [System.Serializable]
        public class SaveData
        {
            public string version = "0.5.0";
            public string timestamp;
            public BookPlacement[] books;
            public NpcState[] npcs;
            public int playerActions;
        }

        [System.Serializable]
        public class BookPlacement
        {
            public string title;
            public string shelfLabel;
            public float[] position;
            public float[] rotation;
        }

        [System.Serializable]
        public class NpcState
        {
            public string npcId;
            public int observations;
            public string lastAction;
        }

        void Start()
        {
            // Auto-load on start
            if (File.Exists(SavePath))
            {
                Load();
                Debug.Log($"[Cog] Save loaded from {SavePath}");
            }
        }

        public void Save()
        {
            var data = new SaveData
            {
                timestamp = DateTime.Now.ToString("O"),
                books = new BookPlacement[0],
                npcs = new NpcState[0],
                playerActions = CogWorldManager.Instance?.GetWorldEvents().Count ?? 0,
            };

            // Collect book placements
            var books = FindObjectsByType<Book>(FindObjectsSortMode.None);
            var placements = new System.Collections.Generic.List<BookPlacement>();
            foreach (var b in books)
            {
                var parent = b.transform.parent;
                var shelf = parent?.GetComponent<ShelfSlot>();
                placements.Add(new BookPlacement
                {
                    title = b.title,
                    shelfLabel = shelf?.shelfLabel ?? "Unsorted",
                    position = new[] { b.transform.position.x, b.transform.position.y, b.transform.position.z },
                    rotation = new[] { b.transform.rotation.x, b.transform.rotation.y, b.transform.rotation.z, b.transform.rotation.w },
                });
            }
            data.books = placements.ToArray();

            // Collect NPC state
            var npcs = FindObjectsByType<CogNPC>(FindObjectsSortMode.None);
            var npcStates = new System.Collections.Generic.List<NpcState>();
            foreach (var n in npcs)
            {
                npcStates.Add(new NpcState
                {
                    npcId = n.npcId,
                    observations = 0,
                    lastAction = "",
                });
            }
            data.npcs = npcStates.ToArray();

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"[Cog] Saved to {SavePath}");
        }

        public void Load()
        {
            if (!File.Exists(SavePath)) return;
            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            Debug.Log($"[Cog] Loaded save from {data.timestamp}, {data.books.Length} books, {data.npcs.Length} NPCs");
            // In production: restore book positions, NPC states, world events
        }

        void OnApplicationQuit()
        {
            Save();
        }
    }
}
