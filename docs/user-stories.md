# User Stories

## Index
| ID | Title | Status | Application | Role | Updated |
|---|---|---|---|---|---|
| `001` | [Register Local Account](#US-001-Register-Local-Account) | `Done` | `TradingPortal` | `Trader` | `07.03.2026 14:30:00` |
| `002` | [Login Local Account](#US-002-Login-Local-Account) | `Done` | `TradingPortal` | `Trader` | `07.03.2026 14:45:00` |

---

## US-001-Register Local Account

### Required Fields
- `Title`: `Register Local Account`
- `Status`: `Done`
- `Application`: `TradingPortal`
- `Application User`: `Trader`
- `Business Issue`: The application user cannot access protected capabilities until a valid local account exists.
- `Business Setup`: The user reaches the registration flow after completing the existing navigation steps, stopping just before account creation.
- `User Action`: The user submits registration data (`Email`, `UserName`, `Password`, `Roles`) in the registration flow.
- `Business workflow`:
  - Validate the submitted registration data.
    - [x] Registration is rejected when `Email` is missing.
    - [x] Registration is rejected when `UserName` is missing.
    - [x] Registration is rejected when `Password` does not meet policy.
    - [x] Registration is rejected when `Role` is missing.
  - Normalize identity details to a consistent format.
  - Check whether an account with the same email already exists.
    - [ ] Registration is rejected when the email already exists.
  - Create a new account profile with the selected business role.
    - [ ] A successful registration returns the new account identity and assigned roles.
  - Save the new account for future sign-in.
    - [ ] Account is persisted to a durable store
  - Rturn [ ]
    - [ ] Registration succeeds when all required fields are valid.  
- `Achieved Business Outcome`: Application User can gain store its account, which later can be used for authentication and authorization.

### Optional Fields
- `Original Estimated Work`: `6 hours`
- `Remaining Work`: `2 hours`
- `Applied Work`: `16 hours`
- `Frequency`: `Often`
- `Importance`: `Must have`
- `Urgency`: `Immediate`
- `Impact`: `Business Critical`
- `Certainty`: `High confidence`
- `Stable`: `Stable`

## US-002-Login Local Account

### Required Fields
- `Title`: `Login Local Account`
- `Status`: `Done`
- `Application`: `TradingPortal`
- `Application User`: `Trader`
- `Business Issue`: The application user cannot access role-based capabilities without proving identity and account eligibility.
- `Business Setup`: The user navigates to the login flow after reaching the point just before authentication.
- `User Action`: The user submits login credentials (`Email`, `Password`) in the login flow.
- `Business workflow`:
  - Validate that login uses supported account type and includes required credentials.
  - Normalize email input for consistent identity lookup.
  - Find the matching account by normalized email.
  - Reject access when the account is missing, locked, or password verification fails.
  - Approve authentication and resolve business roles for authorized access.
- `Achieved Business Outcome`: A valid existing user is authenticated and receives identity and role context for subsequent authorized actions.
- `Acceptance criteria`:
  - [x] Login is rejected when account type is not supported.
  - [x] Login is rejected when `Email` credential is missing.
  - [x] Login is rejected when `Password` credential is missing.
  - [x] Login is rejected when no account exists for the provided email.
  - [x] Login is rejected when the account is locked.
  - [x] Login is rejected when password verification fails.
  - [x] Login succeeds with valid credentials and returns authentication identity, user name, and roles.

### Optional Fields
- `Original Estimated Work`: `6 hours`
- `Remaining Work`: `2 hours`
- `Applied Work`: `16 hours`
- `Frequency`: `Very often`
- `Importance`: `Must have`
- `Urgency`: `Immediate`
- `Impact`: `Business Critical`
- `Certainty`: `High confidence`
- `Stable`: `Stable`

## Scales
- `Status`
  - `Forming`
  - `Accepted`
  - `In Progress`
  - `Done`
- `Work`
  - `2 hours`
  - `6 hours`
  - `16 hours`
  - `32 hours`
- `Frequency`
  - `Rarely`
  - `Occasionally`
  - `Often`
  - `Very often`
- `Importance`
  - `Won't have`
  - `Could have`
  - `Should have`
  - `Must have`
- `Urgency`
  - `No rush`
  - `Soon`
  - `Urgent`
  - `Immediate`
- `Impact`
  - `Minor`
  - `Moderate`
  - `Major`
  - `Business Critical`
- `Certainty`
  - `Guess`
  - `Low confidence`
  - `High confidence`
  - `Confirmed`
- `Stable`
  - `Forming`
  - `Evolving`
  - `Stable`
  - `Never change`
