using GalacticLauncher.Frontend.Tools.Files;

namespace GalacticLauncher.Frontend.Tests.Tools.Files;

[TestFixture]
public class FileHasherTests
{
    private FileHasher _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new FileHasher();
    }

    [Test]
    public void HashSha256Async_FileNotFound_ThrowsFileNotFoundException()
    {
        string fakePath = "this_file_does_not_exist.txt";

        Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await _sut.HashSha256Async(fakePath, CancellationToken.None));
    }

    [Test]
    public async Task HashSha256Async_ValidFile_ReturnsCorrectHash()
    {
        string tempFile = Path.GetTempFileName();
        await File.WriteAllTextAsync(tempFile, "test");

        string expectedHash = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

        try
        {
            string actualHash = await _sut.HashSha256Async(tempFile, CancellationToken.None);

            Assert.That(actualHash, Is.EqualTo(expectedHash));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}