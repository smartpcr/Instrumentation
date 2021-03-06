﻿Feature: TrackCallGraph
	In order to understand how calls are passed
	As a developer
	I want to be told the call graph

@callgraph
Scenario: track call graph
	Given product store is setup with following records
	| Id | Name  |
	| 1  | Bike  |
	| 2  | Train |
	When I call api layer to get product by id 2
	Then I should get the following product with name "Train"


@callgraph
Scenario: track call graph in async methods
	Given product store is setup with following records
	| Id | Name  |
	| 1  | Bike  |
	| 2  | Train |
	When I call api layer to get product by id 1 and 2 asynchronously
	Then I should get the following product with name "Bike" and "Train" respectively