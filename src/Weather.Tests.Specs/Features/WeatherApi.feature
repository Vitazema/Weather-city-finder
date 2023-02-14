Feature: Check weather api

Scenario: It should convert Date and Time correctly
	Given the test datetime in unix format 1674239133
	When app converted it from unix to datetime format
	Then the result should be 2023-1-20T21:25:33
	
Scenario: Get weather information about locations
	Given I have locations with names
		| Name   |
		| Moscow |
		| London |
		| Paris  |
 	When I'm requesting weather
 	Then the system should return following temperatures
 		| Temperature |
	    | 10          |
	    | 20          |
	    | 25          |