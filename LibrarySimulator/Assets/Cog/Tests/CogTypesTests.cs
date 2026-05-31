using NUnit.Framework;
using Cog;

namespace Cog.Tests
{
    public class CogTypesTests
    {
        [Test]
        public void Observation_HasTimestamp()
        {
            var obs = new Observation("test", ObservationCategory.PlayerAction);
            Assert.IsTrue(obs.timestamp > 0);
            Assert.AreEqual("test", obs.content);
            Assert.AreEqual(ObservationCategory.PlayerAction, obs.category);
        }

        [Test]
        public void RelationshipWeights_DefaultValues()
        {
            var w = RelationshipWeights.Default;
            Assert.AreEqual(0.5f, w.trust);
            Assert.AreEqual(0.5f, w.respect);
            Assert.AreEqual(0.3f, w.affection);
            Assert.AreEqual(0.1f, w.annoyance);
        }

        [Test]
        public void AgentEvent_StructProperties()
        {
            var evt = new AgentEvent
            {
                npcId = "test_npc",
                npcName = "Test NPC",
                text = "Hello world",
                emotion = "happy",
                intensity = 3
            };
            Assert.AreEqual("test_npc", evt.npcId);
            Assert.AreEqual("Test NPC", evt.npcName);
            Assert.AreEqual("Hello world", evt.text);
            Assert.AreEqual("happy", evt.emotion);
            Assert.AreEqual(3, evt.intensity);
        }
    }
}
