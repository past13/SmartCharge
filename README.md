Technologies Used
ASP.NET Core: For building and hosting the API.
MSSQL database. (migartion allready added just run project automaticly create tables from migration)
MediatR: For implementing the mediator pattern to decouple business logic from controllers.
AutoMapper: For mapping between domain entities and DTOs.

**Usage**

The endpoints are available under the **GroupController**:

**GET** /group/all: Retrieve all groups.
**GET** /group/{id}: Retrieve a group by ID.
**POST** /group: Add a new group.
**PUT** /group: Update an existing group.
**DELETE** /group/{id}: Delete a group by ID.

The endpoints are available under the **ChargeStationController**:

**GET** /chargestation/all: Retrieve all charge stations.
**GET** /chargestation/{id}: Retrieve a charge station by ID.
**POST** /chargestation/group/{groupId}: Add a new charge station to a specified group.
**PUT** /chargestation/{id}/group/{groupId}: Update an existing charge station within a group.
**DELETE** /chargestation/{id}/group/{groupId}: Delete a charge station from a group.

The endpoints are organized under the **ConnectorController**:

**GET** /connector/all: Retrieve all connectors.
**GET** /connector/{id}: Retrieve a connector by its ID.
**PUT** /connector/chargestation/{chargeStationId}: Add a new connector to the specified charge station.
**PUT** /connector/{id}/chargestation/{chargeStationId}: Update an existing connector in a charge station.
**DELETE** /connector/{id}/chargestation/{chargeStationId}: Remove a connector from a charge station.
