using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace API.Tests.PactSetup
{
    public abstract class ContractTestBase: IDisposable
    {
        protected const string ProviderUri = "https://127.0.0.1:9310";
        protected readonly PactVerifierConfig _pactVerifierConfig;
        private bool _disposedValue;
        private readonly ITestOutputHelper _output;
        private readonly IWebHost _webHost;
        
        protected ContractTestBase(ITestOutputHelper output)
        {
            _output = output;
            _webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseKestrel()
                .UseUrls(ProviderUri)
                .Build();
            _webHost.Start();
            
            _pactVerifierConfig = new PactVerifierConfig
            {
                Outputters =
                    new
                        List<IOutput> //NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture
                        //the console output, so a custom outputter is required.
                        {
                            new XUnitOutput(_output)
                        },
                // CustomHeaders = new Dictionary<string, string>
                // {
                //     {"Authorization", "Basic VGVzdA=="}
                // }, //This allows the user to set request headers that will be sent with every request the verifier
                //sends to the provider
                Verbose = true //Output verbose verification logs to the test output
            };
        }

        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _webHost.StopAsync().GetAwaiter().GetResult();
                _webHost.Dispose();
            }

            _disposedValue = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
    }
}