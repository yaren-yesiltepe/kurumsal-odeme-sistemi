import { useEffect, useState, useCallback } from 'react'
import { fetchPayments } from '../api/client'

const STATUS_OPTIONS = ['ALL', 'SUCCESS', 'FAILED', 'PENDING', 'REFUNDED']

export default function PaymentTable({ refreshKey }) {
  const [payments, setPayments] = useState([])
  const [status, setStatus] = useState('ALL')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const load = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await fetchPayments({
        take: 100,
        status: status === 'ALL' ? undefined : status,
      })
      setPayments(data)
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }, [status])

  useEffect(() => {
    load()
  }, [load, refreshKey])

  return (
    <div>
      <div className="filters">
        <select value={status} onChange={(e) => setStatus(e.target.value)}>
          {STATUS_OPTIONS.map((s) => (
            <option key={s} value={s}>{s}</option>
          ))}
        </select>
        <button className="primary" onClick={load} type="button">Refresh</button>
      </div>

      {error && <div className="msg-err">{error}</div>}
      {!error && loading && <div className="empty-state">Loading...</div>}
      {!error && !loading && payments.length === 0 && (
        <div className="empty-state">No transactions yet.</div>
      )}

      {!error && !loading && payments.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Reference</th>
              <th>Merchant</th>
              <th>Card</th>
              <th>Amount</th>
              <th>Status</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            {payments.map((p) => (
              <tr key={p.id}>
                <td>{p.referenceNo}</td>
                <td>{p.merchantName}</td>
                <td>{p.cardType} •••• {p.last4}</td>
                <td>{p.amount.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} {p.currency}</td>
                <td><span className={`badge badge-${p.status}`}>{p.status}</span></td>
                <td>{new Date(p.createdAt).toLocaleString('tr-TR')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
