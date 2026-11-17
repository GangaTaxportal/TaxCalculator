const monetaryFields = [
  'grossSalary',
  'salaryAllowancesExemptBothRegimes',
  'otherIncome',
  'housePropertyInterest',
  'newRegimeDeductions',
]

function toAmount(value) {
  if (value === '' || value === null || value === undefined) return 0
  const amount = Number(value)
  return Number.isFinite(amount) ? amount : 0
}

export function toTaxRequest(values) {
  return {
    ...values,
    ...Object.fromEntries(monetaryFields.map((field) => [field, toAmount(values[field])])),
    oldRegimeDeductions: Object.fromEntries(
      Object.entries(values.oldRegimeDeductions).map(([field, value]) => [field, toAmount(value)]),
    ),
  }
}
