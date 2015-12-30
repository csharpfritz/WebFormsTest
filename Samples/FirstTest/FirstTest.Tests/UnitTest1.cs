using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FirstTest.Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {

      // Arrange

      // Act
      var sut = new FirstTest.Default();
      sut.FireEvent(WebFormsTest.TestablePage.WebFormEvent.Load, EventArgs.Empty);

      // Assert
      Assert.IsTrue(sut.LoadEventTriggered, "Load event was not triggered");

    }
  }
}
