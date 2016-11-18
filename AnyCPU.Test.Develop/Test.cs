using NUnit.Framework;

namespace AnyCPU.Test.Develop
{
    [TestFixture]
    public class AnyCPU_Test
    {
        [TestCase]
        public void AnyCPU_Develop_TestPlatform()
        {
            var obj = new AnyCPU.Object();
            if (System.Environment.Is64BitProcess)
            {
                Assert.AreEqual("x64", obj.Platform);
            }
            else
            {
                Assert.AreEqual("x86", obj.Platform);
            }
        }
    }
}
