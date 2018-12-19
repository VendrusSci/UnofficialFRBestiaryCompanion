using System.IO;
using System.Text;
using BestiaryLauncher.Model;
using NSubstitute;
using NUnit.Framework;

namespace LauncherTests
{
    public class StatusChecksTests
    {
        [Test]
        public void TestIsVersionDifferentWithSameVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.VersionFile).Returns(Encoding.ASCII.GetBytes("expected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, "expected version");
            loader.Received().Load(ApplicationPaths.VersionFile);
            Assert.IsFalse(wasDifferent, "expected version should match");
        }

        [Test]
        public void TestIsVersionDifferentWithDifferentVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.VersionFile).Returns(Encoding.ASCII.GetBytes("unexpected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, "expected version");
            loader.Received().Load(ApplicationPaths.VersionFile);
            Assert.IsTrue(wasDifferent, "expected version should NOT match");
        }
    }
}
