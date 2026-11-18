const BASE = '/api'

export async function compareTax(request) {
  const response = await fetch(`${BASE}/tax/compare`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
    body: JSON.stringify(request),
  })

  const responseText = await response.text()
  let payload = null
  try {
    payload = responseText ? JSON.parse(responseText) : null
  } catch {
    payload = null
  }

  if (!response.ok) {
    const validationErrors = payload?.errors && Object.values(payload.errors).flat()
    const message = payload?.message || payload?.detail || payload?.title || responseText || `Request failed (${response.status}).`
    throw Object.assign(new Error(message), {
      messages: validationErrors?.length ? validationErrors : [message],
    })
  }

  const resultStatus = payload?.resultStatus ?? payload?.ResultStatus
  const message = payload?.message ?? payload?.Message
  switch (String(resultStatus).toLowerCase()) {
    case 'success': {
      const data = payload?.data ?? payload?.Data
      if (data == null) {
        const errorMessage = message || 'The tax API returned a successful status without result data.'
        throw Object.assign(new Error(errorMessage), { messages: [errorMessage] })
      }
      return data
    }
    case 'validationfailure':
    case 'error': {
      const errorMessage = message || (String(resultStatus).toLowerCase() === 'validationfailure'
        ? 'The tax calculation request did not pass validation.'
        : 'The tax calculation failed.')
      throw Object.assign(new Error(errorMessage), { messages: [errorMessage] })
    }
    default: {
      const errorMessage = `The tax API returned an unsupported result status: ${String(resultStatus)}.`
      throw Object.assign(new Error(errorMessage), { messages: [errorMessage] })
    }
  }
}
