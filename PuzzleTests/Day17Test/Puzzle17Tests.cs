using NUnit.Framework;
using Puzzles;

namespace PuzzleTests;

[TestFixture]
public class Puzzle17Tests
{
    private readonly Day17 _puzzle = new(TestHelpers.TestDataPath(17));
    private readonly int _expectedResult1 = 45;
    private readonly int _expectedResult2 = 112;

    [Test]
    public void SolvePart1Test()
    {
        var result = _puzzle.SolvePart1();
        Assert.AreEqual(_expectedResult1, result);
    }

    [Test]
    public void SolvePart2Test()
    {
        var result = _puzzle.SolvePart2();
        Assert.AreEqual(_expectedResult2, result);
    }
}