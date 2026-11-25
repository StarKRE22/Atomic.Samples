using Atomic.Infrastructure;

namespace Atomic.Entities
{
    public class MonoEntityAspectSpy : MonoAspect<IEntity>
    {
        public bool Applied { get; private set; }
        
        public bool Discarded { get; private set; }

        public override void Apply(IEntity entity) => Applied = true;
        
        public override void Discard(IEntity entity) => Discarded = true;
    }
}