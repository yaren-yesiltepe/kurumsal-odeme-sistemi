import { useState } from 'react'
import PaymentForm from './PaymentForm'
import PaymentTable from './PaymentTable'
import VolumeChart from './VolumeChart'

export default function Dashboard() {
  const [refreshKey, setRefreshKey] = useState(0)

  return (
    <>
      <div className="card">
        <h2>New payment (mock)</h2>
        <PaymentForm onCreated={() => setRefreshKey((k) => k + 1)} />
      </div>

      <div className="card">
        <h2>Daily volume & 7-day moving average</h2>
        <VolumeChart />
      </div>

      <div className="card">
        <h2>Recent transactions</h2>
        <PaymentTable refreshKey={refreshKey} />
      </div>
    </>
  )
}
