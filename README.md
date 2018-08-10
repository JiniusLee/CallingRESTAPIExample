# CallingRESTAPIExample
Project that calls REST API

### Overview
This is a project that calls a REST API.
Phase 1 (Completed): 
There is a C# library that calls the REST API and converts into an object. 
This library is able to log using the Microsoft.Extensions.ILogger.
There's unit test and intergration test associated with the library. 
You would potentially be able to use this library by getting the dll.

**There is a design doc to see the design of this library.**

Phase 2: I would add UI that can display UI more friendly.
I would add pagination support, especailly for UI.

### How to run
1. Open Tweets.sln
2. Make sure to check TweetConsole is set as start up project.
3. Run. This should open console that displays all the tweets.

#### Run option.
When running, you will be prompted with option of 3 things.
1. Print all tweets in console.
2. Print all tweets in "tweets.txt" file.
3. Print all tweets in "logs.txt" file.
Type 1, 2, or 3 to to print in desired manner.

*The text file will be located where the exec was ran*
