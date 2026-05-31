using UnityEngine;

namespace Cog
{
    /// <summary>
    /// Simple first-person controller for the Library Simulator demo.
    /// WASD to move, mouse to look, E to grab/place books.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float lookSpeed = 2f;
        public float grabRange = 3f;

        private float yaw;
        private float pitch;
        private GameObject heldBook;
        private ShelfSlot highlightedShelf;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // Look
            yaw += Input.GetAxis("Mouse X") * lookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);

            // Move
            var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            transform.Translate(move * (moveSpeed * Time.deltaTime));

            // Interact (E key)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldBook != null)
                {
                    PlaceBook();
                }
                else
                {
                    GrabBook();
                }
            }

            // Highlight shelf
            HighlightShelf();
        }

        void GrabBook()
        {
            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, grabRange))
            {
                var book = hit.collider.GetComponent<Book>();
                if (book != null)
                {
                    heldBook = book.gameObject;
                    heldBook.GetComponent<Rigidbody>().isKinematic = true;
                    heldBook.transform.SetParent(transform);
                    heldBook.transform.localPosition = new Vector3(0.3f, -0.2f, 0.8f);

                    CogWorldManager.Instance.LogEvent(
                        $"Player picked up '{book.title}'",
                        ObservationCategory.PlayerAction,
                        transform.position
                    );
                }
            }
        }

        void PlaceBook()
        {
            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, grabRange))
            {
                var shelf = hit.collider.GetComponent<ShelfSlot>();
                if (shelf != null)
                {
                    var book = heldBook.GetComponent<Book>();
                    heldBook.transform.SetParent(shelf.transform);
                    heldBook.transform.localPosition = Vector3.zero;
                    heldBook.transform.localRotation = Quaternion.identity;
                    heldBook.GetComponent<Rigidbody>().isKinematic = false;

                    CogWorldManager.Instance.LogEvent(
                        $"Player placed '{book.title}' on {shelf.shelfLabel} shelf",
                        ObservationCategory.PlayerAction,
                        shelf.transform.position
                    );

                    heldBook = null;
                }
            }
        }

        void HighlightShelf()
        {
            var ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, grabRange) && heldBook != null)
            {
                var shelf = hit.collider.GetComponent<ShelfSlot>();
                if (shelf != highlightedShelf)
                {
                    if (highlightedShelf != null)
                        highlightedShelf.Highlight(false);
                    highlightedShelf = shelf;
                    if (highlightedShelf != null)
                        highlightedShelf.Highlight(true);
                }
            }
            else if (highlightedShelf != null)
            {
                highlightedShelf.Highlight(false);
                highlightedShelf = null;
            }
        }
    }
}
