-- Script to Add Test Orders for Pagination and Print Queue Testing
-- Run this script in SQL Server Management Studio or via command line

USE PrinterAppDb;
GO

-- Variables for IDs (will be retrieved from existing data)
DECLARE @CustomerId1 INT, @CustomerId2 INT, @CustomerId3 INT;
DECLARE @SupplierId1 INT, @SupplierId2 INT;
DECLARE @ProductId1 INT, @ProductId2 INT, @ProductId3 INT;
DECLARE @MachineId1 INT, @MachineId2 INT;
DECLARE @CoreId1 INT, @CoreId2 INT;
DECLARE @KnifeId1 INT, @KnifeId2 INT;
DECLARE @CartonId1 INT, @CartonId2 INT;
DECLARE @MoldId1 INT, @MoldId2 INT;
DECLARE @RawMaterialId1 INT, @RawMaterialId2 INT;
DECLARE @RollDirectionId1 INT, @RollDirectionId2 INT;
DECLARE @UserId NVARCHAR(450);

-- Get existing data IDs (take first available records)
SELECT TOP 1 @CustomerId1 = Id FROM Customers WHERE IsActive = 1;
SELECT TOP 1 @CustomerId2 = Id FROM Customers WHERE IsActive = 1 AND Id <> @CustomerId1;
SELECT TOP 1 @CustomerId3 = Id FROM Customers WHERE IsActive = 1 AND Id NOT IN (@CustomerId1, @CustomerId2);

SELECT TOP 1 @SupplierId1 = Id FROM Suppliers WHERE IsActive = 1;
SELECT TOP 1 @SupplierId2 = Id FROM Suppliers WHERE IsActive = 1 AND Id <> @SupplierId1;

SELECT TOP 1 @ProductId1 = Id FROM Products WHERE IsActive = 1;
SELECT TOP 1 @ProductId2 = Id FROM Products WHERE IsActive = 1 AND Id <> @ProductId1;
SELECT TOP 1 @ProductId3 = Id FROM Products WHERE IsActive = 1 AND Id NOT IN (@ProductId1, @ProductId2);

SELECT TOP 1 @MachineId1 = Id FROM Machines WHERE IsActive = 1;
SELECT TOP 1 @MachineId2 = Id FROM Machines WHERE IsActive = 1 AND Id <> @MachineId1;

SELECT TOP 1 @CoreId1 = Id FROM Cores WHERE IsActive = 1;
SELECT TOP 1 @CoreId2 = Id FROM Cores WHERE IsActive = 1 AND Id <> @CoreId1;

SELECT TOP 1 @KnifeId1 = Id FROM Knives WHERE IsActive = 1;
SELECT TOP 1 @KnifeId2 = Id FROM Knives WHERE IsActive = 1 AND Id <> @KnifeId1;

SELECT TOP 1 @CartonId1 = Id FROM Cartons WHERE IsActive = 1;
SELECT TOP 1 @CartonId2 = Id FROM Cartons WHERE IsActive = 1 AND Id <> @CartonId1;

SELECT TOP 1 @MoldId1 = Id FROM Molds WHERE IsActive = 1;
SELECT TOP 1 @MoldId2 = Id FROM Molds WHERE IsActive = 1 AND Id <> @MoldId1;

SELECT TOP 1 @RawMaterialId1 = Id FROM RawMaterials WHERE IsActive = 1;
SELECT TOP 1 @RawMaterialId2 = Id FROM RawMaterials WHERE IsActive = 1 AND Id <> @RawMaterialId1;

SELECT TOP 1 @RollDirectionId1 = Id FROM RollDirections WHERE IsActive = 1;
SELECT TOP 1 @RollDirectionId2 = Id FROM RollDirections WHERE IsActive = 1 AND Id <> @RollDirectionId1;

SELECT TOP 1 @UserId = Id FROM AspNetUsers;

-- Print information
PRINT '========================================';
PRINT 'Creating Test Orders';
PRINT '========================================';
PRINT 'Customer IDs: ' + CAST(ISNULL(@CustomerId1, 0) AS NVARCHAR) + ', ' + CAST(ISNULL(@CustomerId2, 0) AS NVARCHAR) + ', ' + CAST(ISNULL(@CustomerId3, 0) AS NVARCHAR);
PRINT 'Supplier IDs: ' + CAST(ISNULL(@SupplierId1, 0) AS NVARCHAR) + ', ' + CAST(ISNULL(@SupplierId2, 0) AS NVARCHAR);
PRINT 'Product IDs: ' + CAST(ISNULL(@ProductId1, 0) AS NVARCHAR) + ', ' + CAST(ISNULL(@ProductId2, 0) AS NVARCHAR) + ', ' + CAST(ISNULL(@ProductId3, 0) AS NVARCHAR);
PRINT '========================================';

-- Check if we have necessary data
IF @CustomerId1 IS NULL OR @SupplierId1 IS NULL OR @ProductId1 IS NULL
BEGIN
    PRINT 'ERROR: Required master data not found!';
    PRINT 'Please ensure you have at least:';
    PRINT '  - 1 Customer';
    PRINT '  - 1 Supplier';
    PRINT '  - 1 Product';
    PRINT '  - 1 Machine';
    PRINT '  - 1 Core';
    PRINT '  - 1 Knife';
    PRINT '  - 1 Carton';
    PRINT '  - 1 Mold';
    PRINT '  - 1 RawMaterial';
    PRINT '  - 1 RollDirection';
    RETURN;
