const BASE_URL = '/api';
async function handleResponse(res) {
  if (!res.ok) {
    let detail = res.statusText
    try {
      const body = await res.json()
      detail = body.error || JSON.stringify(body)
    } catch {
      // response had no json body, ignore
    }
    throw new Error(detail || `Request failed with ${res.status}`)
  }
  return res.json()
}

export async function createPayment(payload) {
  const res = await fetch(`${BASE_URL}/payments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  })
  return handleResponse(res)
}

export async function fetchPayments({ take = 50, status } = {}) {
  const params = new URLSearchParams({ take: String(take) })
  if (status) params.set('status', status)
  const res = await fetch(`${BASE_URL}/payments?${params.toString()}`)
  return handleResponse(res)
}

export async function fetchDailyVolume(daysBack = 30) {
  const res = await fetch(`${BASE_URL}/reports/daily-volume?daysBack=${daysBack}`)
  return handleResponse(res)
}
