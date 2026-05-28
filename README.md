# 🔬 Research Hypothesis Generator

A web application that helps university students generate testable research hypotheses using AI. Enter any research topic and instantly get 5 AI-generated hypotheses complete with difficulty ratings, suggested methodologies, and explanations.

**Live Demo:** [hypothesis-generator-production.up.railway.app](https://hypothesis-generator-production.up.railway.app)

---

## ✨ Features

- **AI-Powered Generation** — Generates 5 unique, testable research hypotheses per topic
- **Difficulty Levels** — Filter by Beginner, Intermediate, Advanced, or Mixed
- **Suggested Methodology** — Each hypothesis comes with a recommended research method (Survey, Experiment, Case Study, etc.)
- **Regenerate Single Hypothesis** — Don't like one? Regenerate just that card without losing the rest
- **Dark / Light Mode** — Toggle between themes
- **Example Topics** — Quick-start buttons for common research areas
- **Responsive Design** — Works on desktop and mobile

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core 8 (Razor Pages) |
| Language | C# |
| AI API | Groq API (LLaMA 3.3 70B) |
| Frontend | Bootstrap 5, Bootstrap Icons |
| Fonts | Google Fonts (Inter, Playfair Display) |
| Hosting | Railway |
| Version Control | Git / GitHub |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A free [Groq API key](https://console.groq.com)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/theodor-d/hypothesis-generator.git
   cd hypothesis-generator
   ```

2. **Add your Groq API key**

   Open `appsettings.json` and add your key:
   ```json
   {
     "GroqApiKey": "your_groq_api_key_here"
   }
   ```

3. **Run the app**
   ```bash
   dotnet run
   ```

4. Open your browser at `https://localhost:5001`

---

## 📸 Screenshots

### Home Page
> Enter your research topic and select a difficulty level
> <img width="1850" height="878" alt="image" src="https://github.com/user-attachments/assets/737f222b-40a3-497a-b935-5d947482f14e" />


### Results Page
> View 5 generated hypotheses with difficulty badges, methodology tags, and explanations
<img width="1830" height="874" alt="image" src="https://github.com/user-attachments/assets/21481310-5d4f-4259-8926-5db86f8e79b4" />

---

## 💡 How It Works

1. User enters a research topic (e.g. *"social media and sleep quality in teenagers"*)
2. The app sends a structured prompt to the Groq API (LLaMA 3.3 70B model)
3. The AI returns 5 hypotheses as structured JSON
4. Results are parsed and displayed as interactive cards
5. Users can regenerate individual hypotheses they don't like

---

## 📁 Project Structure

```
HypothesisGenerator/
├── Models/
│   ├── HypothesisRequest.cs      # Input model (topic + difficulty)
│   └── HypothesisResult.cs       # Output model (statement, difficulty, methodology, explanation)
├── Pages/
│   ├── Index.cshtml               # Home page (input form)
│   ├── Index.cshtml.cs            # Home page logic
│   ├── Results.cshtml             # Results page (hypothesis cards)
│   ├── Results.cshtml.cs          # Results page logic + single regenerate handler
│   └── Shared/
│       └── _Layout.cshtml         # Shared layout (navbar, footer, theme toggle)
├── Services/
│   └── GeminiService.cs           # Groq API integration
├── Program.cs                     # App entry point
└── appsettings.json               # Configuration (API key goes here)
```

---

## 🔐 Environment Variables

For production deployment, set the following environment variable instead of using `appsettings.json`:

| Variable | Description |
|----------|-------------|
| `GroqApiKey` | Your Groq API key from [console.groq.com](https://console.groq.com) |

---

## 👤 Author

**Theodor** ([@theodor-d](https://github.com/theodor-d))

Built with assistance from [Claude](https://claude.ai) by Anthropic.

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).
