using Atomic.Entities;
using Atomic.Infrastructure;
using UnityEngine;

namespace RTSGame
{
    public sealed class HeadquartersBaker : GameEntityBaker
    {
        [SerializeField]
        private LifeEntityBaker _lifeBaker;

        [SerializeField]
        private TeamEntityBaker _teamBaker;

        [SerializeField]
        private TransformEntityBaker _transformBaker;

        protected override void OnValidate()
        {
            _teamBaker.OnValidate();
            base.OnValidate();
        }

        protected override void Override(IGameEntity entity, Args<IGameContext> args)
        {
            entity.Install(_lifeBaker);
            entity.Install(_teamBaker);
            entity.Install(_transformBaker);
        }
    }
}