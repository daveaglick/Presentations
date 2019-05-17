﻿namespace Nancy.Testing.Tests
{
    using System.Linq;

    using Nancy.Tests;

    using Xunit;

    public class BrowserDefaultsFixture
    {
        private string expected;
        private CaptureRequestModule captureRequestModule;
 
        public BrowserDefaultsFixture()
        {
            // Given
            this.expected = "application/json";
            this.captureRequestModule = new CaptureRequestModule();
        }

        [Fact]
        public void Should_pass_default_headers_in_get_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Get("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_post_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Post("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_put_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Put("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_patch_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Patch("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_delete_request_when_using_configurable_bootstrapper_ctor()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Delete("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_get_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));
            
            // When
            sut.Get("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_post_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));
            
            // When
            sut.Post("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_put_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));
            
            // When
            sut.Put("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_patch_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));
            
            // When
            sut.Patch("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_default_headers_in_delete_request_when_using_inancybootstrapper_ctor()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with => with.Module(this.captureRequestModule));
            var sut = new Browser(bootstrapper, defaults: to => to.Accept(this.expected));
            
            // When
            sut.Delete("/");
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);
        }

        [Fact]
        public void Should_pass_both_defaults_and_request_specific_context_through()
        {
            // Given
            var sut = new Browser(with => with.Module(this.captureRequestModule), defaults: to => to.Accept(this.expected));
            
            // When
            sut.Get("/", with => with.Query("testKey", "testValue"));
            
            // Then
            this.captureRequestModule.CapturedRequest.Headers.Accept.First().Item1.ShouldEqual(this.expected);            
            Assert.Equal(this.captureRequestModule.CapturedRequest.Query.testKey.Value, "testValue");
        }

        public class CaptureRequestModule : NancyModule
        {
            public Request CapturedRequest;

            public CaptureRequestModule()
            {
                Get["/"] = _ => this.CaptureRequest();
                Post["/"] = _ => this.CaptureRequest();
                Put["/"] = _ => this.CaptureRequest();
                Delete["/"] = _ => this.CaptureRequest();
                Patch["/"] = _ => this.CaptureRequest();
            }

            private dynamic CaptureRequest()
            {
                this.CapturedRequest = this.Request;
                return HttpStatusCode.OK;
            }
        }
    }
}
