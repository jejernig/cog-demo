using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cog;

namespace Cog.Editor
{
    public static class LibrarySimulatorBuilder
    {
        [MenuItem("Cog/Build Library Simulator Scene")]
        public static void BuildScene()
        {
            Debug.Log("[Cog] Building Library Simulator scene...");

            // World Manager
            var worldGo = new GameObject("CogWorldManager");
            worldGo.AddComponent<CogWorldManager>();

            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(5, 1, 5);

            // Bookshelves
            var shelves = new[] {
                ("Fantasy", new Vector3(-3, 0, 2)),
                ("Sci-Fi", new Vector3(0, 0, 2)),
                ("Horror", new Vector3(3, 0, 2)),
                ("Nonfiction", new Vector3(-3, 0, -2)),
                ("Cooking", new Vector3(0, 0, -2)),
                ("Uncategorized", new Vector3(3, 0, -2)),
            };
            foreach (var (label, pos) in shelves)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"Shelf_{label}";
                go.transform.position = pos;
                go.transform.localScale = new Vector3(1.5f, 3, 0.3f);
                go.GetComponent<Renderer>().material.color = new Color(0.4f, 0.25f, 0.15f);
                var shelf = go.AddComponent<ShelfSlot>();
                shelf.shelfLabel = label;
            }

            // NPCs
            CreateNpc(new Vector3(-4, 1, 4), "Archibald", new Color(0.3f, 0.3f, 0.7f), CogPersonalityProfile.CreateArchivist());
            CreateNpc(new Vector3(0, 1, 4), "Maggie", new Color(0.8f, 0.35f, 0.3f), CogPersonalityProfile.CreatePragmatist());
            CreateNpc(new Vector3(4, 1, 4), "Finn", new Color(0.3f, 0.7f, 0.3f), CogPersonalityProfile.CreateLoreNerd());

            // Player
            var player = new GameObject("Player");
            player.transform.position = new Vector3(0, 1.5f, -4);
            var cam = player.AddComponent<Camera>();
            cam.tag = "MainCamera";
            player.AddComponent<PlayerController>();

            // Light
            var light = new GameObject("Light");
            var l = light.AddComponent<Light>();
            l.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);

            // UI Canvas
            var canvasGo = new GameObject("UICanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            canvasGo.AddComponent<DialogueUI>();

            // Book generator
            var bookGen = new GameObject("BookGenerator");
            bookGen.AddComponent<BookGenerator>();

            // Save System
            var saveGo = new GameObject("SaveSystem");
            saveGo.AddComponent<SaveSystem>();

            Debug.Log("[Cog] Scene built! Press Play.");
        }

        static void CreateNpc(Vector3 pos, string name, Color color, CogPersonalityProfile profile)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = $"NPC_{name}";
            go.transform.position = pos;
            go.transform.localScale = new Vector3(0.5f, 0.8f, 0.5f);
            go.GetComponent<Renderer>().material.color = color;

            var npc = go.AddComponent<CogNPC>();
            npc.profile = profile;
            npc.npcId = name.ToLower();
        }
    }
}
