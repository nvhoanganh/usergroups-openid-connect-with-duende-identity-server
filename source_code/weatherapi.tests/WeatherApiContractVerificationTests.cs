using API.Tests.PactSetup;
using PactNet;
using Xunit;
using Xunit.Abstractions;

namespace API.Tests
{
    public class WeatherApiContractVerificationTests : ContractTestBase
    {
        public WeatherApiContractVerificationTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void EnsureWeatherAPIHonorsPactWithConsumer1()
        {
            new PactVerifier(_pactVerifierConfig)
                .ProviderState($"{ProviderUri}/provider-states")
                .ServiceProvider("Weather API", ProviderUri)
                .HonoursPactWith("react-client")
                // point to the contract file
                // not this is coming from bin folder of .net 
                .PactUri($"..\\..\\..\\..\\pacts\\react-client-weatherapi.json")
                .Verify();
        }
    }
}