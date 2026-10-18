using System;
using System.Reflection;
using NUnit.Framework;
using Sim.Dispositio.Shared;
using UnityEditor;
using UnityEngine;

namespace Sim.Utility.Tests
{
    public sealed class SerializableGuidTests
    {
        private const string FirstGuid = "12345678-1234-5678-9abc-1234567890ab";
        private const string SecondGuid = "abcdef01-2345-6789-abcd-0123456789ab";

        [Test]
        public void DefaultConstructorRepresentsEmptyGuid()
        {
            Assert.That(new SerializableGuid().Instance, Is.EqualTo(Guid.Empty));
        }

        [TestCase(null)]
        [TestCase("")]
        public void EmptySerializedStringRepresentsEmptyGuid(string value)
        {
            Assert.That(new SerializableGuid(value).Instance, Is.EqualTo(Guid.Empty));
        }

        [Test]
        public void GuidConstructorPreservesValueAndEquality()
        {
            Guid expected = Guid.Parse(FirstGuid);
            var actual = new SerializableGuid(expected);

            Assert.That(actual.Instance, Is.EqualTo(expected));
            Assert.That(actual.ToString(), Is.EqualTo(FirstGuid));
            Assert.That(actual.GetHashCode(), Is.EqualTo(expected.GetHashCode()));
            Assert.That(actual.Equals(new SerializableGuid(FirstGuid)), Is.True);
        }

        [Test]
        public void SetGuidInvalidatesTheParsedValue()
        {
            var value = new SerializableGuid(FirstGuid);
            Assert.That(value.Instance, Is.EqualTo(Guid.Parse(FirstGuid)));

            value.SetGuid(Guid.Parse(SecondGuid));

            Assert.That(value.Instance, Is.EqualTo(Guid.Parse(SecondGuid)));
        }

        [Test]
        public void CopyConstructorPreservesIndependentSerializedValue()
        {
            var original = new SerializableGuid(FirstGuid);
            var copy = new SerializableGuid(original);
            original.SetGuid(Guid.Parse(SecondGuid));

            Assert.That(copy.Instance, Is.EqualTo(Guid.Parse(FirstGuid)));
        }

        [Test]
        public void NullCopyIsRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new SerializableGuid((SerializableGuid)null));
        }

        [Test]
        public void MalformedStringStillFailsWhenRead()
        {
            var value = new SerializableGuid("not-a-guid");

            Assert.Throws<FormatException>(() => _ = value.Instance);
        }

        [Test]
        public void JsonRetainsTheOriginalFieldNameAndExcludesParsedCaches()
        {
            var value = new SerializableGuid(FirstGuid);
            _ = value.Instance;
            _ = value.GetHashCode();

            string json = JsonUtility.ToJson(value);

            Assert.That(json, Is.EqualTo("{\"_guidString\":\"" + FirstGuid + "\"}"));
            Assert.That(JsonUtility.FromJson<SerializableGuid>(json).Instance, Is.EqualTo(Guid.Parse(FirstGuid)));
            FieldInfo field = typeof(SerializableGuid).GetField("_guidString", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.GetCustomAttribute<SerializeField>(), Is.Not.Null);
        }

        [Test]
        public void JsonPreservesTheStoredStringFormat()
        {
            string upperCase = FirstGuid.ToUpperInvariant();
            var value = new SerializableGuid(upperCase);

            Assert.That(JsonUtility.ToJson(value), Is.EqualTo("{\"_guidString\":\"" + upperCase + "\"}"));
        }

        [Test]
        public void SerializedPropertyConstructorAndGenericReadRemainAvailableInEditor()
        {
            GuidHolder holder = ScriptableObject.CreateInstance<GuidHolder>();
            try
            {
                holder.Value = new SerializableGuid(FirstGuid);
                using (var serialized = new SerializedObject(holder))
                {
                    SerializedProperty property = serialized.FindProperty(nameof(GuidHolder.Value));

                    Assert.That(new SerializableGuid(property).Instance, Is.EqualTo(Guid.Parse(FirstGuid)));
                    Assert.That(SerializableGuid.GetPropertyGuid<SerializableGuid>(property).Instance, Is.EqualTo(Guid.Parse(FirstGuid)));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(holder);
            }
        }

        [Test]
        public void GenericSerializedPropertyWritePreservesTheGuidStringSchema()
        {
            GuidHolder holder = ScriptableObject.CreateInstance<GuidHolder>();
            try
            {
                holder.Value = new SerializableGuid(FirstGuid);
                using (var serialized = new SerializedObject(holder))
                {
                    SerializedProperty property = serialized.FindProperty(nameof(GuidHolder.Value));
                    SerializableGuid.SetPropertyGuid<SerializableGuid>(property, new SerializableGuid(SecondGuid));
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                    serialized.Update();

                    Assert.That(property.FindPropertyRelative("_guidString").stringValue, Is.EqualTo(SecondGuid));
                    Assert.That(SerializableGuid.GetPropertyGuid<SerializableGuid>(property).Instance, Is.EqualTo(Guid.Parse(SecondGuid)));
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(holder);
            }
        }

        public sealed class GuidHolder : ScriptableObject
        {
            public SerializableGuid Value;
        }
    }
}
