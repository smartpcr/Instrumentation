Feature: TrackMethodCall
	In order to help trouble shooting 
	As a developer
	I want to be able to get list of correlated calls 

@track
Scenario Template: track method call
	Given a student grades
	| name | subject | grade |
	| John | Math    | 90    |
	| xd   | Math    | 80    |
	| Adam | Math    | 50    |
	| John | Literature    | 77    |
	| xd   | Literature    | 65    |
	| Adam | Literature    | 100    |
	When I caculate grade average by subject "<Subject>"
	Then the average should be <Average>

Examples: 
| Subject    | Average |
| Math       | 73      |
| Literature | 80      |