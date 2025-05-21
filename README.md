# TLR Log Processing Service

This service processes large Telco TLR log files, parses SMS-related records, and inserts them efficiently into a SQL Server database in batches using Dapper. It is designed to handle files larger than 500MB using memory-efficient streaming and background job processing via Hangfire.

---

## 🚀 Features

- Efficiently parses large `.txt` TLR log files line by line
- Maps lines into `SmsTlrRecord` objects
- Batch inserts records using Dapper
- Runs processing jobs via Hangfire Background Jobs
- Supports large file uploads beyond the default request limit
- Supports `GUID`-based primary keys (`Id`) for traceability

---

## 📦 Technologies Used

- C# / .NET 7 (or .NET 6+)
- Dapper
- SQL Server
- Hangfire
- ASP.NET Core Web API

---

## 🛠️ Configuration

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=TLRDb;User Id=USERNAME;Password=PASSWORD;"
  },
  "HangfireSettings": {
    "DashboardPath": "/hangfire"
  }
}
