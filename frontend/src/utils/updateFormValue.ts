import type { CalculatorFormValues } from '../types/tax'

export function updateFormValue(
  values: CalculatorFormValues,
  name: string,
  value: string,
): CalculatorFormValues {
  switch (name) {
    case 'oldRegimeDeductions.section80C':
      return { ...values, oldRegimeDeductions: { ...values.oldRegimeDeductions, section80C: value } }
    case 'oldRegimeDeductions.section80D':
      return { ...values, oldRegimeDeductions: { ...values.oldRegimeDeductions, section80D: value } }
    case 'oldRegimeDeductions.otherDeductions':
      return { ...values, oldRegimeDeductions: { ...values.oldRegimeDeductions, otherDeductions: value } }
    case 'taxYear':
      return { ...values, taxYear: value }
    case 'ageCategory':
      if (value === 'below60' || value === '60to79' || value === '80plus') {
        return { ...values, ageCategory: value }
      }
      return values
    case 'grossSalary':
      return { ...values, grossSalary: value }
    case 'salaryAllowancesExemptBothRegimes':
      return { ...values, salaryAllowancesExemptBothRegimes: value }
    case 'otherIncome':
      return { ...values, otherIncome: value }
    case 'housePropertyInterest':
      return { ...values, housePropertyInterest: value }
    case 'newRegimeDeductions':
      return { ...values, newRegimeDeductions: value }
    default:
      return values
  }
}
