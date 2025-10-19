# REST API Plan

## 1. Resources

- __Drugs__ – Represents a drug’s definition, including its primary unit and derived unit settings.
- __Locations__ – Represents storage locations arranged in a hierarchical (tree) structure with unique symbols and optional QR code data.
- __Stocks__ – Represents drug inventories and batches (parties) with associated quantities, expiry dates, and location links.
- __Dosing Schedules__ – Represents scheduled dosage events including assignment to a household profile and linked drug information.
- __Alerts/Notifications__ – Represents push notifications and dashboard alerts for dosage reminders and expiring stock.
- __Authentication__ – Represents endpoints for user registration, login, and logout.

## 2. Endpoints

### Drugs

- __GET /api/drugs__\
  *Description:* Retrieves a paginated list of active drugs with summary stock and nearest expiry.\
  *Query Params:* `page`, `pageSize`, `sort`, `filter`\
  *Response:* JSON array of drug objects including `id`, `name`, `primaryUnit`, `derivedUnits` (with conversion factors), total stock, and metadata (created/updated timestamps).\
  *Success Codes:* 200, with appropriate messages on failure (e.g. 404 if no drugs found).

- __GET /api/drugs/{drugId}__\
  *Description:* Retrieves detailed information for a single drug including full unit definitions.\
  *Response:* JSON object describing drug details, associated units, barcode if available, and archival status.

- __POST /api/drugs__\
  *Description:* Creates a new drug definition.\
  *Request Payload:* JSON with properties such as `name`, `primaryUnit`, `derivedUnits` (array with names and conversion factors), optional barcode.\
  *Response:* New drug object with assigned `id` and confirmation message.\
  *Success Codes:* 201, error for validation failures (400).

- __PUT /api/drugs/{drugId}__\
  *Description:* Updates an existing drug’s definition (editable properties; primary unit is locked if there are stock entries).\
  *Request Payload:* JSON with updated fields for `name`, `derivedUnits`, barcode, etc.\
  *Response:* Updated drug object.\
  *Success Codes:* 200, errors for conflicts or validation (409, 400).

- __DELETE /api/drugs/{drugId}__\
  *Description:* Archives (deactivates) a drug; it is no longer returned in active listings.\
  *Response:* Confirmation message.\
  *Success Codes:* 200, error if drug not found (404).

- __POST /api/drugs/{drugId}/restore__\
  *Description:* Reactivates an archived drug definition along with its related stock history.\
  *Response:* Reactivated drug object.

### Locations

- __GET /api/locations__\
  *Description:* Retrieves the full nested tree of locations.\
  *Response:* JSON representing a hierarchical list where each location includes `id`, `name`, `symbol` (if set), and children locations.\
  *Success Codes:* 200.

- __POST /api/locations__\
  *Description:* Creates a new location.\
  *Request Payload:* JSON with `name`, optional `parentId` (for nested locations), and optional unique `symbol`.\
  *Response:* Newly created location object.\
  *Success Codes:* 201, with error messages for duplicate symbols (400).

- __PUT /api/locations/{locationId}__\
  *Description:* Updates properties of an existing location, including renaming and assigning/updating its unique symbol.\
  *Response:* Updated location object.

- __DELETE /api/locations/{locationId}__\
  *Description:* Archives a location (and its children are also archived).\
  *Response:* Confirmation message.

- __GET /api/locations/{locationId}/qrcode__\
  *Description:* Retrieves the QR code data/image for the location’s unique symbol.\
  *Response:* QR code image (or the data URL) representing the symbol.

### Stocks (Inventory)

- __GET /api/stocks__\
  *Description:* Retrieves a list of stock batches with filtering by drug, location, or expiry window.\
  *Query Params:* `drugId`, `locationId`, `expiryBefore` etc.\
  *Response:* JSON array containing each batch’s `id`, `drugId`, converted quantity (in primary unit), expiry date, and location details.

