# IoT RAG System (Local Emulators)

This repository scaffolds a **complete, runnable end-to-end IoT RAG system** using Azure emulators locally. The system ingests telemetry from industrial devices via Event Hubs, detects anomalies, forwards alerts through Service Bus, stores everything in Cosmos DB, and exposes an HTTP chat interface powered by vector-based RAG over operational manuals.

## Technology Stack

- **Language:** C# (.NET 7 for Functions, .NET 8 for tests)
- **Database:** Cosmos DB (emulator, NoSQL API)
- **Messaging:** Event Hubs + Service Bus (emulators)
- **Storage:** Blob Storage (Azurite emulator)
- **Compute:** Azure Functions (local Core Tools)
- **Orchestration:** Docker Compose (all emulators together)

## System Architecture

### Data Flow

1. **Telemetry Ingestion:** IoT devices → Event Hubs emulator
2. **IngestTelemetry Function:** Reads telemetry, detects anomalies, publishes alerts to Service Bus
3. **ProcessAlerts Function:** Reads alerts, updates device state, forwards combined alert (device + telemetry) to combined-alerts queue
4. **Document Ingestion:** SOP files uploaded to Azurite Blob Storage → IngestDocuments Function chunks and embeds them → Cosmos DB
5. **RAG Query:** Operator asks question via HTTP POST → ChatWithRag Function performs vector search + returns answer with sources + recent telemetry

### Containers & Schema

**Cosmos DB Database:** `iotdb`

| Container | Partition Key | Purpose |
| --- | --- | --- |
| `documents` | `/deviceType` | SOP/manual chunks with embeddings |
| `telemetry` | `# Use Azure Storage Explorer to connect to http://127.0.0.1:10010
# Upload SampleData/* files to "manuals" container
# Or use Azure CLI if you have it configured` | Device sensor readings |
| `devices` | `/deviceId` | Device state / alerts |
| `alerts` | `/deviceId` | Alert log |

---

## Quick Start

### 1. Prerequisites

- Docker Desktop (or Docker + Docker Compose)
- .NET 8 SDK
- Azure Functions Core Tools v4
- Git

### 2. Start Emulators

```powershell
cd c:/Users/Khush/OneDrive/Desktop/IoTRag
docker-compose up -d
docker-compose ps
```

Expected output: 4 containers running (Azurite on 10010-10012, Cosmos on 8082, Event Hubs on 5671-5672, Service Bus on 5673).

### 3. Create Cosmos DB Database & Containers

Navigate to **Cosmos DB Explorer** at:
```
https://localhost:8082/_explorer/index.html
```

Create database `iotdb` with four containers:
- `documents` (partition key: `/deviceType`)
- `telemetry` (partition key: `/deviceId`)
- `devices` (partition key: `/deviceId`)
- `alerts` (partition key: `/deviceId`)

### 4. Run Unit Tests

```powershell
cd c:/Users/Khush/OneDrive/Desktop/IoTRag/FunctionApp.Tests
dotnet test
```

Expected: **2 tests passed** (EmbeddingService determinism + RagService RAG query).

### 5. Start Azure Functions Host (One Terminal)

```powershell
cd c:/Users/Khush/OneDrive/Desktop/IoTRag/FunctionApp
func start
```

Expected: Functions running on `http://localhost:7071`

Functions available:
- `POST /api/chat` — RAG query endpoint
- `EventHubTrigger: IngestTelemetry` — Reads from Event Hubs
- `ServiceBusTrigger: ProcessAlerts` — Reads from Service Bus alerts queue
- `BlobTrigger: IngestDocuments` — Reads from Blob Storage manuals container
- `TimerTrigger: CleanupOldTelemetry` — Scheduled cleanup (every 6 hours)

### 6. Start Telemetry Generator (New Terminal)

```powershell
cd c:/Users/Khush/OneDrive/Desktop/IoTRag/TelemetryGenerator
dotnet run
```

Expected: Events flowing every 3 seconds:
```
[HH:mm:ss] Generated: {"deviceId":"pump-001","timestamp":"...","temperatureC":72.5,"vibrationMm":0.45,"isAnomaly":false}
[HH:mm:ss] Generated: {"deviceId":"compressor-002","timestamp":"...","temperatureC":88.2,"vibrationMm":1.2,"isAnomaly":true}
...
Batch sent at 2026-01-10T21:30:00...
```

### 7. Upload Sample Documents to Blob Storage

**Automated (Recommended):**

```powershell
cd c:/Users/Khush/OneDrive/Desktop/IoTRag
.\upload-samples.ps1
```

Expected output:
```
Connecting to Azurite Blob Storage...
Uploading 3 files to 'manuals' container...
  Uploading: compressor-troubleshooting.txt
     ✓ Uploaded successfully
  Uploading: pump-maintenance-sop.txt
     ✓ Uploaded successfully
  Uploading: safety-procedures.txt
     ✓ Uploaded successfully

✅ All 3 files uploaded to Azurite!
Container: manuals
Endpoint: http://127.0.0.1:10010
```

**Alternative: Azure Storage Explorer (GUI)**

1. Open [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/)
2. Click **Add Account** → **Use a connection string** → paste:
   ```
   DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10010/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10011/devstoreaccount1;TableEndpoint=http://127.0.0.1:10012/devstoreaccount1;
   ```
3. Right-click **devstoreaccount1** → **Create Blob Container** → name it `manuals`
4. Upload files from `SampleData/` folder to the `manuals` container

**Using curl:**
```bash
curl -X POST http://localhost:7071/api/chat \
  -H "Content-Type: application/json" \
  -d '{"deviceId":"pump-001","question":"Why is pump-001 showing high temperature? What should I check?"}'
```

