var pact = require("@pact-foundation/pact-node");
var opts = {
	pactFilesOrDirs: [`../pacts`],
	pactBroker: "https://ssw.pactflow.io",
	// this can be the fix version (e.g. release version)
	// or this could be git commit SHA
  consumerVersion: "1.0.1.103",
  pactBrokerToken: "xRy0RdbaDE1oSvyxEfuT6Q", 
	// which environemtn is this? 
	tags: ["test","prod"],
  // verbose: true
};

pact.publishPacts(opts).then(function () {
	// do something
	console.log("pact published");
});
