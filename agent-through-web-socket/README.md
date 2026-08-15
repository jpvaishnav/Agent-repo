# agent-through-web-socket

Small .NET sample showing design and placeholders for using Azure Relay / WebSocket to let an outbound-only machine (Agent X) expose functionality to another service (Service S).

Problem Statement
- Agent X runs on machine M which is not inbound-reachable (no public IP / port forwarding).
- Machine M can open outbound connections only.
- Service S needs to request data/output from Agent X.

Solution (high-level)
- Machine M (Agent X) opens an outbound hybrid connection (Azure Relay) or WebSocket to a cloud relay endpoint and keeps it open.
- Service S connects to the Relay and sends requests addressed to the hybrid connection.
- Relay forwards requests through the already-open tunnel to the listener on Machine M.
- The listener on Machine M forwards requests to the local AI Agent (e.g., https://localhost:5001/get_ai_tools) and returns responses back through the relay to Service S.

Security
- Service S authenticates to Azure Relay using either an approved Managed Identity, or a shared access policy (key + name).
- Relay validates credentials before forwarding traffic.

Project layout
- Program.cs - minimal Web API host (controllers included below).
- Controllers/AiAgentController.cs - dummy AI Agent X endpoints (e.g., /get_ai_tools).
- Controllers/ServiceController.cs - placeholder endpoint for Service S (e.g., /service/check_with_x?q=).
- Relay/AzureRelayHelper.cs - conceptual snippets and notes for Azure Relay usage.
- Relay/ServiceSClient.cs - conceptual placeholder client logic for Service S.

Design diagrams (simple, explanatory)

1) Initial handshake: Machine M registers / opens listener to Relay

  [Machine M (Agent X)]
           |
           | outbound TLS connection
           v
  [Azure Relay namespace]
           ^
           | keeps hybrid connection open (listener)
  [Operator / Admin] - configure relay policies & keys

Sequence:
1. Machine M starts a HybridConnectionListener to sb://<namespace>.servicebus.windows.net/<hybridName>
2. Listener authenticates using SAS-token or Managed Identity (outbound)
3. Listener calls OpenAsync() and keeps the channel alive

2) Request flow: Service S -> Relay -> Machine M -> Agent X -> back

  [Service S] --> (1) Auth + HTTP over Relay --> [Azure Relay]
                     (2) Relay forwards over open tunnel
                                           v
                                    [Machine M: Relay listener]
                                           (3) Listener converts to HTTP to local agent
                                           v
                                    [Agent X (https://localhost:5001/get_ai_tools)]
                                           (4) Agent X responds
                                           ^
                                           (5) Listener sends response back through Relay
                     <--- Service S receives response ---

Sequence (numbered):
1. Service S performs an authenticated request to the Azure Relay hybrid connection (or opens a WebSocket to the relay endpoint). 
2. Azure Relay accepts request and forwards it down the already-open tunnel to Machine M.
3. Relay listener on Machine M receives request, forwards it to local Agent X (e.g., localhost:5001/get_ai_tools).
4. Agent X returns data; listener reads it.
5. Listener sends the response back through the tunnel; Relay returns it to Service S.

Notes and next steps
- The code in this repo is intentionally illustrative and minimal: it provides endpoints and conceptual helpers, not a fully working Relay client/server implementation.
- To turn into a working PoC:
  1. Install the Microsoft.Azure.Relay package and implement the HybridConnectionListener on Machine M.
  2. Implement HybridConnectionClient or HTTP-over-relay on Service S to forward requests.
  3. Secure keys via Azure Key Vault or use Managed Identity for production.

References
- Azure Relay overview: https://learn.microsoft.com/en-us/azure/azure-relay/relay-what-is-it

How to push & open a PR
1. git checkout -b feature/agent-through-web-socket
2. git add agent-through-web-socket && git commit -m "Add agent-through-web-socket project and README"
3. git push --set-upstream origin feature/agent-through-web-socket
4. Open a PR in GitHub from that branch describing the problem and design.

