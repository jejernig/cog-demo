using System.Collections.Generic;
using UnityEngine;

namespace Cog
{
    /// <summary>Spawns book prefabs in the scene for the Library Simulator.</summary>
    public class BookGenerator : MonoBehaviour
    {
        public int bookCount = 20;
        public Vector3 spawnArea = new(0, 1.5f, -3);
        public float spawnRadius = 2f;

        // Book corpus — public domain + generated titles
        static readonly (string title, string author, string genre)[] Books = {
            ("The Hobbit", "J.R.R. Tolkien", "Fantasy"),
            ("Dune", "Frank Herbert", "Sci-Fi"),
            ("Frankenstein", "Mary Shelley", "Horror"),
            ("The Joy of Cooking", "Irma Rombauer", "Cooking"),
            ("Neuromancer", "William Gibson", "Sci-Fi"),
            ("Pride and Prejudice", "Jane Austen", "Romance"),
            ("The Art of War", "Sun Tzu", "Nonfiction"),
            ("Watchmen", "Alan Moore", "Graphic Novel"),
            ("The Name of the Rose", "Umberto Eco", "Mystery"),
            ("Sapiens", "Yuval Noah Harari", "Nonfiction"),
            ("Dracula", "Bram Stoker", "Horror"),
            ("Foundation", "Isaac Asimov", "Sci-Fi"),
            ("The Great Gatsby", "F. Scott Fitzgerald", "Fiction"),
            ("A Brief History of Time", "Stephen Hawking", "Nonfiction"),
            ("The Catcher in the Rye", "J.D. Salinger", "Fiction"),
            ("On the Origin of Species", "Charles Darwin", "Nonfiction"),
            ("The Lord of the Rings", "J.R.R. Tolkien", "Fantasy"),
            ("The Color of Magic", "Terry Pratchett", "Fantasy"),
            ("Mastering the Art of French Cooking", "Julia Child", "Cooking"),
            ("Cosmos", "Carl Sagan", "Nonfiction"),
        };

        void Start()
        {
            for (int i = 0; i < bookCount; i++)
            {
                var (title, author, genre) = Books[i % Books.Length];
                SpawnBook(title, author, genre,
                    spawnArea + new Vector3(
                        Random.Range(-spawnRadius, spawnRadius),
                        0,
                        Random.Range(-spawnRadius, spawnRadius)
                    ));
            }
        }

        void SpawnBook(string title, string author, string genre, Vector3 position)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = $"Book_{title.Replace(" ", "")}";
            go.transform.position = position;
            go.transform.localScale = new Vector3(0.12f, 0.2f, 0.03f);
            go.transform.rotation = Random.rotation;

            var book = go.AddComponent<Book>();
            book.title = title;
            book.author = author;
            book.genre = genre;
        }
    }
}
