# IoTRag - IoT RAG System
**Complete Azure IoT system with vector-based RAG over operational manuals**

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                                                 │
│  [IoT Devices] → Event Hubs → Functions → Cosmos DB  │
│                                                 │
│  [Documents] → Blob Storage → Functions → Cosmos DB  │
│           (Vector Search + RAG)                      │
│                                                 │
│  [Chat Interface] ← HTTP Functions ← RAG Service   │
│      (Operators query device status + troubleshooting)      │
└─────────────────────────────────────────────────────────────┘
```

## 🎯 Features

- **Real-time telemetry ingestion** from industrial IoT devices
- **Anomaly detection** with configurable thresholds
- **Service Bus alerting** for critical conditions
- **Document chunking & embedding** for knowledge base
- **Vector search** in Cosmos DB (native vector similarity)
- **RAG-powered chat interface** with device context
- **Complete audit logging** for all operations
- **Local Azure emulator support** for zero-cost development

## 🛠️ Technology Stack

- **Backend**: C# (.NET 8), Azure Functions v4
- **Database**: Azure Cosmos DB (NoSQL with vector search)
- **Messaging**: Azure Event Hubs, Service Bus
- **Storage**: Azure Blob Storage (Azurite emulator)
- **Embeddings**: Mock (deterministic) or Azure OpenAI
- **Frontend**: HTTP API (no UI required)
- **Infrastructure**: Docker Compose (all emulators)
- **Development**: Azure Functions Core Tools

## 📁 Quick Start

```bash
# 1. Clone and start emulators
git clone https://github.com/yourusername/IoTRag-System
cd IoTRag-System
docker-compose up -d

# 2. Create Cosmos DB resources
# Open https://localhost:8081/_explorer/index.html
# Create database: iotdb
# Create containers: documents, telemetry, devices, alerts

# 3. Run the system
cd FunctionApp && func start                    # Terminal 1
cd TelemetryGenerator && dotnet run          # Terminal 2
```

## 📖 Documentation

- [Architecture](./docs/architecture.md)
- [API Reference](./docs/api.md)
- [Development Guide](./docs/development.md)
- [Deployment Guide](./docs/deployment.md)
- [Troubleshooting](./docs/troubleshooting.md)

## 🧪 Testing

```bash
# Run comprehensive test suite
./scripts/test-all.sh
```

## 🚀 Deployment

Production-ready with ARM templates and CI/CD pipelines.

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

---

**Built with ❤️ for industrial IoT operations teams**