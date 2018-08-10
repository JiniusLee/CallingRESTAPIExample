# CallingRESTAPIEExample - Design Spec
Understanding the project

## Document Information
### References
1. https://badapi.iqvia.io/swagger/

### Contributors
1. jinhyunlee@email.virginia.edu

## Overview
### Overview
This is to create a library that can be used to get tweets from the public REST API. 

### Problem
There is a public API that returns tweets given date time. However, it only returns at most 100 entries. Therefore, we need a way to get tweets without worrying that we may have missed some.

## Architectural Goals
### Design goals
* Identify data model for tweet.
* Identify classes and function.

### Future goals (non-goals)
* UI Design to consume the library
* Pagination support

## User Story
### Getting tweets
User choses the start date time and end date time.
User gets the tweets from start date to end date.

### Console tester
For example, console test should be able to consume the library and get tweets from 2016 and 2017.

## High Level Architecture
### TweetServiceClient

## Design Details
### Tweet
This is the data model for tweet object. It contains id, timestamp, and text

Name  | Type | Description
------- | ------ | ------------
id | string | The unique identifier
stamp | DateTimeOffset | The time stamp associated with the tweet
text | string | The message

#### Assumption
1. We assume that the API will return **unique** identifier.
..* If not unique, we would just have to add check if rest of the other contents to see if its actually a duplicate.
2. We assume there **can** be **multiple** tweets at **same** timestamp. (timestamp is not unique)
..* If there is **no multiple** tweets at **same** timestamp, it becomes easier as getting next set of tweets would be a tick after last item's timestamp.


### TweetServiceLogEvent
This is the event code for logging purposes

Name  | Description
------- | -------------
GetTweetAsyncStarted | Get tweet API call has started
GetTweetAsyncEnded | Get tweet API call has ended
GetTweetAsyncException | Get tweet API call has ended with a error.
GetRestOfTweets |  The first get tweet API call was not enough, and more call is needed.

#### Assumption
1. We assume that the client will provide a valid ILogger.

### TweetServices
The main logic code.

#### private TweetServices data
Name  | Type | Description
------- | ------ | -------------
client | HttpClient | The http client that can make REST API call
logger | ILogger | The logger used to log events.
MAXCOUNT | readonly int | Max number of tweets possible from API call, 100

#### TweetServices Class 
Initialize TweetServices class and sets the URI for the class.
##### Param 
Name  | Type | Description
------- | ------ | -------------
client | HttpClient | The http client that can make REST API call
logger | ILogger | The logger used to log events.

#### GetPartialTweetsAsync
Gets at most 100 tweets form given time stamp asyncroniously. 

1. Set the url with parameters
2. Call the REST API
3. If successful, read the result as string. (4 - 5)
4. Deserialize  into list of tweets. 
5. Return the tweets.
6. If failure, throw, but just add to a log as well.

##### Param
Name  | Type | Description
------- | ------ | -------------
startDateTime | DateTimeOffset | The start date time offset
endDateTime | DateTimeOffset | The end date time offset

##### Return
Type | Description
------ | -------------
Task<IList<Tweet>> | The list of tweets 

#### GetTweetsAsync
Gets all the tweets form given time stamp asyncroniously. 

1. Attemps to get tweets from given time stamp using getPartialTweets.
2. Add to all tweet collection. 
3. If the result is 100, then we need to make another getPartialTweets call. (Step 4 - 7)
4. Change the start date time to last item's date time.
..* This is because there may be another tweet at the last tweet's time stamp. We can't just assume all tweets have unique time stamp.
5. Get the tweets from new time stamp using getPartialTweets.
6. Since API date range is inclusive, remove all the tweets that getTweets retrieved again.
..* This changes the capability of the each rest API call from 100 at most to 99 at most.
7. Repeat step 2.
8. Return the all the aggregated tweets.

##### Param
Name  | Type | Description
------- | ------ | -------------
startDateTime | DateTimeOffset | The start date time offset
endDateTime | DateTimeOffset | The end date time offset

##### Return
Type | Description
------ | -------------
Task<IList<Tweet>> | The list of tweets 

#### Assumption
1. We assume that the get tweet API will only return at most 100.
..* If changed, we just have to update that. 
2. We hard code the service URI.
..* If changed, we just have to update that.
3. From investigation, we assume get tweet API will return in **first 100 in ascending order**.
..* If not in ascending order, but descending, we can change the logic *backward*
..* If random, then we must pick a smaller date range that will return less than 100 to make sure you didn't miss any.
4. We assume there are always **less than 100** tweets at exact same time stamp.
..* If there are 100 or more tweets at exact same time, there are **no way** to retrieve those.


## Testing
1. Test more than 100
2. Test less than 100
3. Test exact 100
4. Test none
5. Test error

## Performance
### Review
The main bottleneck is the REST API call.
However if we consider each API call as 1 unit of work O(x) -> O(1),
Let n = API calls needed since each API returns 100 items from n tweets.

Readding the response is O(n), but 100 at most, O(100) -> O(1)
Deserialize takes O(n), but 100 at most, O(100) -> O(1)
Add to total array O(n), but 100 at most, O(100) -> O(1)
Save all the tweets at the last date time in to a set O(n), but 99 at most, O(99) -> O(1)

This block is O(100)  but will be call O(n/100) times. from the while loop.
A while loop to do this block over again, O(n)

This has time complexity of O(n)
This has space complexity of O(n)

### How to improve
We can't improve the API call, but we can support pagination API so that UI don't have to wait if using this library.



## Phase 2
1. Add UI that can display UI more friendly.
2. I would add pagination support, especailly for UI.

Display for example, first 25 out of 100.
When user scrolls to 50+, call to get next 100.
and so on and so on. This would make first loading fast and wouldn't show *lagging* as much.
