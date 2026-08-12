import { useEffect, useState } from 'react'
import {
  ResponsiveContainer, ComposedChart, Bar, Line, XAxis, YAxis,
  Tooltip, CartesianGrid, Legend,
} from 'recharts'
import { fetchDailyVolume } from '../api/client'

export default function VolumeChart() {
  const [rows, setRows] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    fetchDailyVolume(30)
      .then((data) =>
        setRows(
          data.map((d) => ({
            date: d.txnDate.slice(5, 10),
            total: d.totalAmount,
            movingAvg: d.movingAvg7Day,
            count: d.txnCount,
          }))
        )
      )
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  if (loading) return <div className="empty-state">Loading report...</div>
  if (error) return <div className="msg-err">{error}</div>
  if (rows.length === 0) return <div className="empty-state">Not enough data yet.</div>

  return (
    <ResponsiveContainer width="100%" height={320}>
      <ComposedChart data={rows}>
        <CartesianGrid strokeDasharray="3 3" stroke="#eee" />
        <XAxis dataKey="date" fontSize={12} />
        <YAxis fontSize={12} />
        <Tooltip
          formatter={(value, name) =>
            name === 'count' ? [value, 'Txn count'] : [Number(value).toFixed(2), name]
          }
        />
        <Legend />
        <Bar dataKey="total" name="Daily volume (TRY)" fill="#2f5fdd" radius={[4, 4, 0, 0]} />
        <Line type="monotone" dataKey="movingAvg" name="7-day moving avg" stroke="#e2762f" strokeWidth={2} dot={false} />
      </ComposedChart>
    </ResponsiveContainer>
  )
}
