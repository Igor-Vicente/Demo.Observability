## To test locally:

```bash
# Aspire Dashboard (visualize telemetry)
docker run -d -p 18889:18889 -p 18890:18890 -p 18888:18888 --name dashboard mcr.microsoft.com/dotnet/aspire-dashboard
```

```bash
# Seq (visualize logging and tracing)
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```

## Refs

- https://opentelemetry.io/docs/what-is-opentelemetry/
- https://learn.microsoft.com/en-us/dotnet/core/diagnostics/

# Observability

Observability requires **instrumentation**.

Instrumentation is code that is added to a software project to record what it is doing. This information can then be collected in files, databases, or in-memory and analyzed to understand how a software program is operating.

**Three Key Pillars of Observability:**

- **Logging:** Logging is a technique where code is instrumented to produce a log, a record of interesting events that occurred while the program was running.
- **Tracing:** Distributed Tracing is a specialized form of logging that helps you localize failures and performance issues within applications distributed across multiple machines or processes.
- **Metrics:** Numerical measurements over time for performance monitoring and trend analysis.

## 📌 What Is Serilog?

Serilog is a structured logging library for .NET applications. Unlike traditional logging, which outputs simple text messages, Serilog logs data in structured formats (like JSON), making it easier to analyze, filter, and search logs.

**_🔹 Why Use Serilog Instead of Default .NET Logging?_**

.NET applications come with Microsoft.Extensions.Logging, which provides basic logging. **_Serilog enhances logging by:_**

- ✅ Structured Logging → Logs data as key-value pairs (e.g., JSON).
- ✅ Sinks → Allows logging to multiple destinations (e.g., Console, File, Elasticsearch, Seq, etc.).
- ✅ Enrichers → Automatically adds metadata (e.g., request ID, user info).
- ✅ Filtering → Enables fine-grained control over what gets logged.

**_🔹 Popular Serilog Sinks_**

A sink is where Serilog sends logs. Some common ones:

- 📌 Console → WriteTo.Console()
- 📌 File → WriteTo.File("logs.txt")
- 📌 Elasticsearch → WriteTo.Elasticsearch(...)
- 📌 Seq → WriteTo.Seq("http://localhost:5341")
- 📌 Application Insights → WriteTo.ApplicationInsights(...)

## Sinks vs. Application Performance Management (APM)

| Feature                | APM                                                                                         | Sink                                                       |
| :--------------------- | :------------------------------------------------------------------------------------------ | :--------------------------------------------------------- |
| **Purpose**            | Monitors, analyzes, and optimizes app performance.                                          | Destinations for log data.                                 |
| **Data Handling**      | Provides dashboards, alerts, and AI-based insights. Acts as a storage destination for data. | Stores logs, traces, or metrics, but doesn’t analyze them. |
| **Examples**           | Datadog, New Relic, Splunk APM, Application Insights                                        | Elastic APM Seq, Elasticsearch, Prometheus, Loki           |
| **OpenTelemetry Role** | Receives logs, traces, and metrics from OpenTelemetry for analysis.                         | Stores logs, traces, or metrics, but doesn’t analyze them. |

## What Is OpenTelemetry (OTel)?

OpenTelemetry (OTel) is an **open-source** observability framework for instrumenting, generating, collecting, and exporting telemetry data (logs, traces, and metrics) from applications.

It helps developers monitor applications across different environments (cloud, on-prem, hybrid) without being locked into a specific vendor like Datadog, New Relic, or Azure Application Insights.

**_🔹 Why Use OpenTelemetry?_**

- ✅ Vendor-neutral – Works with multiple backends like Prometheus, Jaeger, Zipkin, Elasticsearch, etc.
- ✅ Cross-language support – Supports .NET, Java, Python, Go, and more.
- ✅ Standardized instrumentation – Provides a consistent API for collecting telemetry data.
- ✅ Reduces vendor lock-in – You can switch monitoring tools without modifying your application’s instrumentation.

**_🔹 How OpenTelemetry Works in .NET?_**

- 1️⃣ Instrumentation – Collects telemetry data (traces, logs, metrics).
- 2️⃣ OTel SDK & API – Formats data in OpenTelemetry standard.
- 3️⃣ Exporters – Sends data to a backend (e.g., Jaeger, Prometheus, Azure Monitor).

### How OTel Keeps Track (Trace) Between Underlying Systems (HTTP Request Example)

