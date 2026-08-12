import { useState } from 'react'
import { createPayment } from '../api/client'

const CARD_TYPES = ['VISA', 'MASTERCARD', 'TROY', 'AMEX']

export default function PaymentForm({ onCreated }) {
  const [form, setForm] = useState({
    merchantName: '',
    cardNumber: '',
    cardType: 'VISA',
    amount: '',
    currency: 'TRY',
  })
  const [submitting, setSubmitting] = useState(false)
  const [feedback, setFeedback] = useState(null)

  function updateField(key, value) {
    setForm((prev) => ({ ...prev, [key]: value }))
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setSubmitting(true)
    setFeedback(null)

    try {
      const result = await createPayment({
        ...form,
        amount: Number(form.amount),
      })
      setFeedback({ type: 'ok', text: `${result.referenceNo} -> ${result.status}` })
      onCreated?.(result)
      setForm((prev) => ({ ...prev, merchantName: '', cardNumber: '', amount: '' }))
    } catch (err) {
      setFeedback({ type: 'err', text: err.message })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <div className="form-grid">
        <label>
          Merchant
          <input
            required
            value={form.merchantName}
            onChange={(e) => updateField('merchantName', e.target.value)}
            placeholder="Migros Ticaret A.S."
          />
        </label>

        <label>
          Card number (mock)
          <input
            required
            maxLength={19}
            value={form.cardNumber}
            onChange={(e) => updateField('cardNumber', e.target.value.replace(/[^0-9]/g, ''))}
            placeholder="4111111111111111"
          />
        </label>

        <label>
          Card type
          <select value={form.cardType} onChange={(e) => updateField('cardType', e.target.value)}>
            {CARD_TYPES.map((c) => (
              <option key={c} value={c}>{c}</option>
            ))}
          </select>
        </label>

        <label>
          Amount
          <input
            required
            type="number"
            step="0.01"
            min="0.01"
            value={form.amount}
            onChange={(e) => updateField('amount', e.target.value)}
            placeholder="1250.00"
          />
        </label>
      </div>

      <div className="submit-row">
        <button className="primary" type="submit" disabled={submitting}>
          {submitting ? 'Processing...' : 'Process payment'}
        </button>
        {feedback && (
          <span className={feedback.type === 'ok' ? 'msg-ok' : 'msg-err'}>
            {feedback.text}
          </span>
        )}
      </div>
    </form>
  )
}
