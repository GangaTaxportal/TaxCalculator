const baseUrl = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/$/, '')

export async function compareTax(request) {
  const endpoint = `${baseUrl}/api/tax/compare`
  let response
  try {
    response = await fetch(endpoint, {
      method: 'POST', headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
      body: JSON.stringify(request),
    })
  } catch (cause) {
    const target = baseUrl ? endpoint : '/api/tax/compare (Vite proxy → https://localhost:7206)'
    throw new Error(`Could not reach ${target}. Start TaxCalculatorAPI with its HTTPS launch profile and confirm it is listening on https://localhost:7206.`, { cause })
  }
  const data = await response.json().catch(() => ({}))
  if (!response.ok) {
    const validationErrors = data.errors && Object.values(data.errors).flat()
    throw Object.assign(new Error(data.detail || data.title || 'The request could not be processed.'), {
      messages: validationErrors?.length ? validationErrors : [data.detail || data.title || `Request failed (${response.status}).`],
    })
  }
  return data
}
