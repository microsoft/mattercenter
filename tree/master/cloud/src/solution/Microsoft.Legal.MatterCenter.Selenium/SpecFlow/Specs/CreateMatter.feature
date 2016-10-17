Feature: Create Matter Page

@E2E
Scenario: 01.Open the browser and load create matter page
	When user enters credentials on matter provision page
	Then matter provision page should be loaded with element 'snConflictCheck'

@E2E
Scenario: 02.Verify incorrect form submission error on open matter tab
	When user submits blank and incorrect forms
	Then next step should not be allowed

@E2E
Scenario: 03.Verify Open Matter tab
	When user selects basic matter information
	Then it should navigate to second step

@E2E
Scenario: 04.Verify incorrect form submission error on Assign Permission tab
	When user submits blank forms on assign permissions
	Then it should not navigate to third step

@E2E
Scenario: 05.Verify Assign Permission tab
	When user selects permission for current matter
	Then it should navigate to third step

@E2E
Scenario: 06.Verify create and notify tab
	When user checks all check boxes 
	Then all check boxes should get checked
	When user clicks on create and notify 
	Then a new matter should get created



