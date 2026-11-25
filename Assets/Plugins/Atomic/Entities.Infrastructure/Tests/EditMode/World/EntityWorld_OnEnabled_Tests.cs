using NUnit.Framework;

namespace Atomic.Entities
{
    [TestFixture]
    public class EntityWorld_OnEnabled_Tests
    {
        [Test]
        public void Enable_Raises_OnEnabled()
        {
            var world = new EntityManager<Entity>();
            int callCount = 0;
            world.OnEnabled += () => callCount++;

            world.Enable();

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Enable_DoesNotRaise_OnEnabled_IfAlreadyEnabled()
        {
            var world = new EntityManager<Entity>();
            int callCount = 0;
            world.OnEnabled += () => callCount++;

            world.Enable();
            world.Enable(); // повторный вызов

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Enable_TriggersSpawn_IfNotAlreadySpawned()
        {
            var entity = new Entity();
            var world = new EntityManager<Entity>(entity);

            Assert.IsFalse(entity.Initialized);

            world.Enable();

            Assert.IsTrue(entity.Initialized);
        }

        [Test]
        public void Enable_Raises_OnEnabled_WithNoEntities()
        {
            var world = new EntityManager<Entity>();
            bool called = false;
            world.OnEnabled += () => called = true;

            world.Enable();

            Assert.IsTrue(called);
        }

        [Test]
        public void Enable_LateSubscriber_DoesNotGetEvent()
        {
            var world = new EntityManager<Entity>();
            world.Enable();

            bool called = false;
            world.OnEnabled += () => called = true;

            Assert.IsFalse(called);
        }
    }
}