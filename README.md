Development Notes
The application is currently in a testing phase and has not yet been validated in a live production environment.

It was built using Printavo’s official API documentation, available at:
https://printavo.docs.apiary.io/#

API testing has been conducted using Printavo’s sandbox/mock API:
https://private-anon-f15ec4e8e7-printavo.apiary-mock.com/api/version/sessions

Known Issues
An anomaly was identified in the sandbox order response JSON schema where a missing comma results in an invalid structure. As a temporary workaround during testing, the following string replacement was added to the response:

csharp
Copy
Edit
json = json.Replace("\"id\": 1194978", "\"id\": 1194978,");
This line is located in cPrintavo.cs at line 169. You may choose to remove it when working with a corrected or live API endpoint, although it should not cause issues if left in place for now.

Usage
The process can be run either manually or automatically. When the application is minimized to the system tray and the background service is running, it will continuously monitor and process order data.

Users can also trigger sync operations manually if needed.

Contact and License
For any questions, feature requests, or issues, please feel free to reach out directly.
mikeoshea781@gmail.com

This software is provided free of charge under the MIT License. No warranty is provided, and the author assumes no responsibility for compiled or distributed versions.


