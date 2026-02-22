# API Gateway Service Tests
# This script tests all three microservices through the API Gateway

$gatewayUrl = "https://localhost:7000" # Update with actual gateway port

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Microservices via API Gateway" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ==========================================
# User Service Tests
# ==========================================
Write-Host ">>> Testing User Service" -ForegroundColor Yellow
Write-Host ""

# Create User
Write-Host "1. Creating User..." -ForegroundColor Green
$createUserBody = @{
    email = "test@example.com"
    firstName = "John"
    lastName = "Doe"
    passwordHash = "hashedpassword123"
} | ConvertTo-Json

try {
    $createUserResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/users" `
        -Method Post `
        -Body $createUserBody `
        -ContentType "application/json"
    
    Write-Host "✓ User created successfully" -ForegroundColor Green
    Write-Host "User ID: $($createUserResponse.id)" -ForegroundColor Gray
    $userId = $createUserResponse.id
}
catch {
    Write-Host "✗ Failed to create user: $_" -ForegroundColor Red
}

Write-Host ""

# Get User
Write-Host "2. Getting User..." -ForegroundColor Green
try {
    $getUserResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/users/$userId" `
        -Method Get
    
    Write-Host "✓ User retrieved successfully" -ForegroundColor Green
    Write-Host "Email: $($getUserResponse.email)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ Failed to get user: $_" -ForegroundColor Red
}

Write-Host ""

# ==========================================
# Inventory Service Tests
# ==========================================
Write-Host ">>> Testing Inventory Service" -ForegroundColor Yellow
Write-Host ""

# Create Inventory
Write-Host "1. Creating Product Inventory..." -ForegroundColor Green
$createInventoryBody = @{
    productId = [Guid]::NewGuid().ToString()
    quantityOnHand = 100
    reorderLevel = 20
    reorderQuantity = 50
} | ConvertTo-Json

try {
    $createInventoryResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/inventory" `
        -Method Post `
        -Body $createInventoryBody `
        -ContentType "application/json"
    
    Write-Host "✓ Inventory created successfully" -ForegroundColor Green
    Write-Host "Inventory ID: $($createInventoryResponse.id)" -ForegroundColor Gray
    $inventoryId = $createInventoryResponse.id
    $productId = $createInventoryResponse.productId
}
catch {
    Write-Host "✗ Failed to create inventory: $_" -ForegroundColor Red
}

Write-Host ""

# Reserve Quantity
Write-Host "2. Reserving Quantity..." -ForegroundColor Green
$reserveBody = @{
    quantity = 10
    orderId = [Guid]::NewGuid().ToString()
} | ConvertTo-Json

try {
    $reserveResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/inventory/$inventoryId/reserve" `
        -Method Post `
        -Body $reserveBody `
        -ContentType "application/json"
    
    Write-Host "✓ Quantity reserved successfully" -ForegroundColor Green
    Write-Host "Reserved: $($reserveResponse.reservedQuantity)" -ForegroundColor Gray
    Write-Host "Available: $($reserveResponse.availableQuantity)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ Failed to reserve quantity: $_" -ForegroundColor Red
}

Write-Host ""

# ==========================================
# Order Service Tests
# ==========================================
Write-Host ">>> Testing Order Service" -ForegroundColor Yellow
Write-Host ""

# Create Order
Write-Host "1. Creating Order..." -ForegroundColor Green
$createOrderBody = @{
    customerId = $userId
} | ConvertTo-Json

try {
    $createOrderResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/orders" `
        -Method Post `
        -Body $createOrderBody `
        -ContentType "application/json"
    
    Write-Host "✓ Order created successfully" -ForegroundColor Green
    Write-Host "Order ID: $($createOrderResponse.id)" -ForegroundColor Gray
    $orderId = $createOrderResponse.id
}
catch {
    Write-Host "✗ Failed to create order: $_" -ForegroundColor Red
}

Write-Host ""

# Add Order Item
Write-Host "2. Adding Order Item..." -ForegroundColor Green
$addItemBody = @{
    productId = $productId
    quantity = 5
    unitPrice = 29.99
} | ConvertTo-Json

try {
    $addItemResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/orders/$orderId/items" `
        -Method Post `
        -Body $addItemBody `
        -ContentType "application/json"
    
    Write-Host "✓ Order item added successfully" -ForegroundColor Green
    Write-Host "Total Amount: $($addItemResponse.totalAmount)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ Failed to add order item: $_" -ForegroundColor Red
}

Write-Host ""

# Confirm Order
Write-Host "3. Confirming Order..." -ForegroundColor Green
try {
    $confirmResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/orders/$orderId/confirm" `
        -Method Post
    
    Write-Host "✓ Order confirmed successfully" -ForegroundColor Green
    Write-Host "Status: $($confirmResponse.status)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ Failed to confirm order: $_" -ForegroundColor Red
}

Write-Host ""

# Get Order
Write-Host "4. Getting Order..." -ForegroundColor Green
try {
    $getOrderResponse = Invoke-RestMethod -Uri "$gatewayUrl/api/orders/$orderId" `
        -Method Get
    
    Write-Host "✓ Order retrieved successfully" -ForegroundColor Green
    Write-Host "Order Status: $($getOrderResponse.status)" -ForegroundColor Gray
    Write-Host "Items Count: $($getOrderResponse.items.Count)" -ForegroundColor Gray
    Write-Host "Total: $($getOrderResponse.totalAmount)" -ForegroundColor Gray
}
catch {
    Write-Host "✗ Failed to get order: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Suite Completed" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
