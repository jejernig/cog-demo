using NUnit.Framework;
using UnityEngine;
using System.IO;
using Cog;

namespace Cog.Tests
{
    public class SaveSystemTests
    {
        private string testPath;

        [SetUp]
        public void SetUp()
        {
            testPath = Path.Combine(Application.temporaryCachePath, "cog_test_save.json");
            if (File.Exists(testPath)) File.Delete(testPath);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testPath)) File.Delete(testPath);
        }

        [Test]
        public void SaveData_Serialization()
        {
            var data = new SaveSystem.SaveData
            {
                version = "0.5.0",
                timestamp = "2026-05-31T12:00:00Z",
                playerActions = 42,
                books = new[] {
                    new SaveSystem.BookPlacement {
                        title = "The Hobbit",
                        shelfLabel = "Fantasy",
                        position = new[] { 1f, 2f, 3f },
                        rotation = new[] { 0f, 0f, 0f, 1f }
                    }
                },
                npcs = new[] {
                    new SaveSystem.NpcState {
                        npcId = "archibald",
                        observations = 10,
                        lastAction = "Categorized The Hobbit as Fantasy"
                    }
                }
            };

            var json = JsonUtility.ToJson(data, true);
            Assert.IsTrue(json.Contains("The Hobbit"));
            Assert.IsTrue(json.Contains("Fantasy"));
            Assert.IsTrue(json.Contains("0.5.0"));
            Assert.IsTrue(json.Contains("archibald"));
        }

        [Test]
        public void SaveData_Deserialization()
        {
            var json = @"{
                ""version"": ""0.5.0"",
                ""timestamp"": ""2026-05-31T12:00:00Z"",
                ""playerActions"": 7,
                ""books"": [{""title"": ""Dune"",""shelfLabel"": ""Sci-Fi"",""position"":[0,0,0],""rotation"":[0,0,0,1]}],
                ""npcs"": [{""npcId"":""maggie"",""observations"":3,""lastAction"":""Suggested cookbook placement""}]
            }";

            var data = JsonUtility.FromJson<SaveSystem.SaveData>(json);
            Assert.AreEqual("0.5.0", data.version);
            Assert.AreEqual(7, data.playerActions);
            Assert.AreEqual(1, data.books.Length);
            Assert.AreEqual("Dune", data.books[0].title);
            Assert.AreEqual("Sci-Fi", data.books[0].shelfLabel);
            Assert.AreEqual("maggie", data.npcs[0].npcId);
        }

        [Test]
        public void SaveData_HandlesEmptyArrays()
        {
            var data = new SaveSystem.SaveData { books = new SaveSystem.BookPlacement[0], npcs = new SaveSystem.NpcState[0] };
            var json = JsonUtility.ToJson(data);
            var restored = JsonUtility.FromJson<SaveSystem.SaveData>(json);
            Assert.IsNotNull(restored.books);
            Assert.IsNotNull(restored.npcs);
            Assert.AreEqual(0, restored.books.Length);
        }
    }
}
