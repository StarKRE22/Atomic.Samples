using Atomic.Infrastructure;

namespace RTSGame
{
    public sealed class GameEntityPriorityStrategy : IPriorityStrategy<IGameEntity>
    {
        public static readonly GameEntityPriorityStrategy Default = new();
        
        public Priority Evaluate(IGameEntity obj) => obj.GetUpdatePriority().Value;
    }
}