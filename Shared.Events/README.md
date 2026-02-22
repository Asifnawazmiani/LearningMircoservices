# Shared.Events

This project contains integration events shared across all microservices.

## Structure (to be implemented):
- `/Events` - Integration events (e.g., UserCreatedIntegrationEvent, OrderPlacedIntegrationEvent)
- `/Contracts` - Shared event contracts

## Usage:
Integration events are used for cross-service communication via message broker (RabbitMQ/MassTransit).

## Note:
These are different from Domain Events (which are internal to a service).
