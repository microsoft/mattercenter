Feature: Settings Page 

@E2E
Scenario: 01. Open the browser and load settings page
	When user enters credentials on settings page
	Then settings page should be loaded with element 'settingName'

@E2E
Scenario: 02. Set the value on settings page 
	When settings page is configured and save button is clicked
	Then settings should be saved and confirmation message should be displayed
