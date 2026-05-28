using Esolang.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace Esolang.Generator.Tests;

[TestClass]
public class KindTests
{
    [TestMethod]
    [SuppressMessage("MSTest", "MSTEST0032")]
    public void MethodInputKind_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)MethodInputKind.None);
        Assert.AreEqual(1, (int)MethodInputKind.String);
        Assert.AreEqual(2, (int)MethodInputKind.TextReader);
        Assert.AreEqual(3, (int)MethodInputKind.PipeReader);
    }

    [TestMethod]
    [SuppressMessage("MSTest", "MSTEST0032")]
    public void MethodOutputKind_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)MethodOutputKind.None);
        Assert.AreEqual(1, (int)MethodOutputKind.TextWriter);
        Assert.AreEqual(2, (int)MethodOutputKind.PipeWriter);
    }

    [TestMethod]
    [SuppressMessage("MSTest", "MSTEST0032")]
    public void MethodReturnKind_HasExpectedValues()
    {
        Assert.AreEqual(0, (int)MethodReturnKind.Invalid);
        Assert.AreEqual(1, (int)MethodReturnKind.Void);
        Assert.AreEqual(2, (int)MethodReturnKind.Int32);
        Assert.AreEqual(3, (int)MethodReturnKind.String);
        Assert.AreEqual(4, (int)MethodReturnKind.NullableString);
        Assert.AreEqual(5, (int)MethodReturnKind.Task);
        Assert.AreEqual(6, (int)MethodReturnKind.TaskInt32);
        Assert.AreEqual(7, (int)MethodReturnKind.TaskString);
        Assert.AreEqual(8, (int)MethodReturnKind.TaskNullableString);
        Assert.AreEqual(9, (int)MethodReturnKind.ValueTask);
        Assert.AreEqual(10, (int)MethodReturnKind.ValueTaskInt32);
        Assert.AreEqual(11, (int)MethodReturnKind.ValueTaskString);
        Assert.AreEqual(12, (int)MethodReturnKind.ValueTaskNullableString);
        Assert.AreEqual(13, (int)MethodReturnKind.IEnumerableByte);
        Assert.AreEqual(14, (int)MethodReturnKind.IAsyncEnumerableByte);
    }
}