OpenTelemetry uses **distributed tracing** and **context propagation** to follow a request across multiple services. Here's a more detailed breakdown:

1️⃣. **Trace and Span Creation:** When an operation begins (e.g., an incoming HTTP request in Service A), the OTel SDK creates a **trace**. This trace has a unique **Trace ID**. Within the trace, the operation in Service A is represented by a **span**, which has its own unique **Span ID** and is associated with the Trace ID. This is the **root span** of the trace for this entry point.

2️⃣. **Context Propagation using Injectors:** When Service A needs to make an outgoing call to Service B (e.g., an HTTP request using `HttpClient`), the OTel SDK's **injector** comes into play. The injector is configured for a specific **propagation format** (the standard is **W3C Trace Context**). It does the following:

    - Retrieves the current Trace ID and Span ID from the active context in Service A.
    - Serializes this context information into HTTP headers that will be added to the outgoing request. The primary header is `traceparent`, which typically contains the trace ID, the current span ID, and flags (e.g., indicating if the trace should be sampled).

3️⃣. **Context Extraction using Extractors:** When Service B receives the incoming HTTP request from Service A, the OTel SDK's **extractor** on the Service B side examines the incoming HTTP headers.

    - It looks for the configured propagation headers (like `traceparent`).
    - If the headers are present and valid, the extractor deserializes the trace context, retrieving the Trace ID and the Span ID of the operation in Service A that initiated the call.

4️⃣. **Creating Child Spans and Linking Context:** In Service B, the OTel SDK uses the extracted Trace ID to ensure that the work done in response to the incoming request belongs to the _same_ distributed trace. It then creates a new **child span** representing the operation within Service B. This child span is linked to the span in Service A (the "parent span") through the extracted Span ID. This establishes the parent-child relationship within the trace, showing the flow of the request.

5️⃣. **Continuing Propagation:** If Service B, in turn, makes calls to other downstream services (Service C, etc.), the same injection and extraction process repeats, ensuring that the trace context continues to propagate across all involved systems.

6️⃣. **Telemetry Export and Trace Reconstruction:** Each service, instrumented with the OTel SDK, exports its generated spans (containing Trace ID, Span ID, parent Span ID, operation name, timestamps, attributes, etc.) to a configured observability backend (like Jaeger, Zipkin, or an APM system). The backend then uses the Trace IDs and the parent-child relationships between the spans to reconstruct the entire end-to-end journey of the request, providing a comprehensive view of the distributed transaction.

**In essence, OpenTelemetry doesn't "know" the systems are connected; it explicitly propagates the context of a trace from one system to another using standardized formats. The receiving system then uses this propagated context to associate its own operations with the same trace, allowing an observability backend to piece together the complete picture.**

**Debugging Trace ID:**

In .NET, you can often access the current `traceId` and `spanId` using:

```csharp
using System.Diagnostics;

// ...

var currentActivity = Activity.Current;
var traceId = currentActivity?.TraceId;
var spanId = currentActivity?.SpanId;

// ...
```

## APM Should Notify When Errors Occur in the Application

### Steps to Configure an Alert for 500 Errors in Application Insights

**1️⃣ Create a Log-Based Alert for 500 Errors**

1.  Go to the [Azure Portal](https://portal.azure.com/) and navigate to your Application Insights resource.
2.  In the left menu, go to **Monitoring** > **Alerts**.
3.  Click **Create** > **Alert rule**.
4.  Under **Scope**, ensure your Application Insights instance is selected.
5.  Under **Condition**, click **Add condition**.
6.  In the **Signal name** list, select **Custom log search**.
7.  In the **Query** field, enter the following Kusto Query Language (KQL) query to detect HTTP 500 errors:

    ```kusto
    requests
    | where resultCode == "500"
    | summarize count() by bin(timestamp, 5m)
    ```

8.  Click **Done** to save the condition.

**2️⃣ Set Up Notification (Email, SMS, Teams, etc.)**

1.  Under **Actions**, click **Add action group**.
2.  Click **Create action group** and configure:
    - **Action type**: Select **Email/SMS/Push/Voice** or **Azure Monitor action** (e.g., Logic App, Function, Webhook, Teams).
    - Provide recipient details (email address, phone number, etc.) based on the selected action type.
3.  Click **Review + Create** to finalize the action group.
