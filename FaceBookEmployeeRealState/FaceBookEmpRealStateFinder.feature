Feature: FaceBookEmployeeRealStateFinder
         This app will help you identify the top homes for sale near the new Facebook Menlo Park campus 
             
                                     
 Background: 
  Given alteryx running at "http://gallery.alteryx.com/"
  And I am logged in using "deepak.manoharan@accionlabs.com" and "P@ssw0rd"
  
Scenario Outline: FaceBook Employee Real State Finder
When I run analog store analysis with NumberOfFaceBookShare <NumberOfFaceBookShare> NumberOfBedRooms <NumberOfBedRooms> NumberOfBathRooms <NumberOfBathRooms>
Then I see the Facebook Shares result "<FacebookShares>"

Examples: 
| NumberOfFaceBookShare | NumberOfBedRooms | NumberOfBathRooms | FacebookShares                                  | 
| 0-20000               | 1                | 1                 | Facebook Employee Real Estate Finder            | 