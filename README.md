# ⚡ Virtual Power Grid Simulator

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![React](https://img.shields.io/badge/frontend-React_%7C_TypeScript-61DAFB?logo=react)
![MUI](https://img.shields.io/badge/UI-Material--UI-007FFF?logo=mui)
![.NET](https://img.shields.io/badge/backend-.NET_8-512BD4?logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/database-PostgreSQL-4169E1?logo=postgresql)

> An interactive Energy Management System (EMS) simulator that models the physics of an isolated power grid ("Island Mode"). It visualizes frequency dynamics, generation control, and load shedding in real-time.

<img width="1920" height="903" alt="image" src="https://github.com/user-attachments/assets/2f690c7f-e84b-4dc7-bc03-eb5be7010956" />


## 🌟 Key Features

### 🎮 Interactive Control
- **Manual Dispatch:** Control power plants manually using switches and sliders.
- **Real-time Physics:** Grid frequency fluctuates based on the balance between Generation and Demand ($F = 50 + \Delta P \cdot k$).
- **Weather Dependence:** Solar and Wind plants are automated and cannot be manually adjusted, simulating real-world weather constraints.

### 📊 Visualization & Analytics
- **Live Frequency Chart:** Real-time `Recharts` area chart monitoring grid stability (49.0Hz - 51.0Hz range).
- **Dynamic UI:** Glassmorphism design with reactive gradients that change based on plant status and system balance.
- **Consumer Monitoring:** Visual status of load distribution (Hospitals, Residential, Industry) with priority levels.

### ⚡ System Logic
- **AFLS (Automatic Frequency Load Shedding):** If frequency drops below critical levels, low-priority consumers are automatically disconnected to prevent system collapse.
- **Plant Types:**
  - 🏭 **Thermal:** Manually adjustable (Base load/Peaking).
  - ☢️ **Nuclear:** High inertia, base load.
  - ☀️ **Solar:** Weather-dependent (Day/Night cycle simulation).
  - 💨 **Wind:** Weather-dependent (Stochastic generation).
  - 💧 **Hydro:** Fast response for frequency containment.

## 🛠️ Tech Stack

### Frontend
- **Framework:** React 18 (Vite)
- **Language:** TypeScript
- **Styling:** Material UI (MUI v5) with custom dark theme & glassmorphism.
- **Charts:** Recharts
- **State Management:** React Hooks
- **HTTP Client:** Axios

### Backend
- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** PostgreSQL (Entity Framework Core)
- **Architecture:** RESTful API, Repository Pattern, Background Services (HostedService) for simulation ticks.

## 🚀 Getting Started

### Prerequisites
- Node.js (v18+)
- .NET 8 SDK
- PostgreSQL

### Installation

1. **Clone the repository**
   ```bash
   git clone [https://github.com/your-username/virtual-power-grid.git](https://github.com/your-username/virtual-power-grid.git)
   ````

2. **Setup Backend**
    ````bash
    cd server
    # Update ConnectionStrings in appsettings.json if needed
    dotnet restore
    dotnet ef database update
    dotnet run
    ````
3. **Setup Frontend**
    ````bash
    cd client
    npm install
    npm run dev
    ````
4. **Access the App**
    Open http://localhost:5173 in your browser.

### 📐 Architecture
The simulation runs on a background thread in the .NET backend (SimulationWorker), calculating the physics every second. The frontend polls the API for the GridSnapshot and updates the UI state optimistically for instant feedback.

### 🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

### 📝 License
This project is licensed under the MIT License.