END

-- Start transaction
BEGIN TRANSACTION;

BEGIN TRY
    DECLARE @OrderCounter INT = 1;
    DECLARE @MaxOrders INT = 50; -- Create 50 test orders
    DECLARE @OrderNumber NVARCHAR(50);
    DECLARE @RandomStage INT;
    DECLARE @RandomStatus INT;
    DECLARE @RandomPriority INT;
    DECLARE @OrderDate DATETIME2;
    DECLARE @DeliveryDate DATETIME2;

    WHILE @OrderCounter <= @MaxOrders
    BEGIN
        -- Generate order number
        SET @OrderNumber = 'TEST-' + RIGHT('00000' + CAST(@OrderCounter AS NVARCHAR), 5);
        
        -- Random values
        SET @RandomStage = (ABS(CHECKSUM(NEWID())) % 5); -- 0-4 (Order, Review, Manufacturing, Printing, Completed)
        SET @RandomStatus = (ABS(CHECKSUM(NEWID())) % 7); -- 0-6
        SET @RandomPriority = (ABS(CHECKSUM(NEWID())) % 20) + 1; -- 1-20
        
        -- Random dates
        SET @OrderDate = DATEADD(DAY, -(ABS(CHECKSUM(NEWID())) % 30), GETDATE()); -- Last 30 days
        SET @DeliveryDate = DATEADD(DAY, (ABS(CHECKSUM(NEWID())) % 30) + 5, @OrderDate); -- 5-35 days from order date
        
        -- Insert order
        INSERT INTO Orders (
            OrderNumber,
            CustomerId,
            SupplierId,
            ProductId,
            RollDirectionId,
            RawMaterialId,
            Quantity,
            Length,
            Width,
            OrderDate,
            ExpectedDeliveryDate,
            Status,
            Stage,
            Priority,
            OrderNotes,
            IsActive,
            CreatedDate,
            CreatedBy,
            MachineId,
            CoreId,
            KnifeId,
            CartonId,
            MoldId,
            ReviewedBy,
            ReviewNotes,
            ReviewedDate,
            PrintingStartDate,
            PrintedBy,
            PrintingNotes
        )
        VALUES (
            @OrderNumber,
            CASE (ABS(CHECKSUM(NEWID())) % 3) 
                WHEN 0 THEN @CustomerId1 
                WHEN 1 THEN ISNULL(@CustomerId2, @CustomerId1)
                ELSE ISNULL(@CustomerId3, @CustomerId1)
            END,
            CASE (ABS(CHECKSUM(NEWID())) % 2) 
                WHEN 0 THEN @SupplierId1 
                ELSE ISNULL(@SupplierId2, @SupplierId1)
            END,
            CASE (ABS(CHECKSUM(NEWID())) % 3) 
                WHEN 0 THEN @ProductId1 
                WHEN 1 THEN ISNULL(@ProductId2, @ProductId1)
                ELSE ISNULL(@ProductId3, @ProductId1)
            END,
            CASE (ABS(CHECKSUM(NEWID())) % 2) 
                WHEN 0 THEN @RollDirectionId1 
                ELSE ISNULL(@RollDirectionId2, @RollDirectionId1)
            END,
            CASE (ABS(CHECKSUM(NEWID())) % 2) 
                WHEN 0 THEN @RawMaterialId1 
                ELSE ISNULL(@RawMaterialId2, @RawMaterialId1)
            END,
            (ABS(CHECKSUM(NEWID())) % 900) + 100, -- Quantity 100-1000
            (ABS(CHECKSUM(NEWID())) % 80) + 20, -- Length 20-100 cm
            (ABS(CHECKSUM(NEWID())) % 50) + 10, -- Width 10-60 cm
            @OrderDate,
            @DeliveryDate,
            @RandomStatus,
            @RandomStage,
            @RandomPriority,
            N'طلب تجريبي رقم ' + CAST(@OrderCounter AS NVARCHAR) + N' - تم إنشاؤه للاختبار',
            1, -- IsActive
            @OrderDate,
            @UserId,
            -- Add review/manufacturing data for orders in stage 2+ (Review and beyond)
            CASE WHEN @RandomStage >= 1 THEN ISNULL(@MachineId1, NULL) ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN ISNULL(@CoreId1, NULL) ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN ISNULL(@KnifeId1, NULL) ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN ISNULL(@CartonId1, NULL) ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN ISNULL(@MoldId1, NULL) ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN @UserId ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN N'تمت المراجعة - جاهز للتصنيع' ELSE NULL END,
            CASE WHEN @RandomStage >= 1 THEN DATEADD(DAY, 1, @OrderDate) ELSE NULL END,
            -- Add printing data for orders in stage 3+ (Printing)
            CASE WHEN @RandomStage >= 3 THEN DATEADD(DAY, 2, @OrderDate) ELSE NULL END,
            CASE WHEN @RandomStage >= 3 THEN @UserId ELSE '' END,
            CASE WHEN @RandomStage >= 3 THEN N'جاري الطباعة' ELSE N'' END
        );

        DECLARE @NewOrderId INT = SCOPE_IDENTITY();

        -- Add timeline entries based on stage
        IF @RandomStage >= 0 -- Order created
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, 0, @RandomStatus, N'OrderCreated', N'تم إنشاء الطلب', @OrderDate, @UserId, N'System');
        END

        IF @RandomStage >= 1 -- Review stage
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, 1, @RandomStatus, N'MovedToReview', N'تم نقل الطلب إلى مرحلة المراجعة', DATEADD(DAY, 1, @OrderDate), @UserId, N'System');
        END

        IF @RandomStage >= 2 -- Manufacturing stage
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, 2, @RandomStatus, N'ManufacturingStarted', N'بدء التصنيع', DATEADD(DAY, 2, @OrderDate), @UserId, N'System');
        END

        IF @RandomStage >= 3 -- Printing stage
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, 3, 3, N'PrintingStarted', N'بدء الطباعة', DATEADD(DAY, 3, @OrderDate), @UserId, N'System');
        END

        IF @RandomStage >= 4 -- Completed
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, 4, 4, N'OrderCompleted', N'تم إكمال الطلب', DATEADD(DAY, 4, @OrderDate), @UserId, N'System');
        END

        -- Add priority change timeline if priority is not default
        IF @RandomPriority < 15
        BEGIN
            INSERT INTO OrderTimelines (OrderId, Stage, Status, Action, Notes, ActionDate, ActionBy, ActionByName)
            VALUES (@NewOrderId, @RandomStage, @RandomStatus, N'PriorityChanged',
                    N'تم تغيير الأولوية إلى ' + CAST(@RandomPriority AS NVARCHAR), 
                    DATEADD(HOUR, 1, @OrderDate), @UserId, N'System');
        END

        SET @OrderCounter = @OrderCounter + 1;
        
        -- Print progress every 10 orders
        IF @OrderCounter % 10 = 0
        BEGIN
            PRINT 'Created ' + CAST(@OrderCounter AS NVARCHAR) + ' orders...';
        END
    END

    -- Commit transaction
    COMMIT TRANSACTION;
    
    PRINT '========================================';
    PRINT 'SUCCESS: Created ' + CAST(@MaxOrders AS NVARCHAR) + ' test orders!';
    PRINT '========================================';
    
    -- Show summary
    PRINT '';
    PRINT 'Orders by Stage:';
    SELECT 
        CASE Stage 
            WHEN 0 THEN 'Order'
            WHEN 1 THEN 'Review'
            WHEN 2 THEN 'Manufacturing'
            WHEN 3 THEN 'Printing'
            WHEN 4 THEN 'Completed'
        END AS StageName,
        COUNT(*) AS OrderCount
    FROM Orders
    WHERE OrderNumber LIKE 'TEST-%'
    GROUP BY Stage
    ORDER BY Stage;
    
    PRINT '';
    PRINT 'Orders by Status:';
    SELECT 
        CASE Status
            WHEN 0 THEN 'Pending'
            WHEN 1 THEN 'UnderReview'
            WHEN 2 THEN 'InManufacturing'
            WHEN 3 THEN 'InPrinting'
            WHEN 4 THEN 'Completed'
            WHEN 5 THEN 'Cancelled'
            WHEN 6 THEN 'OnHold'
        END AS StatusName,
        COUNT(*) AS OrderCount
    FROM Orders
    WHERE OrderNumber LIKE 'TEST-%'
    GROUP BY Status
    ORDER BY Status;
    
    PRINT '';
    PRINT 'Printing Stage Orders (for Print Queue):';
    SELECT COUNT(*) AS PrintingOrdersCount
    FROM Orders
    WHERE OrderNumber LIKE 'TEST-%' AND Stage = 3;

END TRY
BEGIN CATCH
    -- Rollback on error
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '========================================';
    PRINT 'ERROR: Failed to create test orders!';
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT '========================================';
END CATCH;

GO

-- Show sample of created orders
PRINT '';
PRINT 'Sample of Created Orders:';
SELECT TOP 10
    OrderNumber,
    CASE Stage 
        WHEN 0 THEN 'Order'
        WHEN 1 THEN 'Review'
        WHEN 2 THEN 'Manufacturing'
        WHEN 3 THEN 'Printing'
        WHEN 4 THEN 'Completed'
    END AS Stage,
    CASE Status
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'UnderReview'
        WHEN 2 THEN 'InManufacturing'
        WHEN 3 THEN 'InPrinting'
        WHEN 4 THEN 'Completed'
        WHEN 5 THEN 'Cancelled'
        WHEN 6 THEN 'OnHold'
    END AS Status,
    Priority,
    Quantity,
    OrderDate,
    ExpectedDeliveryDate
FROM Orders
WHERE OrderNumber LIKE 'TEST-%'
ORDER BY OrderNumber;

PRINT '';
PRINT 'Done! You can now test pagination and print queue features.';
