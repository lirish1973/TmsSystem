# Software Pricing Calculator - Implementation Summary

## Overview
This implementation adds a comprehensive software pricing calculator to the TMS System that helps determine the recommended sale price of the software based on operational costs.

## Problem Statement (Hebrew)
בכמה כסף עלי לתמחר את מכירת התוכנה שבניתי, כולל מחיר חודשי של השרת על סך 6 דולר ושרת מייל על סך 8 דולר?

**Translation:** "How much money should I price the sale of the software I built, including a monthly server cost of $6 and an email server cost of $8?"

## Solution

### 1. Core Service: SoftwarePricingCalculator
**File:** `TmsSystem/Services/SoftwarePricingCalculator.cs`

**Features:**
- Calculates monthly operational costs: $6 (server) + $8 (email server) = $14/month
- Calculates annual operational costs: $14 × 12 = $168/year
- Computes recommended pricing based on:
  - Number of months to cover
  - Desired profit margin percentage
- Provides detailed pricing breakdown

**Key Methods:**
```csharp
public decimal GetMonthlyOperationalCost() // Returns $14
public decimal GetAnnualOperationalCost()  // Returns $168
public decimal CalculateRecommendedPrice(int numberOfMonths, decimal profitMarginPercentage)
public PricingBreakdown GetPricingBreakdown(int numberOfMonths, decimal profitMarginPercentage)
```

### 2. View Model: SoftwarePricingViewModel
**File:** `TmsSystem/ViewModels/SoftwarePricingViewModel.cs`

**Properties:**
- Input fields: NumberOfMonths (1-120), ProfitMarginPercentage (0-1000%)
- Output fields: All cost breakdowns and recommended price
- Includes validation attributes for input constraints

### 3. Controller: PricingController
**File:** `TmsSystem/Controllers/PricingController.cs`

**Actions:**
- `GET /Pricing/Calculator` - Displays the calculator form
- `POST /Pricing/Calculator` - Processes calculation and returns results

### 4. View: Calculator
**File:** `TmsSystem/Views/Pricing/Calculator.cshtml`

**Features:**
- Beautiful, responsive Hebrew (RTL) interface
- Bootstrap 5 styling with custom CSS
- Interactive form with:
  - Number of months input (1-120)
  - Profit margin percentage input (0-1000%)
- Real-time calculation display showing:
  - Fixed operational costs
  - Total operational cost for selected period
  - Profit amount based on margin
  - Final recommended price
- Detailed breakdown list

## Example Calculations

### Scenario 1: 12 Months, 50% Profit Margin
- Monthly operational cost: $14.00
- Number of months: 12
- Total operational cost: $168.00
- Profit (50%): $84.00
- **Recommended Price: $252.00**

### Scenario 2: 24 Months, 100% Profit Margin
- Monthly operational cost: $14.00
- Number of months: 24
- Total operational cost: $336.00
- Profit (100%): $336.00
- **Recommended Price: $672.00**

### Scenario 3: 6 Months, 0% Profit Margin (Break-even)
- Monthly operational cost: $14.00
- Number of months: 6
- Total operational cost: $84.00
- Profit (0%): $0.00
- **Recommended Price: $84.00**

## Testing Results

### Unit Tests
✅ All tests passed:
- Monthly operational cost calculation: PASS
- Annual operational cost calculation: PASS
- Recommended price calculation with profit: PASS
- Multiple scenario testing: PASS

### Build Status
✅ Project builds successfully with 0 errors
- 95 warnings (all pre-existing, unrelated to new code)

### Security Analysis
✅ CodeQL security scan: PASS
- 0 security vulnerabilities detected in new code

## Usage

### Accessing the Calculator
Navigate to: `/Pricing/Calculator`

### Input Parameters
1. **Number of Months**: How many months of operational costs to cover (1-120)
2. **Profit Margin**: Desired profit percentage above costs (0-1000%)

### Output
- Detailed breakdown of all costs
- Final recommended sale price

## Technical Details

### Dependencies
- ASP.NET Core MVC
- Bootstrap 5 (for UI)
- Bootstrap Icons (for icons)

### File Structure
```
TmsSystem/
├── Controllers/
│   └── PricingController.cs
├── Services/
│   └── SoftwarePricingCalculator.cs
├── ViewModels/
│   └── SoftwarePricingViewModel.cs
└── Views/
    └── Pricing/
        └── Calculator.cshtml
```

### Code Quality
- Well-documented with XML comments in Hebrew and English
- Follows existing project conventions
- Minimal, focused changes
- No modifications to existing functionality

## Screenshots

### Default View (12 months, 50% profit)
Shows the calculator with default values displaying a recommended price of $252.00

### Custom Calculation (24 months, 100% profit)
Shows the calculator with custom values displaying a recommended price of $672.00

## Conclusion

The software pricing calculator successfully addresses the problem statement by:
1. Incorporating the fixed operational costs ($6 server + $8 email server)
2. Allowing flexible calculation based on business needs
3. Providing clear, detailed breakdown of pricing
4. Offering an intuitive, bilingual interface

The implementation is production-ready, secure, and fully tested.
