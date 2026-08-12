/* ============================================================
   Payment System - Schema
   Run against a fresh database, e.g.:
       CREATE DATABASE PaymentSystemDb;
       GO
       USE PaymentSystemDb;
       GO
   ============================================================ */

IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL
    DROP TABLE dbo.Payments;
GO

CREATE TABLE dbo.Payments
(
    Id            INT IDENTITY(1,1)     NOT NULL,
    ReferenceNo   VARCHAR(30)           NOT NULL,
    MerchantName  VARCHAR(120)          NOT NULL,
    CardType      VARCHAR(20)           NOT NULL,   -- VISA / MASTERCARD / TROY / AMEX
    Last4         CHAR(4)               NOT NULL,
    Amount        DECIMAL(18,2)         NOT NULL,
    Currency      CHAR(3)               NOT NULL DEFAULT 'TRY',
    Status        VARCHAR(15)           NOT NULL,   -- SUCCESS / FAILED / PENDING / REFUNDED
    FailReason    VARCHAR(200)          NULL,
    CreatedAt     DATETIME2             NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Payments PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Payments_ReferenceNo UNIQUE (ReferenceNo),
    CONSTRAINT CK_Payments_Amount CHECK (Amount > 0),
    CONSTRAINT CK_Payments_Status CHECK (Status IN ('SUCCESS','FAILED','PENDING','REFUNDED'))
);
GO

CREATE NONCLUSTERED INDEX IX_Payments_CreatedAt ON dbo.Payments (CreatedAt) INCLUDE (Amount, Status);
GO
CREATE NONCLUSTERED INDEX IX_Payments_Status ON dbo.Payments (Status);
GO

/* ============================================================
   Daily volume + moving average report
   Used by ReportsController -> GET /api/reports/daily-volume
   ============================================================ */
IF OBJECT_ID('dbo.usp_GetDailyVolumeReport', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetDailyVolumeReport;
GO

CREATE PROCEDURE dbo.usp_GetDailyVolumeReport
    @DaysBack INT = 30
AS
BEGIN
    SET NOCOUNT ON;

    WITH DailyTotals AS (
        SELECT
            CAST(CreatedAt AS DATE) AS TxnDate,
            SUM(Amount)             AS TotalAmount,
            COUNT(*)                AS TxnCount
        FROM dbo.Payments
        WHERE Status = 'SUCCESS'
          AND CreatedAt >= DATEADD(DAY, -@DaysBack, SYSUTCDATETIME())
        GROUP BY CAST(CreatedAt AS DATE)
    )
    SELECT
        TxnDate,
        TotalAmount,
        TxnCount,
        AVG(TotalAmount) OVER (ORDER BY TxnDate ROWS BETWEEN 6 PRECEDING AND CURRENT ROW) AS MovingAvg7Day,
        SUM(TotalAmount) OVER (ORDER BY TxnDate) AS RunningTotal,
        LAG(TotalAmount) OVER (ORDER BY TxnDate) AS PrevDayAmount,
        CASE
            WHEN LAG(TotalAmount) OVER (ORDER BY TxnDate) IS NULL THEN NULL
            ELSE ROUND(
                (TotalAmount - LAG(TotalAmount) OVER (ORDER BY TxnDate))
                / NULLIF(LAG(TotalAmount) OVER (ORDER BY TxnDate), 0) * 100, 2)
        END AS DayOverDayChangePct
    FROM DailyTotals
    ORDER BY TxnDate;
END
GO
