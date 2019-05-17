﻿namespace Nancy.Tests.Unit.Localization
{
    using FakeItEasy;
    using Xunit;
    using Nancy.Localization;

    public class ResourceBasedTextResourceFixture
    {
        [Fact]
        public void Should_Return_Null_If_No_Assembly_Found()
        {
            //Given
            var resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            A.CallTo(() => resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[] { typeof(NancyEngine).Assembly });

            var defaultTextResource = new ResourceBasedTextResource(resourceAssemblyProvider);
            var context = new NancyContext();

            //When
            var result = defaultTextResource["Texts.Greeting", context];

            //Then
            result.ShouldBeNull();
        }
    }
}
