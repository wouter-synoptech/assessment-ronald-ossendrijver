# assessment-ronald-ossendrijver

## The functionality of the system
The system basically works as follows:
* It will handle all parcel container xml files that are placed in the Container folder on the server. 
* Parcels will be assigned to Departments based on the Department definition file (departmentconfig.txt) on the server. The syntax of this definition is explained below. This definition also tells the system which actions a Department can perform on a parcel.
* Performing an action (like Authorize or Handle) in the user interface will change the state of the parcel accordingly.
* For demo purposes, the state of each parcel can be reset from the user interface after which any containers will be unpacked from scratch.

## Technical info
The system is implemented in .NET 7. The simple UI is written in Blazor Webassembly and retrieves data from the Server using Web API (ASP.NET MVC). It is deployed as an Azure Web App.

The code is structured as follows:
* ParcelHandling.Client is a single page app. Pages/Index.razor constructs the NavBar to browse departments and each Parcel assigned to a department is visualized using the ParcelComponent Blazor component.
* ParcelHandling.Server defines the Web API used by the UI (in Controllers) and manages Container, Parcel and department configuration files on the server (in Managers).
* ParcelHandling.Shared contains shared classes and data structures, like Container and Parcel and the classes that implement the (simple) Rule Engine that assigns parcels to departments.
* ParcelHandling.Tests is a MSTest project containing a couple of unit tests that were used during design and implementation.

## Department definition
The assignment has much emphasis on the configurability and flexibility of departments and the business rules for assigning parcels to departments. A reasonable amount of flexibility is implemented with a simple workflow/rule engine that is configured in a configuration file as follows:
The file contains one of more departments, and for each department the expected format is as follows:
* The first line contains a Department name.
* The second line specifies zero or more actions the department can perform on a parcel and what the resulting parcel state will be.
* The next zero or more lines each contain expressions a parcel should ALL comply with to be assigned to the Department. Each line contains one or more conditions of which any one should me met. These conditions state that some characteristic of the parcel (like Weight) should be within some interval or should be equal to an exact value.
* An empty line marks the end the definition of the department.

Example:
```
Big Packages
Handle Package -> Handled, Scrap a Package -> Handled
Value in <*,1000] or Authorized = true
Handled = false

Insurance
Authorize Package -> Authorized
Value in [1000,*>
Handled = false
```