- __POST /api/stocks/{drugId}/adjust__\
  *Description:* Adjusts the stock for a drug batch by adding, consuming, or correcting its quantity.\
  *Request Payload:* JSON with action (`add`, `consume`, `adjust`), quantity, unit (which will be converted to the primary unit by the system), and optional batch identifier.\
  *Response:* Updated stock batch detail and confirmation message.\
  *Success Codes:* 200, error for over-consumption (400).

- __POST /api/stocks/{drugId}/dispose__\
  *Description:* Marks a specific stock batch as disposed (for expired or used-up inventories).\
  *Response:* Confirmation message.

- __GET /api/stocks/inventory?locationId={locationId}__\
  *Description:* Returns expected drug stocks for a given location (used for inventory verification).\
  *Response:* Array of drug stock summaries.

### Dosing Schedules

- __GET /api/schedules__\
  *Description:* Retrieves a list of dosing schedules with filters (date, drug, household member).\
  *Response:* JSON array of schedule objects detailing `scheduleId`, `drugId`, `userId`, `dose`, schedule type, and timing details.

- __GET /api/schedules/{scheduleId}__\
  *Description:* Retrieves detailed dosing schedule information.

- __POST /api/schedules__\
  *Description:* Creates a new dosing schedule entry.\
  *Request Payload:* JSON with `drugId`, `userId` (household profile), `dose`, `doseUnit`, schedule type (daily, multiple times, specific days), and time details.\
  *Response:* New schedule object with `scheduleId`.

- __PUT /api/schedules/{scheduleId}__\
  *Description:* Updates an existing dosing schedule.\
  *Response:* Updated schedule object.

- __DELETE /api/schedules/{scheduleId}__\
  *Description:* Deletes a dosing schedule.\
  *Response:* Confirmation message.

### Alerts/Notifications

- __GET /api/alerts__\
  *Description:* Retrieves pending alerts for dosage reminders and expiry warnings.\
  *Response:* JSON list of alert objects containing type, related drug/stock information, and message details.

- __PUT /api/alerts/{alertId}/acknowledge__\
  *Description:* Marks a given alert as acknowledged by the user.\
  *Response:* Confirmation message.

### Authentication

- __POST /api/auth/register__\
  *Description:* Registers a new household owner account using email and password.\
  *Request Payload:* JSON with `email`, `password`, and password confirmation.\
  *Response:* User object with a JWT token issued on successful registration.

- __POST /api/auth/login__\
  *Description:* Authenticates a user with their credentials.\
  *Request Payload:* JSON with `email` and `password`.\
  *Response:* JWT token with user details.\
  *Error Codes:* 401 for invalid credentials.

- __POST /api/auth/logout__\
  *Description:* Logs out the currently authenticated user (if using token invalidation).\
  *Response:* Confirmation message.

## 3. Authentication and Authorization

- A __JWT Bearer token__ mechanism will be used to secure endpoints.
- Endpoints in the drugs, stocks, schedules, locations, and alerts modules will require an `Authorization` header with a valid token.
- The system supports a single "Household Owner" account; however, Profiles for household members can be maintained for dosing schedules.
- Role-based or policy-based checks will be implemented to ensure API calls are permitted.

## 4. Validation and Business Logic

- __Input Validation:__

    - Drug creation must provide a non-empty name and valid unit definitions.
    - Unique constraints are enforced for location symbols.
    - Stock adjustments validate that quantity changes do not reduce inventory below zero.

- __Business Logic:__

    - Unit conversions are performed automatically during stock adjustments (e.g., converting ‘blister’ to primary unit “tablet”).
    - When a dosage is confirmed (via schedule endpoints), the system automatically updates the stock based on the defined dose.
    - Expiry-based prioritization is built into stock consumption: batches with the soonest expiry are used first.
    - Archiving operations on drugs and locations do not delete historical data but mark records as inactive.

- __Error Handling:__

    - All endpoints return clear error messages for validation failures (400), authentication errors (401), forbidden actions (403), and not found cases (404).
    - Conflict errors (409) are returned when preconditions (e.g., duplicate symbols) are violated.
