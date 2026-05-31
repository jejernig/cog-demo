using UnityEngine;
using TMPro;

namespace Cog
{
    /// <summary>Displays NPC dialogue in the HUD.</summary>
    public class DialogueUI : MonoBehaviour
    {
        public GameObject dialoguePanel;
        public TMP_Text npcNameText;
        public TMP_Text dialogueText;

        private void Start()
        {
            if (dialoguePanel == null)
            {
                dialoguePanel = new GameObject("DialoguePanel");
                dialoguePanel.transform.SetParent(transform);
                var panelRect = dialoguePanel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0);
                panelRect.anchorMax = new Vector2(0.5f, 0);
                panelRect.pivot = new Vector2(0.5f, 0);
                panelRect.sizeDelta = new Vector2(600, 100);
                panelRect.anchoredPosition = new Vector2(0, 20);

                // Background
                var panelImg = dialoguePanel.AddComponent<UnityEngine.UI.Image>();
                panelImg.color = new Color(0, 0, 0, 0.8f);

                npcNameText = CreateTMPText("NPCName", dialoguePanel.transform, 18, new Vector2(5, -5));
                dialogueText = CreateTMPText("DialogueText", dialoguePanel.transform, 14, new Vector2(5, -30));
            }

            // Hook into NPC events
            var npcs = FindObjectsByType<CogNPC>(FindObjectsSortMode.None);
            foreach (var npc in npcs)
            {
                npc.OnAgentAction.AddListener(OnNpcSpoke);
            }
        }

        TMP_Text CreateTMPText(string name, Transform parent, int fontSize, Vector2 pos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            var rt = go.AddComponent<RectTransform>();
            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(590, 30);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.color = Color.white;
            return text;
        }

        void OnNpcSpoke(AgentEvent evt)
        {
            npcNameText.text = $"<b>{evt.npcName}</b>";
            dialogueText.text = evt.text;
            CancelInvoke(nameof(ClearDialogue));
            Invoke(nameof(ClearDialogue), 8f);
        }

        void ClearDialogue()
        {
            npcNameText.text = "";
            dialogueText.text = "";
        }
    }
}
