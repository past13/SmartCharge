Technologies Used
ASP.NET Core: For building and hosting the API.<br>
MSSQL database. (migartion allready added just run project automaticly create tables from migration)<br>
MediatR: For implementing the mediator pattern to decouple business logic from controllers.<br>
AutoMapper: For mapping between domain entities and DTOs.<br>

**Usage**<br>

The endpoints are available under the **GroupController**:<br>

**GET** /group/all: Retrieve all groups.<br>
**GET** /group/{id}: Retrieve a group by ID.<br>
**POST** /group: Add a new group.<br>
**PUT** /group: Update an existing group.<br>
**DELETE** /group/{id}: Delete a group by ID.<br>

The endpoints are available under the **ChargeStationController**:<br>

**GET** /chargestation/all: Retrieve all charge stations.<br>
**GET** /chargestation/{id}: Retrieve a charge station by ID.<br>
**POST** /chargestation/group/{groupId}: Add a new charge station to a specified group.<br>
**PUT** /chargestation/{id}/group/{groupId}: Update an existing charge station within a group.<br>
**DELETE** /chargestation/{id}/group/{groupId}: Delete a charge station from a group.<br>

The endpoints are organized under the **ConnectorController**:<br>

**GET** /connector/all: Retrieve all connectors.<br>
**GET** /connector/{id}: Retrieve a connector by its ID.<br>
**PUT** /connector/chargestation/{chargeStationId}: Add a new connector to the specified charge station.<br>
**PUT** /connector/{id}/chargestation/{chargeStationId}: Update an existing connector in a charge station.<br>
**DELETE** /connector/{id}/chargestation/{chargeStationId}: Remove a connector from a charge station.<br>
