using UnityEngine;

namespace Cog
{
    /// <summary>A book that can be grabbed and placed on shelves.</summary>
    public class Book : MonoBehaviour
    {
        public string title = "Unknown Book";
        public string author = "Unknown Author";
        public string genre = "Uncategorized";

        private void Start()
        {
            // Add physics
            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.drag = 2f;

            var col = GetComponent<Collider>();
            if (col == null)
            {
                var box = gameObject.AddComponent<BoxCollider>();
                box.size = new Vector3(0.15f, 0.22f, 0.04f);
            }

            // Random color for visual variety
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Random.ColorHSV(0f, 1f, 0.3f, 0.8f, 0.4f, 0.9f);
            }
        }

        public override string ToString() => $"'{title}' by {author} [{genre}]";
    }
}
