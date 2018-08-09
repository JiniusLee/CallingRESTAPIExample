# CallingRESTAPIExample
Project that calls REST API

### Overview
This is a project that calls a REST API.
Phase 1 (Completed): 
There is a C# library that calls the REST API and converts into an object. 
This library is able to log using the Microsoft.Extensions.ILogger.
There's unit test and intergration test associated with the library. 
You would potentially be able to use this library by getting the dll.

Phase 2: I would add UI that can display UI more friendly.
I would add pagination support, especailly for UI.

### Projects 
#### TweetServiceClient.csproj
This gets the 'tweets' by calling the REST API

#### TweetConsole.csproj
This is the start up project that you would run to use the library to see an example.
This is what you should run

#### TweetServiceClient.IntTest.csproj
This is the integration test that calls the actual API.

#### TweetServiceClient.UnitTest.csproj
This is the unit test that mocks the API call.

### How to run
1. Open Tweets.sln
2. Make sure to check TweetConsole is set as start up project.
3. Run. This should open console that displays all the tweets.
