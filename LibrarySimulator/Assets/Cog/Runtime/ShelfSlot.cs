using UnityEngine;

namespace Cog
{
    /// <summary>A shelf where books can be placed.</summary>
    public class ShelfSlot : MonoBehaviour
    {
        public string shelfLabel = "Uncategorized";
        private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        public void Highlight(bool on)
        {
            if (rend != null)
                rend.material.color = on ? Color.yellow : Color.white;
        }
    }
}
