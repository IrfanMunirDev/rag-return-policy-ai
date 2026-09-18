# 🧠 Return Policy AI

extended from [Telerik Blog](https://www.telerik.com/blogs/return-policy-ai-smart-answers-for-your-return-policy-powered-by-openai-and-aspnet-core)

**This project contains a sample ASP.NET Core app. This app is an example of the article I produced for the Telerik Blog (telerik.com/blogs)**

🤖 **Smart answers for your return policy, powered by OpenAI and ASP.NET Core!**

## 🚀 Overview

**Return Policy AI** is an intelligent ASP.NET Core web application that helps users understand a store’s return policy.
By combining a **policy text file** 📝 and a **user question** 💬, it generates accurate and natural answers using the **OpenAI API**.

Perfect for e-commerce platforms, customer support systems, or anyone who wants to automate policy-based Q&A!

## 🛠️ Tech Stack

| Technology                 | Description                                            |
| -------------------------- | ------------------------------------------------------ |
| 🧩 **ASP.NET Core**        | Backend framework for API and web logic                |
| 🔑 **OpenAI API**          | Provides natural language understanding and generation |
| 💾 **C#**                  | Main programming language                              |
| ⚡ **Dependency Injection** | For clean and modular architecture                     |
| 🧠 **AI Services Layer**   | Handles prompt creation and OpenAI calls               |

---

## 💬 Example

**Policy file content:**

> Items can be returned within 30 days of purchase with a receipt.

**User question:**

> Can I return something after 40 days?

**AI answer:**

> Sorry! According to the policy, returns are accepted only within 30 days of purchase with a valid receipt.

---

## ❤️ Contributing

Pull requests are welcome!
If you’d like to contribute, please open an issue first to discuss the change you’d like to make.

---

## Ollama
I've updated appsettings.json for Ollama, and we can  use OpenAI API as well as free models too. You can run the app with Ollama by following these steps:
`ollama run phi4-mini`


## How To Run
`
in post man or post request send tool of your choice,  add following 
post: https://localhost:7284/api/policy/ask

body: {
    "question": "Can I return an opened product?"
}
update the port if needed.
`

## 📜 License

This project is licensed under the **MIT License**.
