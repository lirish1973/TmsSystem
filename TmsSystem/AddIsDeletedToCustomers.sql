-- SQL Script to add IsDeleted column to customers table
-- This script adds the IsDeleted column with a default value of 0 (false)

-- Add the IsDeleted column
ALTER TABLE customers 
ADD COLUMN IsDeleted TINYINT(1) NOT NULL DEFAULT 0;

-- Optional: Create an index on IsDeleted for better query performance
CREATE INDEX idx_customers_isdeleted ON customers(IsDeleted);

-- Verify the column was added
-- SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
-- WHERE TABLE_NAME = 'customers' AND COLUMN_NAME = 'IsDeleted';
