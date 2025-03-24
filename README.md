App is built do pull orders from Printavo API V1.0 to a local access database.
Users can then setup to import data into UPS World Ship or Fed Ex on their desktop PC's from that Access DB using an ODBC connection.
Once an order is shipped, the cycle completes when UPS World ship or Fed Ex writes tracking information back to the Access DB.
Items are then PUT into their appropriate orders via HttPClient call.

This is untested in a live scenerio and is built on printavo's documentation via this link:
https://printavo.docs.apiary.io/#

App is tested using printavo's sandbox site 'Mock' https://private-anon-f15ec4e8e7-printavo.apiary-mock.com/api/version/sessions
Anomoly: the order response JSON schema has an error where a comma is missing for a property. 
For testing, this replacement string was added in order to perform testing necessary to make the videos 
json = json.Replace("\"id\": 1194978", "\"id\": 1194978,");
You may want to remove this line 169 found in the cPrintavo.cs, otherwise I am sure it won't cause any issues.

The entire process can be handled manually or automaticall if the service is started and the app is sent to tray.
Any questions or change requests, please don't hestitate to reach out to me:
mikeoshea781@gmail.com

Free under MIT and not responsible for any compiled versions.
