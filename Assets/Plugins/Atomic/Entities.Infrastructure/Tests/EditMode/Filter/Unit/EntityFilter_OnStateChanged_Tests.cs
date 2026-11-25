using Atomic.Infrastructure;
using NUnit.Framework;

namespace Atomic.Entities
{
    public partial class Filter_Tests
    {
        [Test]
        public void OnStateChanged_Should_Fire_WhenEntityMatchesPredicate_AndAdded()
        {
            // Arrange
            var source = new Store<IEntity>();
            var entity = new Entity();
            entity.AddTag("Enemy");

            bool wasCalled = false;
            var filter = new Filter<IEntity>(source, e => e.HasTag("Enemy"));

            filter.OnStateChanged += () => wasCalled = true;

            // Act
            source.Add(entity);

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void OnStateChanged_Should_NotFire_WhenEntityDoesNotMatchPredicate()
        {
            // Arrange
            var source = new Store<IEntity>();
            var entity = new Entity();

            bool wasCalled = false;
            var filter = new Filter<IEntity>(source, e => e.HasTag("Enemy"));

            filter.OnStateChanged += () => wasCalled = true;

            // Act
            source.Add(entity);

            // Assert
            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void OnStateChanged_Should_Fire_WhenEntityRemovedFromSource()
        {
            // Arrange
            var source = new Store<IEntity>();
            var entity = new Entity();
            entity.AddTag("Enemy");

            bool wasCalled = false;
            var filter = new Filter<IEntity>(source, e => e.HasTag("Enemy"));

            source.Add(entity);
            filter.OnStateChanged += () => wasCalled = true;

            // Act
            source.Remove(entity);

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void OnStateChanged_Should_Fire_WhenPredicateChanges_ThroughSynchronize()
        {
            // Arrange
            var source = new Store<IEntity>();
            var entity = new Entity();
            entity.AddTag("Enemy");

            var filter = new Filter<IEntity>(source, e => e.HasTag("Enemy"));


            source.Add(entity); // Initially added
            bool wasCalled = false;
            filter.OnStateChanged += () => wasCalled = true;

            // Simulate state change
            entity.DelTag("Enemy");

            // Act
            filter.Synchronize(entity);

            // Assert
            Assert.IsTrue(wasCalled);
        }
    }
}