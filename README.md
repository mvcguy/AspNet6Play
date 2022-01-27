# AspNet6Play
Play project using ASP.NET core 6 using razer pages. The app is configured to work with JWT tokens and cookies. For cookie based auth, the identity server is configured with interactive flow. This uses the PKCE protocol to make sure the data is not tempered in the flight. It has more pros, and i recommend to read the official docs.
The controllers are annotatted in such a way that it should support multiple auth schemes.

Play tasks:
1. Setup a play project using razer pages
2. Add generic navigation repository and service to navigate through records
3. Add generic javascript to support the navigation in the UI.
4. Add models for the application dbconext
5. Add identity server 4 and customize the user store
6. Add custom claims using the custom user store
7. configure the client app to utilize the claims for example user.id and tenant.id

Note: The sample app can be configured to work with both Sql Server and SQL-Lite. SQL-Lite is good for speeding up development locally.

Snapshot showing the Booking view of the play app.

![customers-view]![image](https://user-images.githubusercontent.com/12786083/151269854-2d60eae8-35b7-4d5e-8061-be67a388401d.png)

Its also worth mentioning that the data-table is also implemented part of this play project. The navigation controls communicates using the event listeners to enable or disable controls based on the communication with the backend.
