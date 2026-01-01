# Soft Delete Implementation for Customers (מחיקה רכה של לקוחות)

## Overview (סקירה כללית)
This document describes the implementation of soft delete functionality for customers in the TMS System. Instead of physically deleting customer records from the database (which could cause foreign key constraint violations), customers are now marked as "deleted" using an `IsDeleted` flag.

מסמך זה מתאר את המימוש של פונקציונליות מחיקה רכה עבור לקוחות במערכת TMS. במקום למחוק פיזית רשומות לקוחות ממסד הנתונים (שיכול לגרום להפרות של מגבלות מפתח זר), לקוחות מסומנים כעת כ"נמחקו" באמצעות דגל `IsDeleted`.

## Problem Statement (הגדרת הבעיה)
When attempting to delete a customer who has associated offers (הצעות מחיר), the system would encounter a foreign key constraint error. This is because the offers table has a foreign key relationship with the customers table.

בעת ניסיון למחוק לקוח שיש לו הצעות מחיר משויכות, המערכת נתקלת בשגיאת אילוץ מפתח זר. זאת משום שטבלת ההצעות מחיר כוללת קשר מפתח זר לטבלת הלקוחות.

## Solution (הפתרון)
Implement soft delete pattern by:
1. Adding an `IsDeleted` boolean field to the Customer model
2. Modifying the Delete action to set `IsDeleted = true` instead of removing the record
3. Filtering out deleted customers from all queries by default
4. Adding UI filtering to show active, inactive, or all customers

## Changes Made (שינויים שבוצעו)

### 1. Customer Model (`Models/Customer.cs`)
Added the `IsDeleted` property:
```csharp
public bool IsDeleted { get; set; } = false;
```

### 2. CustomersController (`Controllers/CustomersController.cs`)
- **Index Method**: Added filtering parameter to show active, inactive, or all customers
- **Delete Method**: Changed from `Remove()` to setting `IsDeleted = true`

### 3. Views (`Views/Customers/Index.cshtml`)
- Added filter dropdown (פעילים/לא פעילים/כולם)
- Added status badge to show customer state
- Disabled delete button for already-deleted customers
- Grey out inactive customer rows

### 4. Other Controllers Updated
All controllers that query customers now filter out deleted customers by default:
- `OffersController`
- `TripOffersController`
- `BookingsController`
- `DashboardController`
- `HomeController`

## Database Migration (העברת מסד הנתונים)

### SQL Script to Add IsDeleted Column
Execute the following SQL script on your MySQL database:

```sql
-- Add the IsDeleted column to customers table
ALTER TABLE customers 
ADD COLUMN IsDeleted TINYINT(1) NOT NULL DEFAULT 0;

-- Create an index for better query performance
CREATE INDEX idx_customers_isdeleted ON customers(IsDeleted);
```

### Alternative: Using Entity Framework Migration
If you prefer to use Entity Framework migrations:

```bash
# Create migration
dotnet ef migrations add AddIsDeletedToCustomer

# Update database
dotnet ef database update
```

## Features (תכונות)

### Customer List Filtering
The customer list now has a dropdown filter with three options:
- **פעילים (Active)**: Shows only active customers (default)
- **לא פעילים (Inactive)**: Shows only deleted/inactive customers
- **כולם (All)**: Shows all customers regardless of status

### Visual Indicators
- Active customers have a green "פעיל" (Active) badge
- Inactive customers have a red "לא פעיל" (Inactive) badge
- Inactive customer rows are displayed with a grey background
- Delete button is hidden for already-deleted customers

### Data Integrity
- Deleted customers are hidden from dropdown lists when creating new offers/bookings
- Dashboard statistics count only active customers
- Foreign key relationships are preserved

## Testing Checklist (רשימת בדיקות)

- [ ] Verify customer can be marked as deleted without database errors
- [ ] Confirm deleted customers don't appear in offer creation dropdowns
- [ ] Check that filter dropdown works correctly (Active/Inactive/All)
- [ ] Validate that customer count on dashboard excludes deleted customers
- [ ] Test that edited customer data is saved correctly
- [ ] Ensure existing offers for deleted customers still work

## Rollback Instructions (הוראות חזרה אחורה)
If you need to rollback this change:

```sql
-- Remove the IsDeleted column
ALTER TABLE customers DROP COLUMN IsDeleted;

-- Remove the index
DROP INDEX idx_customers_isdeleted ON customers;
```

Then revert the code changes in your repository.

## Notes (הערות)
- The `IsDeleted` field defaults to `false` for all new customers
- Existing customers will have `IsDeleted = false` after running the migration script
- This implementation preserves data integrity and prevents foreign key constraint violations
- Deleted customers can be "restored" by manually setting `IsDeleted = false` in the database if needed

## Support (תמיכה)
For questions or issues, please contact the development team.
