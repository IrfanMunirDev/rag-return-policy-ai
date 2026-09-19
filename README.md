
---

# 🧠 Return Policy AI (Advanced RAG & Local CI/CD Pipeline)

🤖 **Smart answers for your return policy, powered by Local AI (Ollama) / OpenAI and ASP.NET Core!**

This project is an advanced, production-grade evolution of the sample application featured in the Telerik Blog article: [Building RAG in ASP.NET Core](https://www.telerik.com/blogs/building-rag-aspnet-core?utm_source=gemini).

---

## 🚀 Overview & Evolution of Features

We forked and significantly expanded the original sample project to build a full-fledged Retrieval-Augmented Generation (RAG) pipeline coupled with a zero-cost local CI/CD workflow.

### 1. Architectural Milestones Built:

* **Prompt Stuffing Controller (Simple Approach):** Initially implemented a straightforward service to pass policy text directly into prompt generation.
* **True RAG Embedding Controller (Advanced Approach):** Upgraded to a semantic vector search system:
* **Text Chunking:** Splits long policies into manageable paragraphs.

* **Local Embeddings:** Uses Ollama's `nomic-embed-text` model to generate numerical vectors.


* **Vector Similarity Search:** Matches user queries against the most relevant text chunks before querying the chat model.

* **Database Persistence via Entity Framework Core (EF Core):** Integrated **SQLite** (`policy.db`) using **EF Core** to store text chunks and byte-serialized vector embeddings (BLOBs), preventing redundant processing on application restarts.


* **Flexible LLM Support:** Configured to run entirely locally using Ollama (`phi4-mini` for text generation and `nomic-embed-text` for embeddings), with fallback support for the OpenAI API.


---

## 🧪 Unit Testing & TDD Suite

Following a strict **Test-Driven Development (TDD)** approach, the solution features a dedicated test project (`ResturnPolicyTests`) utilizing **xUnit**.

* **Test Coverage (5/5 Tests Passing):**
* Validates edge cases for **Document Chunking** (empty files, boundary limits).


* Tests vector math and similarity ranking algorithms deterministically.

* **Mocking & Isolation:** Leveraging `Microsoft.Extensions.AI`, standard interfaces (`IChatClient` and `IEmbeddingGenerator`) are fully mockable, allowing the unit test suite to run instantly in memory without requiring a live Ollama connection or paid API keys.

---

---

## 🛠️ Tech Stack

| Technology | Description |
| --- | --- |
| 🧩 **ASP.NET Core 10** | Backend framework for API and web logic |
| 🗄️ **EF Core & SQLite** | Local database persistence for vector embeddings and text chunks

 |
| 🦙 **Ollama** | Local AI runner powering `phi4-mini` and `nomic-embed-text`<br> |
| 🐳 **Docker Desktop** | Containerization tool for local packaging and deployment |
| ⚙️ **GitHub Actions** | CI/CD automation utilizing a custom **Self-Hosted Runner**<br> |

---

## ⚙️ GitHub Actions Self-Hosted Runner Setup

Because local AI models (Ollama) run securely on your local network (`localhost:11434`), cloud-hosted runners (like GitHub's default `ubuntu-latest`) cannot reach them due to network firewalls. To solve this and build a zero-cost local CI/CD pipeline, we configured a **GitHub Actions Self-Hosted Runner** on a local Windows machine.

*(Note: While Azure DevOps also provides powerful self-hosted agent capabilities for local deployment, we fully configured and verified the GitHub Actions self-hosted runner for this repository.)*

### How to Install and Configure a GitHub Self-Hosted Runner on Windows:

1. **Navigate to GitHub Settings:**
* Go to your repository on GitHub: [`/rag-return-policy-ai`](https://www.google.com/url?sa=E&source=gmail&q=https://github.com/IrfanMunirDev/rag-return-policy-ai)[cite: 1]
* Click on **Settings** $\rightarrow$ **Actions** $\rightarrow$ **Runners** $\rightarrow$ **New runner**.

`github action when click on new runner also give you insturcitons that you can use to configure Self-Hosted Runner`

2. **Select Operating System:** Choose **Windows** and **x64**.
3. **Execute Configuration Commands in PowerShell:** Open PowerShell on your local machine and run the commands provided by GitHub:
```powershell
# Create a folder for the runner
mkdir actions-runner ; cd actions-runner

# Download the runner package
Invoke-WebRequest -Uri https://github.com/actions/runner/releases/download/v2.337.0/actions-runner-win-x64-2.337.0.zip -OutFile actions-runner-win-x64-2.337.0.zip

# Extract the package
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory("$PWD/actions-runner-win-x64-2.337.0.zip", "$PWD")

# Configure the runner with your unique repository token provided by GitHub
./config.cmd --url https://github.com/IrfanMunirDev/rag-return-policy-ai --token YOUR_UNIQUE_GITHUB_TOKEN

```

4. **Start Listening for Jobs:**
Launch the runner interactively in your terminal:
```powershell
./run.cmd

```

*(Your terminal will display: `Listening for Jobs`)*[cite: 4]

---

## 🔄 CI/CD Pipeline Workflow (`ci.yml`)

We configured `.github/workflows/ci.yml` to handle both continuous integration (CI) quality checks and continuous deployment (CD) locally:

1. **CI Level (Build & Test):** Every time code is pushed or a pull request is opened against the `main` branch, the self-hosted runner checks out the repository, validates the .NET version, restores NuGet packages, builds the solution, and runs unit tests via `dotnet test RAGReturnPolicy.sln`[cite: 4].
2. **CD Level (Local Deployment):** Upon successful completion of the build and test steps, the pipeline automatically:
* Builds a fresh Docker image tagged `stage-dev-rag-retun-policy:latest`.
* Gracefully stops and removes any existing local container named `stage-dev-rag-retun-policy`.
* Spins up a brand-new container instance mapped to port `8080`, injecting environment variables to seamlessly communicate with your local Ollama instance (`[http://host.docker.internal:11434/v1/](http://host.docker.internal:11434/v1/)`).

---

## 🏃 How To Run Locally via Docker Compose

### 1. Start Ollama Models

Ensure Ollama is running natively on your host machine with both required models active:

```bash
ollama run phi4-mini
ollama run nomic-embed-text

```

### 2. Run with Docker Compose

From the root folder of your repository, build and spin up the container network:

```bash
docker compose up --build

```

*(This automatically mounts your local `Data/` folder, connects to Ollama via `host.docker.internal`, and serves the API on port `8080`).*

### 3. Test the Endpoints

Send a `POST` request using Postman or your preferred HTTP client:

* **Prompt Stuffing Endpoint:**

`use https if needed, we are using local deployed version in docker, and certificates are not installed in docker so using the http`
`POST http://localhost:8080/api/PolicyRagPromptStuffing/ask`
* **RAG Embedding Endpoint:**
`POST http://localhost:8080/api/PolicyRagEmbedding/ask`

**Request Body (JSON):**

```json
{
    "question": "Can I return an opened product?"
}

```

---

## 📜 License

This project is licensed under the **MIT License**.