**Using the web UI:**
Open [index.html](index.html) in a browser, fill in device ID and question, and click **Ask**.

**Expected response:**
```json
{
  "answer": "Based on the operational manuals and current telemetry:\n✅ Device operating normally. Current temperature: 72.5°C, Vibration: 0.45mm.\n\n**Relevant Maintenance Guidance:**\n- Check lubrication levels every 100 operating hours...",
  "sources": ["pump-maintenance-sop.txt (chunk 0)"],
  "recentTelemetry": {"deviceId":"pump-001","temperatureC":72.5,"vibrationMm":0.45,"isAnomaly":false}
}
```

---

## Monitoring & Debugging

### View Function Logs

In the terminal running `func start`, you'll see detailed logs:
```
[timestamp] [Information] Ingested telemetry: pump-001 @ 2026-01-10T21:30:45, Temp=75.2°C, Vibration=0.5mm, Anomaly=False
[timestamp] [Warning] ANOMALY DETECTED on pump-002, alert queued <alertId>
[timestamp] [Information] Processed alert for pump-002: Anomaly detected: high temp/vibration
[timestamp] [Information] Chat query for device pump-001: Why is pump-001 showing high temperature?
```

### Query Cosmos DB

In Cosmos Explorer, run SQL queries:
```sql
-- View recent telemetry
SELECT * FROM c WHERE c.deviceId = 'pump-001' ORDER BY c.timestamp DESC

-- View alerts
SELECT * FROM c WHERE c.deviceId = 'pump-002'

-- View documents (chunks)
SELECT c.id, c.deviceType, c.chunkText FROM c WHERE c.deviceType = 'pump'
```

### Check Blob Storage

Use Azure Storage Explorer → connect to `http://127.0.0.1:10010` → view `manuals` container.

---

## Project Structure

```
IoTRag/
├── docker-compose.yml                      # Emulator definitions
├── FunctionApp/                            # Azure Functions
│   ├── Program.cs                          # DI setup
│   ├── IotRagFunctionApp.csproj           # Package refs
│   ├── local.settings.json                 # Connection strings
│   ├── Models/                             # Data models
│   ├── Services/                           # Business logic
│   │   ├── CosmosService.cs
│   │   ├── EmbeddingService.cs
│   │   ├── RagService.cs
│   │   └── ServiceBusSenders.cs
│   └── Functions/                          # Function triggers
│       ├── IngestTelemetry.cs              # Event Hub → Cosmos/SB
│       ├── ProcessAlerts.cs                # Service Bus → Cosmos/SB
│       ├── IngestDocuments.cs              # Blob → Cosmos (embeddings)
│       ├── ChatWithRag.cs                  # HTTP → RAG query
│       └── CleanupOldTelemetry.cs          # Timer trigger
├── FunctionApp.Tests/                      # xUnit tests
│   ├── EmbeddingServiceTests.cs
│   └── RagServiceTests.cs
├── TelemetryGenerator/                     # Console app
│   └── Program.cs                          # Event Hubs sender
├── SampleData/                             # Documentation samples
│   ├── pump-maintenance-sop.txt
│   ├── compressor-troubleshooting.txt
│   └── safety-procedures.txt
└── index.html                              # Web UI for chat
```

---

## Key Logs & Debugging

### Example Flow Logs

When running the full system end-to-end:

1. **Telemetry received and anomaly detected:**
   ```
   [21:30:15] Ingested telemetry: pump-002 @ 2026-01-10T21:30:15, Temp=92.3°C, Vibration=1.5mm, Anomaly=True
   [21:30:15] ANOMALY DETECTED on pump-002, alert queued d1a2b3c4-e5f6-g7h8-i9j0-k1l2m3n4o5p6
   ```

2. **Alert processed and forwarded:**
   ```
   [21:30:16] Processed alert for pump-002: Anomaly detected: high temp/vibration
   [21:30:16] Updated device pump-002 status to alert
   [21:30:16] Forwarded combined alert for pump-002 to combined-alerts-queue
   ```

3. **RAG query:**
   ```
   [21:30:30] Chat query for device pump-002: Why is pump-002 showing high temperature?
   ```

---

## Next Steps

1. **Replace Mock Embeddings:** Integrate Azure OpenAI Embeddings API for real embeddings.
2. **Add LLM Response:** Use Azure OpenAI Chat Completions API instead of mock answers.
3. **Add Authentication:** Implement Azure AD / API keys for Functions.
4. **Scale to Cloud:** Deploy Functions App Service to Azure with real Cosmos DB and Event Hubs.
5. **Build Web UI:** Replace simple HTML with a React/Vue app.
6. **Add Alerting:** Send email/SMS via SendGrid or Twilio when anomalies detected.

---

## Troubleshooting

| Issue | Solution |
| --- | --- |
| **Port conflicts (8081, 10000, etc.)** | docker-compose already remaps to 8082, 10010-10012. Verify in docker-compose ps. |
| **Cosmos connection fails** | Ensure `local.settings.json` CosmosConnection points to `localhost:8082`. |
| **Event Hubs not receiving events** | Check EventHubsConnection in `local.settings.json`; verify telemetry generator is running. |
| **Embeddings not working** | Using MockEmbeddingService (deterministic hashing). Replace with Azure OpenAI for production. |
| **Functions won't start** | Ensure Docker emulators are healthy: `docker-compose ps` should show all containers "Up". |
| **Tests fail with .NET version** | Test project targets net8.0; FunctionApp targets net7.0 (Functions compatibility). |

---

## Summary

This local setup provides a **sandbox for mastering Azure IoT patterns and RAG without cloud costs**. All services run in containers and local Functions host. Perfect for prototyping, testing, and learning before cloud deployment.
