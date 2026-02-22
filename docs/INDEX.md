# Documentation Index

Welcome to the Learning Microservices documentation!

## 📚 Documentation Structure

### Main Documentation
- **[README.md](../ReadMe.md)** - Project overview, quick start guide, and feature list
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detailed architecture documentation
- **[DIAGRAMS.md](DIAGRAMS.md)** - Visual architecture diagrams reference

---

## 🎯 Quick Navigation

### For First-Time Readers
1. Start with [README.md](../ReadMe.md) to understand the project
2. Review [Project Structure](../ReadMe.md#project-structure) to see how code is organized
3. Explore [Architecture Patterns](../ReadMe.md#architecture-patterns) we implement

### For Developers
1. Read [Developer Guidelines](../ReadMe.md#developer-guidelines)
2. Review [Architecture Documentation](ARCHITECTURE.md)
3. Study [System Architecture](ARCHITECTURE.md#system-architecture)
4. Understand [Messaging Architecture](ARCHITECTURE.md#messaging-architecture)

### For Architects
1. Review [Architectural Principles](ARCHITECTURE.md#architectural-principles)
2. Study [C4 Model Diagrams](ARCHITECTURE.md#c4-model---system-context)
3. Explore [Service Architecture](ARCHITECTURE.md#service-architecture)
4. Understand [Data Architecture](ARCHITECTURE.md#data-architecture)
5. Review [Deployment Architecture](ARCHITECTURE.md#deployment-architecture)

### For DevOps/SRE
1. Check [Deployment Architecture](ARCHITECTURE.md#deployment-architecture)
2. Review [Observability](ARCHITECTURE.md#monitoring--observability)
3. Study [Scalability Considerations](ARCHITECTURE.md#scalability-considerations)

---

## 📖 Documentation Contents

### README.md
- Introduction & Technologies
- Project Structure
- Architecture Patterns
- Microservices Overview
- Developer Guidelines
- Solution Diagrams (High-Level)
- Getting Started
- Features & Roadmap

### ARCHITECTURE.md
Comprehensive architectural documentation covering:
- **Architectural Principles** - Core design principles and constraints
- **System Architecture** - C4 model diagrams (Context & Container)
- **Service Architecture** - Clean architecture layers, dependency flow
- **Data Architecture** - Database schemas, consistency patterns
- **Messaging Architecture** - Event types, message flow, outbox pattern
- **Infrastructure Architecture** - Shared projects, service discovery
- **Deployment Architecture** - Local dev, containers, Kubernetes
- **Scalability Considerations** - Horizontal scaling, database scaling
- **Security Architecture** - Authentication & authorization flows
- **Monitoring & Observability** - OpenTelemetry, metrics, traces

### DIAGRAMS.md
Visual reference guide with detailed diagrams:
- **System Overview** - Microservices ecosystem
- **Layer Architecture** - Clean architecture detailed view
- **Data Flow** - Request/response flow, CQRS pattern
- **Messaging Patterns** - Pub/Sub, Saga, Event Sourcing
- **Deployment Diagrams** - Aspire, Docker Compose, Kubernetes
- **Network & Communication** - Service mesh, API gateway routing
- **Observability** - Distributed tracing, metrics collection
- **Error Handling** - Exception flow
- **Database Patterns** - Repository pattern, Unit of Work

---

## 🔍 Find What You Need

### Understanding Clean Architecture
- [Clean Architecture Layers (README)](../ReadMe.md#clean-architecture-layers-userservice--fully-implemented)
- [Service Architecture (ARCHITECTURE)](ARCHITECTURE.md#service-architecture)
- [Layer Architecture (DIAGRAMS)](DIAGRAMS.md#layer-architecture)

### Understanding Microservices Communication
- [Inter-Service Communication Flow (README)](../ReadMe.md#inter-service-communication-flow)
- [Messaging Architecture (ARCHITECTURE)](ARCHITECTURE.md#messaging-architecture)
- [Messaging Patterns (DIAGRAMS)](DIAGRAMS.md#messaging-patterns)

### Understanding Data Management
- [Data Architecture (ARCHITECTURE)](ARCHITECTURE.md#data-architecture)
- [Database Patterns (DIAGRAMS)](DIAGRAMS.md#database-patterns)
- [Outbox Pattern Flow (README)](../ReadMe.md#outbox-pattern-flow)

### Understanding Deployment
- [Getting Started (README)](../ReadMe.md#getting-started)
- [Deployment Architecture (ARCHITECTURE)](ARCHITECTURE.md#deployment-architecture)
- [Deployment Diagrams (DIAGRAMS)](DIAGRAMS.md#deployment-diagrams)

---

## 🛠️ Technology Stack

### Frontend
- **Blazor WebAssembly** - C# for web UI

### Backend
- **.NET 10** - Latest .NET platform
- **C# 14** - Modern C# features
- **ASP.NET Core** - Web API framework

### Orchestration
- **.NET Aspire** - Cloud-native orchestration and service discovery

### Data
- **PostgreSQL** - Relational database (one per service)
- **Entity Framework Core** - ORM
- **Redis** - Caching layer

### Messaging
- **RabbitMQ** - Message broker
- **MassTransit** - Messaging abstraction

### API Gateway
- **YARP** - Reverse proxy

### Observability
- **OpenTelemetry** - Distributed tracing and metrics
- **Aspire Dashboard** - Development-time observability

---

## 📊 Key Architectural Patterns

### Implemented
✅ **Clean Architecture** - Layer separation with dependency inversion  
✅ **Domain-Driven Design** - Rich domain models with entities, value objects, and events  
✅ **Microservices** - Independent services with their own databases  
✅ **Database per Service** - Data isolation and autonomy  
✅ **Outbox Pattern** - Reliable event publishing  
✅ **Repository Pattern** - Data access abstraction  
✅ **Unit of Work** - Transactional consistency  
✅ **Client SDK Pattern** - Type-safe inter-service communication  
✅ **API Gateway** - Single entry point with YARP  
✅ **Service Discovery** - Aspire-based dynamic service location  

### Planned
🚧 **CQRS** - Separate read/write models  
🚧 **Event Sourcing** - Event-based state reconstruction  
🚧 **Saga Pattern** - Distributed transaction coordination  
🚧 **Circuit Breaker** - Resilience pattern  

---

## 🎓 Learning Path

### Week 1: Fundamentals
1. Read the [Introduction](../ReadMe.md#introduction)
2. Understand [Project Structure](../ReadMe.md#project-structure)
3. Explore [Architecture Patterns](../ReadMe.md#architecture-patterns)
4. Set up the project using [Getting Started](../ReadMe.md#getting-started)

### Week 2: Clean Architecture
1. Study [Clean Architecture Layers](../ReadMe.md#clean-architecture-layers-userservice--fully-implemented)
2. Understand [Dependency Flow](ARCHITECTURE.md#dependency-flow)
3. Review `UserService` implementation
4. Build a simple feature in `UserService`

### Week 3: Domain-Driven Design
1. Study `UserService.Domain` project structure
2. Understand [Entities](ARCHITECTURE.md#shared-base-entity-pattern)
3. Learn about Value Objects (`Email`, `FullName`, `MobileNo`)
4. Understand Domain Events
5. Implement a new entity with domain events

### Week 4: Microservices Communication
1. Study [Messaging Architecture](ARCHITECTURE.md#messaging-architecture)
2. Understand [Outbox Pattern](../ReadMe.md#outbox-pattern-flow)
3. Review [Event Types](ARCHITECTURE.md#event-types)
4. Implement event publishing and consumption

### Week 5: Infrastructure
1. Study [Infrastructure Projects](ARCHITECTURE.md#shared-infrastructure-projects)
2. Understand `BaseDbContext` and `BaseRepository`
3. Learn about Entity Framework configurations
4. Create and run migrations

### Week 6: Advanced Topics
1. Explore [Scalability Considerations](ARCHITECTURE.md#scalability-considerations)
2. Study [Security Architecture](ARCHITECTURE.md#security-architecture)
3. Learn about [Monitoring & Observability](ARCHITECTURE.md#monitoring--observability)
4. Understand deployment strategies

---

## 🤝 Contributing

When contributing to documentation:
1. Keep diagrams up to date with code changes
2. Use Mermaid for all diagrams
3. Follow the existing structure
4. Update this index if adding new documentation

---

## 📞 Support

- **GitHub Repository**: [LearningMircoservices](https://github.com/Asifnawazmiani/LearningMircoservices)
- **Issues**: Report issues or ask questions via GitHub Issues
- **Discussions**: Use GitHub Discussions for general questions

---

## 📝 License

This project is for educational purposes. See the main [README](../ReadMe.md) for details.

---

*Last Updated: 2025*
