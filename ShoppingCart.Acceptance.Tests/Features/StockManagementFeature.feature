Feature: StockManagementFeature
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: See items available
	Given The service is running
	When I request a list of stock items
	Then I recieve items with name, description, identifier and quantity.
