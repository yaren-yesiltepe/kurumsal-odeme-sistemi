import Dashboard from './components/Dashboard'
import './App.css'

function App() {
  return (
    <div className="app-shell">
      <header className="app-header">
        <h1>Payment Ops Dashboard</h1>
        <span>mock processing environment</span>
      </header>
      <Dashboard />
    </div>
  )
}

export default App
