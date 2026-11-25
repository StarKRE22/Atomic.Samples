using Atomic.Infrastructure;

namespace RTSGame
{
    public sealed class GameEntityPriorityStrategy : IUpdatePriorityStrategy<IGameEntity>
    {
        public static GameEntityPriorityStrategy Default = new();
        
        public UpdatePriority EvaluatePriority(IGameEntity obj)
        {
            return obj.GetUpdatePriority().Value;
        }
    }
}