# ChaCha-CT2
ChaCha Cipher + Visualization for CrypTool 2. Bachelor thesis.

### How to build and run 
1. Download Codebase for [CrypTool 2](https://www.cryptool.org/de/cryptool2). Read the [documentation](https://www.cryptool.org/de/ct2-dokumentation) of CT2 for how to do this.
2. Open the CT2 .SLN file with Visual Studio 2019.
3. Add the ChaCha project to `CrypPlugins` using the ChaCha .CSPPROJ file which is provided in this repository.
4. Build the whole solution.
5. Run solution.

### How to run tests

The ChaChaTest .CS file must be added to the `UnitTests` project of the CT2 Solution. Then you can just run the test using the the Test Explorer in Visual Studio. Make sure that you are in the Test configuration.
