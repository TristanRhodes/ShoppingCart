Feature: StockManagementFeature
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: See items available
	Given The service is running
	When I request a list of stock items
	Then I recieve items with name, description, identifier and quantity.

Scenario: Add item to basket
	Given The service is running
	When I request a list of stock items
	And I add a stocked item to a users basket
	Then The basket I recieve contains this item

Scenario: Get user basket
	Given The service is running
	When I request a list of stock items
	And I add a stocked item to a users basket
	When I request a users basket
	Then The basket I recieve contains this item
