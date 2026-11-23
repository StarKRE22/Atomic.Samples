using NUnit.Framework;

namespace Modules.Spatial
{
    public sealed partial class KdTree2DTests
    {
        private KdTree2D<object> tree2D;

        [SetUp]
        public void Setup()
        {
            tree2D = new KdTree2D<object>();
        }
    }
}