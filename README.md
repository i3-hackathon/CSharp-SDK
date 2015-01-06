**NOTICE:** This fork is modified to work in conjunction with the [BMW i3 Hackathon developer center](https://data.hackthedrive.com/) ONLY.  If you are not apart of the BMW i3 hackathon, please refer to [Mojio.Client](https://github.com/mojio/Mojio.Client) project instead.

BMW.Client
============

In order to create our store and apps, we have built a simple C# client library to help us interact with our API. It occurred to us that maybe other people would like to develop in C# as well.  So we have published the BMW Client on github for anyone to use and play around with!


Installation
============

Using NuGet
-----------

We recommend you use the NuGet package manager to install the **BMW.Client** NuGet package.

Manually
--------

Download or checkout the BMW.Client.  You will then need to include the Mojio (Src/Mojio/Mojio.csproj) and Mojio.Client (Src/Mojio.Client/Mojio.Client.csproj) projects into your solution.  Nuget is also required to get our client working, and you will need to perform a Nuget Restore the first time you import the projects.


Getting Started
===============

To begin developing with our client, you will need your very own application ID and secret key.  First you will need to create an account and login to our [developer center](https://data.hackthedrive.com/). 

Once you have logged in, you can create a new Application.  From here, you will want to copy the Application ID and the Secret Key, these will be required to initialize the BMW.Client.


Initializing the Client
-----------------------

To get started using the client, you must first create a new instance of the BMWClient object.  This is where you will need to pass in the Application ID and Secret Key, as well as the BMW server you wish to connect to (optional).

```csharp
using Mojio.Client;

Guid appID = new Guid("{APPID}");
Guid secretKey = new Guid("{SecretKey}");

var client = new BMWClient(
                        BMWClient.Live
                    );

await client.BeginAsync(appID, secretKey);


```

Authenticate a BMW User
-------------------------

Now that your BMWClient is associated with your app, you can get started making some calls.  However, many of our API calls also require an authorized user to be associated with the client sesion.  In order to authenticate a user, you must pass in the users name or email along with their password.

```csharp
// ...
// Authenticate specific user
await client.SetUserAsync( "demo@example.com", "mypassword");
	
// ...
// Logout user.
await client.ClearUserAsync();
```

Fetching Data
-------------

To retrieve a set of a particular entities, you can use the "Get" method.  The returned results will depend on what user and application your client session is authorized as. Lists of data will be returned in a paginated form.  You are able to set the page size and request a particular page.  In order to keep response times fast, it is recommended to keep the page size low.

```csharp
// ...
// Set default page size
client.PageSize = 15;
	
// Fetch first page of 15 trips
MojioResponse<Results<Trip>> response = await client.GetAsync<Trip>();
Results<Trip> result = response.Data;
	
// Iterate over each trip
foreach( Trip trip in results.Data )
{
     // Do something with each trip
     // ...
}
```

Fetch a specific Entity
-----------------------

By passing in the ID of an entity (often a GUID), you can fetch just that single entity from the database.

```csharp
// ...
var vehicleId = new Guid("0a5453a0-7e70-16d1-a2w6-28dl98c10200"); // Vehicle ID
	
// Fetch vehicle from API
MojioResponse<Vehicle> response = await client.GetAsync<Vehicle>(vehicleId);
Vehicle vehicle = response.Data;
	
// Do something with the vehicle data
// ...
```

Update an Entity
----------------

If you want to update and save an entity, you need to first load the entity from the API, make your changes, and then save it back.  Typically only the owner of an entity will be authorized to save changes and not all properties of an entity will be editable (for example, for an App, only the Name and Description properties can be changed).

```csharp
// ...
MojioResponse<Vehicle> response = await client.GetAsync<Vehicle>(new Guid("015151a1-7e70-16d1-a2w6-28dl98c10200"));
Vehicle vehicle = resonse.Data;

// Make a change
vehicle.Name = "New Name";

// Save the changes
await client.UpdateAsync(vehicle);
```

Get a list of child entities
----------------------------

If you want to fetch all the entities associated with another entity, you can call the GetBy method.  For example, if you want to fetch all the events associated with a vehicle.

```csharp
// ...
var vehicleId = new Guid("0a5453a0-7e70-16d1-a2w6-28dl98c10200");

// Fetch all events by vehicle ID
MojioResponse<Results<Event>> response = await client.GetByAsync <Event,Vehicle>(vehicleId);
Results<Event> events = response.Data;

//Or, alternatively
MojioResponse<Vehicle> mojioResponse = await client.GetAsync<Vehicle>(vehicleId);
Vehicle vehicle = mojioResponse.Data;

MojioResponse<Results<Event>> eventsResponse = await client.GetBy<Event>(vehicle);

// ...
```

Using the BMW Storage
-----------------------

With the BMW API, you are able to store your own private data within our database as key value pairs.  These key value pairs will only be accessible by your application, and you can associate them with any entities (ex: Application, User, Trip, Event).

```csharp
// ...
Guid userId = new Guid("0a5453a0-7e70-16d1-a2w6-28dl98c10200");  // Some user's ID
string key = "EyeColour";	// Key to store
string value = "Brown"; 	// Value to store

// Save user's eye colour
await client.SetStoredAsync<User>( userId, key , value );
	
// ...
// Retrieve user's eye colour
String stored = await client.GetStoreAsync<User>( userId, key );
```

Using SignalR to listen for events
----------------------------------

Instead of continuously polling the API to see if any new events have come in, our API has a signalR service you can subscribe to in order to be sent new event notifications as they happen.

```csharp
    // ...
    // The Vehicle ID you wish to listen to
    Guid vehicleId = new Guid("0a5453a0-7e70-16d1-a2w6-28dl98c10200");
	
    // An array of event types you wish to be notified about
    EventType[] types = new EventType[] { EventType.IgnitionOn, EventType.LowFuel };

    // Setup the callback function
    public void ReceiveEvent(Event event)
    {
        if( event.EventType == EventType.IgnitionOn)
            // Ignition on detected!
            // ...
        else if( event.EventType == EventType.LowFuel )
            // Do something with the low fuel warning
            // ...
    }

    client.EventHandler += ReceiveEvent;            // Binds the event listener
    await client.Subscribe<Vehicle>(vehicleId,types);   // Register subscrition

    // ...
    // Unsubscribe
    client.Unsubscribe<Vehicle>(vehicleId,types);
```
