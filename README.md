# SkySports EPL News Publisher - Start Guide

## How to Run the System

### 1. Start the Press

```bash
# Navigate to publisher directory
cd ThePress

# Install required packages
dotnet add package HtmlAgilityPack
dotnet add package iTextSharp
dotnet add package Newtonsoft.Json

# Run the publisher (default port: 8080)
dotnet run
```

### 2. Start One or More Subscribers

```bash
# Navigate to subscriber directory
cd NewsSubscriber

# Install required packages
dotnet add package Newtonsoft.Json

# Run the subscriber (connects to localhost:8080 by default)
dotnet run
```

## What Happens Next

1. The press will:
    - Immediately scrape SkySports for EPL news
    - Continue scraping every 60 minutes
    - Broadcast new articles to all connected subscribers

2. Each subscriber will:
    - Connect to the publisher
    - Receive news articles as PDF files
    - Save them in the current directory with timestamped filenames