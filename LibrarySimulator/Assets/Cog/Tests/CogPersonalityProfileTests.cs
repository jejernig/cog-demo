using NUnit.Framework;
using UnityEngine;
using Cog;

namespace Cog.Tests
{
    public class CogPersonalityProfileTests
    {
        [Test]
        public void BuildSystemPrompt_ContainsNameAndTraits()
        {
            var profile = CogPersonalityProfile.CreateArchivist();
            Assert.IsNotNull(profile);
            
            var prompt = profile.BuildSystemPrompt();
            Assert.IsTrue(prompt.Contains("Archibald"));
            Assert.IsTrue(prompt.Contains("Meticulous Archivist"));
            Assert.IsTrue(prompt.Contains("meticulous"));
        }

        [Test]
        public void ThreeProfiles_AreDistinct()
        {
            var arch = CogPersonalityProfile.CreateArchivist().BuildSystemPrompt();
            var prag = CogPersonalityProfile.CreatePragmatist().BuildSystemPrompt();
            var lore = CogPersonalityProfile.CreateLoreNerd().BuildSystemPrompt();

            Assert.AreNotEqual(arch, prag);
            Assert.AreNotEqual(prag, lore);
            Assert.AreNotEqual(arch, lore);
        }

        [Test]
        public void Archivist_HasDeweyDecimalPhilosophy()
        {
            var profile = CogPersonalityProfile.CreateArchivist();
            Assert.IsTrue(profile.philosophy.Length > 0);
            Assert.AreEqual("dewey_decimal", profile.philosophy[0].category);
            Assert.IsTrue(profile.meticulous > 0.9f);
        }

        [Test]
        public void Pragmatist_HasUsabilityPhilosophy()
        {
            var profile = CogPersonalityProfile.CreatePragmatist();
            Assert.AreEqual("usability", profile.philosophy[0].category);
            Assert.IsTrue(profile.pragmatic > 0.9f);
            Assert.IsTrue(profile.humorous > 0.5f);
        }

        [Test]
        public void LoreNerd_HasUniversePhilosophy()
        {
            var profile = CogPersonalityProfile.CreateLoreNerd();
            Assert.AreEqual("fictional_universe", profile.philosophy[0].category);
            Assert.IsTrue(profile.creative > 0.9f);
        }

        [Test]
        public void BuildSystemPrompt_NoNullReferences()
        {
            // Verify all factory methods produce valid prompts
            foreach (var profile in new[] {
                CogPersonalityProfile.CreateArchivist(),
                CogPersonalityProfile.CreatePragmatist(),
                CogPersonalityProfile.CreateLoreNerd()
            })
            {
                var prompt = profile.BuildSystemPrompt();
                Assert.IsNotNull(prompt);
                Assert.IsNotEmpty(prompt);
                Assert.IsFalse(prompt.Contains("{name}"), "Template placeholder was not replaced");
                Assert.IsFalse(prompt.Contains("{role}"), "Template placeholder was not replaced");
            }
        }
    }
}
