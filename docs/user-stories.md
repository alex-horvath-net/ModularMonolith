# User Stories

## Index
| ID | Title | Status | Application | Role | Updated |
|---|---|---|---|---|---|
| `001` | [US-001](#us-001-register-local-account) | `Done` | `TradingPortal` | `Trader` | `2026-03-07` |

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
  - Normalize identity details to a consistent format.
  - Check whether an account with the same email already exists.
  - Create a new account profile with the selected business role.
  - Save the new account for future sign-in.
- `Achieved Business Outcome`: A new local account is created for the user and can be used in later authentication flows.
- `Acceptance criteria`:
  - [x] Registration is rejected when `Email` is missing.
  - [x] Registration is rejected when `UserName` is missing.
  - [x] Registration is rejected when `Password` does not meet policy.
  - [ ] Registration is rejected when no valid role is selected.
  - [ ] Registration is rejected when the email already exists.
  - [ ] Registration succeeds when all required fields are valid.
  - [ ] A successful registration returns the new account identity and assigned roles.

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

### Scales
- `Status`: `Forming`, `Accepted`, `In Progress`, `Done`
- `Work`: `2 hours`, `6 hours`, `16 hours`, `32 hours`
- `Frequency`: `Rarely`, `Occasionally`, `Often`, `Very often`
- `Importance`: `Won't have`, `Could have`, `Should have`, `Must have`
- `Urgency`: `No rush`, `Soon`, `Urgent`, `Immediate`
- `Impact`: `Minor`, `Moderate`, `Major`, `Business Critical`
- `Certainty`: `Guess`, `Low confidence`, `High confidence`, `Confirmed`
- `Stable`: `Forming`, `Evolving`, `Stable`, `Never change`