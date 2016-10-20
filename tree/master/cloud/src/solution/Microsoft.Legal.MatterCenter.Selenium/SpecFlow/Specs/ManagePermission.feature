Feature: ManagePermission

@E2E
Scenario:01. Open the browser and load manage permission page
	When user enters credentials on manage permissions page
	Then manage permission page should be loaded with default permission	

@E2E
Scenario:02. Verify addition of new Attorney
	When user adds new Attorney to the matter
	Then Attorney should be added in the matter

@E2E
Scenario:03. Verify newly added Attorney
	When user clicks on save button on manage permission page
	Then updated Attorney should be added in the matter

@E2E
Scenario:04. Verify error on adding non existing Attorney
	When user adds non-existing Attorney to the matter
	Then Attorney should not be added