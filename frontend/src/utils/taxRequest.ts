import type { CalculatorFormValues, CompareTaxRequest, TaxAmountInput } from '../types/tax'

function toAmount(value: TaxAmountInput | null | undefined): number {
  if (value === '' || value === null || value === undefined) return 0
  const amount = Number(value)
  return Number.isFinite(amount) ? amount : 0
}

export function toTaxRequest(values: CalculatorFormValues): CompareTaxRequest {
  return {
    taxYear: values.taxYear,
    ageCategory: values.ageCategory,
    grossSalary: toAmount(values.grossSalary),
    salaryAllowancesExemptBothRegimes: toAmount(values.salaryAllowancesExemptBothRegimes),
    otherIncome: toAmount(values.otherIncome),
    housePropertyInterest: toAmount(values.housePropertyInterest),
    oldRegimeDeductions: {
      section80C: toAmount(values.oldRegimeDeductions.section80C),
      section80D: toAmount(values.oldRegimeDeductions.section80D),
      otherDeductions: toAmount(values.oldRegimeDeductions.otherDeductions),
    },
    newRegimeDeductions: toAmount(values.newRegimeDeductions),
  }
}

